using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>The real Ba'al, revealed at half HP (WhichOneIsRealPower, which also gives him Strength). Only a label.</summary>
public class RealBaalPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;
}
