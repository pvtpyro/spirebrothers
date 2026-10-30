using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>A Ba'al revealed at half HP as a clone (WhichOneIsRealPower). Only a label.</summary>
public class JustAClonePower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;
}
