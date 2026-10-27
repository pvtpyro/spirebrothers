using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.Tim;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>On Alexander. Tim starts with Sound Doctrine and gets the opening line (FoePower, CoppersmithElite).</summary>
public class CoppersmithPower : FoePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<SoundDoctrine>();
}
