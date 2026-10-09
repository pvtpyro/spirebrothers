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
/// Filler 40% (6 Block), Useful 30% (draw 1 next turn), Progression 15% (1 energy next turn),
/// Trap 15% (goes to a random enemy instead: 1 Weak, 1 Vulnerable). Rolls use the run RNG so co-op stays in sync.
/// </summary>
public static class Archipelago
{
    public enum Item { Filler, Useful, Progression, Trap }

    public const int FillerBlock = 6;

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
                Announce(item, enemy, owner);
                await PowerCmd.Apply<WeakPower>(ctx, enemy, 1, owner.Creature, source);
                await PowerCmd.Apply<VulnerablePower>(ctx, enemy, 1, owner.Creature, source);
                return;
            }
            item = Item.Filler; // no enemy to trap, so it's just filler
        }

        var players = combat.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList();
        if (players.Count == 0) return;
        Creature receiver = rng.NextItem(players)!;
        Announce(item, receiver, owner);
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

    // How long a bubble stays up: at least 4 seconds, longer for long lines (the game default was 1.5s, too quick to read).
    private static double Seconds(LocString line) => Math.Max(4.0, line.GetFormattedText().Length * 0.11);

    // A thought bubble over whoever got the item, so everyone can see what was found.
    // The receiver (or the trapped enemy) says what they got. When it went to someone else, Joshua also says who
    // he sent it to, so he can tell what his Checks did. Display only, so it never affects co-op sync.
    private static void Announce(Item item, Creature receiver, Player sender)
    {
        try
        {
            var key = "SPIREBROTHERS-ARCHIPELAGO." + item.ToString().ToLowerInvariant();
            var found = new LocString("powers", key);
            ThinkCmd.Play(found, receiver, Seconds(found));
            if (receiver == sender.Creature) return;
            var sent = new LocString("powers", "SPIREBROTHERS-ARCHIPELAGO.sent");
            sent.Add("Item", new LocString("powers", "SPIREBROTHERS-ARCHIPELAGO.short." + item.ToString().ToLowerInvariant()));
            sent.Add("Name", receiver.Name);
            ThinkCmd.Play(sent, sender.Creature, Seconds(sent));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Archipelago announce failed: {e}");
        }
    }
}
