using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Haunting Melody. Heartstrings on enemies no longer goes down (read by HeartstringsPower).</summary>
public class HauntingMelodyPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
