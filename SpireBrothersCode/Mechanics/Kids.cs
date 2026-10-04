using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim's Kids: a counter (max 8, of course). At the end of his turn each Kid deals 1 damage to a random enemy
/// (2 with Proud Dad). Targets use the run RNG so co-op stays in sync.
/// </summary>
public static class Kids
{
    public const int Max = 8;

    public static int Count(Player? player) => player?.Creature?.GetPowerAmount<KidsPower>() ?? 0;

    public static int DamagePerKid(Player? player) => 1 + (player?.Creature?.GetPowerAmount<ProudDadPower>() ?? 0);

    /// <summary>Adds Kids up to the max of 8.</summary>
    public static async Task Gain(PlayerChoiceContext ctx, Player player, int amount, CardModel? source)
    {
        int add = Math.Min(amount, Max - Count(player));
        if (add <= 0 || !player.Creature.IsAlive) return;
        await PowerCmd.Apply<KidsPower>(ctx, player.Creature, add, player.Creature, source);
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
            await CreatureCmd.Damage(ctx, rng.NextItem(enemies)!, damage, ValueProp.Unpowered, player.Creature);
        }
    }
}
