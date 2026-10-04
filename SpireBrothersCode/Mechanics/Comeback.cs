using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using SpireBrothers.SpireBrothersCode.Powers;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Shared Monkey Island rules so every brother's Comeback cards behave the same:
/// Monkey Wrench adds to the per-Insulted bonus, Monkey Business stops Insulted from being cleared.
/// </summary>
public static class Comeback
{
    public const decimal WrenchBonus = 2;

    public static decimal Stacks(Creature? target) => target?.GetPowerAmount<InsultedPower>() ?? 0;

    /// <summary>Extra amount per Insulted stack from relics (Monkey Wrench).</summary>
    public static decimal PerInsultBonus(Player? owner) => owner?.GetRelic<MonkeyWrench>() != null ? WrenchBonus : 0;

    /// <summary>Removes the target's Insulted after a Comeback, unless Monkey Business is active this turn.</summary>
    public static async Task ClearInsulted(Player owner, Creature? target)
    {
        if (target == null || !target.IsAlive) return;
        if (owner.Creature.GetPower<MonkeyBusinessPower>() != null) return;
        var insult = target.GetPower<InsultedPower>();
        if (insult != null) await PowerCmd.Remove(insult);
    }
}
