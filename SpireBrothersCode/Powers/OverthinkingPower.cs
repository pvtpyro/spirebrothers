using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>David's Overthinking. His Rant cards count Amount more Rants played earlier this turn (read in DavidTracker.RantsThisTurn).</summary>
public class OverthinkingPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
