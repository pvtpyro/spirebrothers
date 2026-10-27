using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Magnum Opus. Held Verses count 3 times for a Chorus instead of 2 (read by Verses.HeldBonus).</summary>
public class MagnumOpusPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
