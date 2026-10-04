using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Monkey Island: does nothing alone. Comeback cards deal bonus damage per stack, then clear it.</summary>
public class InsultedPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
