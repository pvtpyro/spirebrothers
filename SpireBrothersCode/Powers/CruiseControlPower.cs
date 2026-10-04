using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Road trip. Retain your hand at the end of each turn (vanilla RetainHand, but it doesn't wear off).</summary>
public class CruiseControlPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldFlush(Player player) => player != Owner.Player;
}
