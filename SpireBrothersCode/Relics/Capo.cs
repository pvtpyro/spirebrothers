using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Chorus cards act as if 1 more Verse was spent. Added in Verses.ForChorus.</summary>
public class Capo : JoshuaRelic
{
    public const int ExtraVerses = 1;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Chorus)];
}
