using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Your potions are 50% stronger (rounded up). Grapefruit really does make some medicines hit harder.
/// The potion is already off the belt when BeforePotionUsed runs, so its numbers can be bumped without restoring them.
/// </summary>
public class Grapefruit : DanielRelic
{
    public const decimal Multiplier = 1.5m;
    public override RelicRarity Rarity => RelicRarity.Common;

    public override Task BeforePotionUsed(PotionModel potion, Creature? target)
    {
        if (potion.Owner != Owner) return Task.CompletedTask;

        foreach (var v in potion.DynamicVars.Values)
        {
            if (v.BaseValue > 0) v.BaseValue = Math.Ceiling(v.BaseValue * Multiplier);
        }
        Flash();
        return Task.CompletedTask;
    }
}
