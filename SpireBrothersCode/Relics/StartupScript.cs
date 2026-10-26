using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>AutoCAD / Lua. Each combat, the first 2 non-Power cards you play gain Script. Read in TimTracker.</summary>
public class StartupScript : TimRelic
{
    public const int Cards = 2;
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Script)];

    // Combat-only, so it isn't saved.
    private int _used;

    public override Task BeforeCombatStart()
    {
        _used = 0;
        return Task.CompletedTask;
    }

    /// <summary>True for the first 2 non-Power cards asked about each combat.</summary>
    public bool TryUse()
    {
        if (_used >= Cards) return false;
        _used++;
        Flash();
        return true;
    }
}
