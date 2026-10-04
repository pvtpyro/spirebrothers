using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Diligent cards grow (1 + Amount) times as much. Checked in DanielCard.GrowDiligent.</summary>
public class TenThousandHoursPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
