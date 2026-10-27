using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Tim's Repeat Last Command (AutoCAD). His Script effects happen Amount more times (read in ScriptPower).</summary>
public class RepeatLastCommandPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
