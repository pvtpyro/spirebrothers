using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// John's Stimm Lab talents, shown as an icon on him in combat (given on turn 1 by JohnTracker if he has any). The
/// number is his total ranks; hovering lists each talent with its rank. Display only: the talents act on their own.
/// Type None so buff-stripping can't remove it.
/// </summary>
public class TalentsPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => StimmLab.TotalRanks(Owner?.Player);

    // One box listing them all, like Family Bonds (several boxes on one creature has crashed hovers in co-op).
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var lines = StimmLab.LearnedLines(Owner?.Player);
            if (lines.Count == 0) return [];
            return [new HoverTip(new LocString("powers", Id.Entry + ".learned"), string.Join("\n", lines))];
        }
    }
}
