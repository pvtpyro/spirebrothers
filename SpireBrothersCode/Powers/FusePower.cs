using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>The Creeper's countdown (Night Falls): turns until it explodes. Burned down by its own moves; type None so it can't be stripped.</summary>
public class FusePower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;
}
