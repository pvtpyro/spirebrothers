using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards;
using SpireBrothers.SpireBrothersCode.Monsters;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim's Kids: Kid pets standing by his feet (max 8, of course). At the end of his turn each Kid deals 1 damage to a
/// random enemy (2 with Proud Dad); TimTracker triggers that. Targets use the run RNG so co-op stays in sync.
/// </summary>
public static class Kids
{
    public const int Max = 8;

    public static int Count(Player? player) =>
        player?.PlayerCombatState?.Pets.Count(p => p.Monster is Kid && p.IsAlive) ?? 0;

    public static int DamagePerKid(Player? player) => 1 + (player?.Creature?.GetPowerAmount<ProudDadPower>() ?? 0);

    /// <summary>Adds Kids up to the max of 8.</summary>
    public static async Task Gain(PlayerChoiceContext ctx, Player player, int amount, CardModel? source)
    {
        if (player.PlayerCombatState == null || player.Creature.CombatState == null || !player.Creature.IsAlive) return;
        for (int i = 0; i < amount && Count(player) < Max; i++)
            await PlayerCmd.AddPet<Kid>(player);
        TimCard.RefreshHand(player);
    }

    /// <summary>Every Kid hits a random enemy. Used at end of turn and by Honey-Do List.</summary>
    public static async Task Act(PlayerChoiceContext ctx, Player player)
    {
        var combat = player.Creature.CombatState;
        int kids = Count(player);
        if (combat == null || kids <= 0 || !player.Creature.IsAlive) return;
        int damage = DamagePerKid(player);
        var rng = player.RunState.Rng.CombatTargets;
        for (int i = 0; i < kids; i++)
        {
            var enemies = combat.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            var target = rng.NextItem(enemies)!;
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_blunt");
            await CreatureCmd.Damage(ctx, target, damage, ValueProp.Unpowered, player.Creature);
        }
    }
}
