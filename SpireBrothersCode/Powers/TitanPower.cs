using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Danny is in his Titan (Mechanics/Titan): his cards use their Titan side, damage meant for him hits the Titan first, and
/// his own cards give him no Block. Type None so nothing strips it; removed when the Titan is destroyed.
/// </summary>
public class TitanPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;
}
