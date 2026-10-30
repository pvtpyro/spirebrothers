using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>On Apophis. Daniel starts with Shol'va and gets the opening line, David answers (FoePower, ApophisBoss).</summary>
public class SystemLordPower : FoePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<SholVa>();
}
