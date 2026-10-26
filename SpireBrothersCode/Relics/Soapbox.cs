using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Your first Rant each turn counts as 2 Rants. The counting happens in DavidTracker.</summary>
public class Soapbox : DavidRelic
{
    public const int FirstRantCounts = 2;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Rant)];
}
