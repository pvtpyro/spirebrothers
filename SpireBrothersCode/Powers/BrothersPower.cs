using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Base for all mod powers. Icons: images/powers/<power_id>.png (and big/).</summary>
public abstract class BrothersPower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    public abstract override PowerType Type { get; }
    public abstract override PowerStackType StackType { get; }
}
