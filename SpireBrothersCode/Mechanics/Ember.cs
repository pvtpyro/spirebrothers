using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Brandi, Private Pyro. Ember (EmberPower) burns at the start of its owner's turn and then grows by 1, up to
/// <see cref="GrowthCap"/>; fires spread, where Poison fades. On a player it fades by 1 instead. When an enemy with Ember
/// dies, its Ember jumps to another random enemy (BrandiTracker). Every Ember she applies goes through Apply, so the
/// Zippo, Eternal Flame, and Private Pyro can add to it.
/// </summary>
public static class Ember
{
    /// <summary>Ember stops growing on its own here. Cards can still push it higher. Was 10 until 2026-10-29 (balance
    /// pass): it never fades on enemies, so long fights ran away.</summary>
    public const int GrowthCap = 5;

    /// <summary>Total Ember on the enemies that gets heart eyes out of her.</summary>
    public const int FireMoment = 15;

    public static int On(Creature? creature) => creature?.GetPowerAmount<EmberPower>() ?? 0;

    /// <summary>Total Ember on every living enemy.</summary>
    public static int Total(ICombatState? combat) =>
        combat?.HittableEnemies.Where(e => e.IsAlive).Sum(On) ?? 0;

    public static async Task Apply(PlayerChoiceContext ctx, IEnumerable<Creature> targets, int amount, Player by, CardModel? card)
    {
        var alive = targets.Where(t => t.IsAlive).ToList();
        if (alive.Count == 0 || amount <= 0) return;
        amount += by.Creature.GetPowerAmount<PrivatePyroPower>();
        if (by.GetRelic<EngravedZippo>() is { } zippo) amount = zippo.Boost(amount, by);
        Sounds.Play(Sounds.BrandiEmber);
        await PowerCmd.Apply<EmberPower>(ctx, alive, amount, by.Creature, card);
        RunStats.Add(RunStats.EmberApplied, by, amount * alive.Count);
        // Something big caught fire: heart eyes (once per combat).
        if (Total(by.Creature.CombatState) >= FireMoment) Banter.Moment(by, "FIRE");
    }

    public static Task Apply(PlayerChoiceContext ctx, Creature target, int amount, Player by, CardModel? card) =>
        Apply(ctx, [target], amount, by, card);

    /// <summary>All living enemies (for the ALL-enemies cards).</summary>
    public static List<Creature> Enemies(ICombatState? combat) =>
        combat?.HittableEnemies.Where(e => e.IsAlive).ToList() ?? [];
}
