using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>On the Lion. Daniel starts with Shut Their Mouths and gets the opening line (FoePower, LionsDenElite).</summary>
public class DenOfLionsPower : FoePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<ShutTheirMouths>();
}
