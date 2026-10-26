using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Archipelago. Your first Check each combat finds 2 items. Read in Archipelago.Check.</summary>
public class SpoilerLog : JoshuaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Check)];

    // Combat-only, so it isn't saved.
    private bool _used;

    public override Task BeforeCombatStart()
    {
        _used = false;
        return Task.CompletedTask;
    }

    /// <summary>True the first time it's asked each combat.</summary>
    public bool TryUse()
    {
        if (_used) return false;
        _used = true;
        Flash();
        return true;
    }
}
