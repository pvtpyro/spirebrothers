using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Bridge. Your next Chorus doesn't spend Verses (one use per stack).</summary>
public class BridgePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Called by Verses.SpendForChorus when a Chorus uses up one Bridge.</summary>
    public async Task Use()
    {
        Flash();
        await PowerCmd.Decrement(this);
    }
}
