using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// John's Hive Scum chems. Toxin is a debuff that does nothing on its own and never wears off; his Chem cards
/// react with it. Every Toxin John applies goes through Apply so Cartel Kingpin can add to it.
/// </summary>
public static class Toxin
{
    public static int On(Creature? creature) => creature?.GetPowerAmount<ToxinPower>() ?? 0;

    public static async Task Apply(PlayerChoiceContext ctx, IEnumerable<Creature> targets, int amount, Player by, CardModel? card)
    {
        var alive = targets.Where(t => t.IsAlive).ToList();
        if (alive.Count == 0 || amount <= 0) return;
        amount += by.Creature.GetPowerAmount<CartelKingpinPower>();
        RunStats.Add(RunStats.ToxinApplied, by, amount * alive.Count);
        await PowerCmd.Apply<ToxinPower>(ctx, alive, amount, by.Creature, card);
    }

    public static Task Apply(PlayerChoiceContext ctx, Creature target, int amount, Player by, CardModel? card) =>
        Apply(ctx, [target], amount, by, card);

    /// <summary>Removes all Toxin from the creature and says how much there was.</summary>
    public static async Task<int> Clear(Creature creature)
    {
        var power = creature.GetPower<ToxinPower>();
        if (power == null) return 0;
        int amount = power.Amount;
        await PowerCmd.Remove(power);
        return amount;
    }
}
