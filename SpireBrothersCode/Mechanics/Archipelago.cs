using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Joshua's Check (Archipelago): find a random item and send it to a random player (yourself in solo).
/// Filler 40% (4 Block), Useful 30% (draw 1 next turn), Progression 15% (1 energy next turn),
/// Trap 15% (goes to a random enemy instead: 1 Weak, 1 Vulnerable). Rolls use the run RNG so co-op stays in sync.
/// </summary>
public static class Archipelago
{
    public enum Item { Filler, Useful, Progression, Trap }

    public const int FillerBlock = 4;

    public static async Task Check(PlayerChoiceContext ctx, Player owner, CardModel? source, int times = 1)
    {
        for (int i = 0; i < times; i++) await CheckOnce(ctx, owner, source);
    }

    private static async Task CheckOnce(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        var combat = owner.Creature.CombatState;
        if (combat == null || !owner.Creature.IsAlive) return;
        var rng = owner.RunState.Rng.CombatTargets;

        Item item;
        var scouted = owner.Creature.GetPower<ScoutingPower>();
        if (scouted != null)
        {
            item = Item.Progression;
            await PowerCmd.Decrement(scouted);
        }
        else
        {
            int roll = rng.NextInt(100);
            item = roll < 40 ? Item.Filler : roll < 70 ? Item.Useful : roll < 85 ? Item.Progression : Item.Trap;
        }

        if (item == Item.Trap)
        {
            var enemies = combat.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count > 0)
            {
                var enemy = rng.NextItem(enemies)!;
                Announce(item, enemy);
                await PowerCmd.Apply<WeakPower>(ctx, enemy, 1, owner.Creature, source);
                await PowerCmd.Apply<VulnerablePower>(ctx, enemy, 1, owner.Creature, source);
                return;
            }
            item = Item.Filler; // no enemy to trap, so it's just filler
        }

        var players = combat.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList();
        if (players.Count == 0) return;
        Creature receiver = rng.NextItem(players)!;
        Announce(item, receiver);
        switch (item)
        {
            case Item.Filler:
                await CreatureCmd.GainBlock(receiver, FillerBlock, ValueProp.Unpowered, null);
                break;
            case Item.Useful:
                await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, receiver, 1, owner.Creature, source);
                break;
            case Item.Progression:
                await PowerCmd.Apply<EnergyNextTurnPower>(ctx, receiver, 1, owner.Creature, source);
                break;
        }
    }

    // A thought bubble over whoever got the item, so everyone can see what was found.
    private static void Announce(Item item, Creature receiver)
    {
        try
        {
            var key = "SPIREBROTHERS-ARCHIPELAGO." + item.ToString().ToLowerInvariant();
            ThinkCmd.Play(new LocString("powers", key), receiver, 1.5);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Archipelago announce failed: {e}");
        }
    }
}
