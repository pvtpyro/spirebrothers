using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Windows Update's progress. Each step is 25% (the icon shows the percent); at 100% the laptop's next move is Restart,
/// which clears this. Type None so it can't be stripped.
/// </summary>
public class UpdatingPower : BrothersPower
{
    public const int Steps = 4;

    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount * 100 / Steps;
}
