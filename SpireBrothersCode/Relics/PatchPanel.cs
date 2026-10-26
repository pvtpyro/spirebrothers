using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// He wired the whole house. The first Logic or Hands card you play each turn also counts as the other
/// (for Wired, Stratagem inputs, and the starter relic). Recorded in TurnTracker.
/// </summary>
public class PatchPanel : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(BrotherKeywords.Logic), HoverTipFactory.FromKeyword(BrotherKeywords.Hands)];
}
