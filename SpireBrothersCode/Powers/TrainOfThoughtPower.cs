using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Daniel's tracker icon. The number is how many Logic/Hands cards he's played this turn;
/// hovering lists them in order. Holds no state of its own, it just reads TurnTracker.
/// </summary>
public class TrainOfThoughtPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount
    {
        get
        {
            var state = Owner?.Player?.PlayerCombatState;
            return state == null ? 0 : TurnTracker.Sequence.Get(state)?.Count ?? 0;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [new HoverTip(new LocString("powers", Id.Entry + ".thisTurn"), DescribeSequence())];

    /// <summary>TurnTracker calls this whenever the sequence changes so the icon number updates.</summary>
    public void Refresh() => InvokeDisplayAmountChanged();

    private string DescribeSequence()
    {
        var state = Owner?.Player?.PlayerCombatState;
        var seq = state == null ? null : TurnTracker.Sequence.Get(state);
        if (seq == null || seq.Count == 0)
            return new LocString("powers", Id.Entry + ".nothingYet").GetFormattedText();
        return string.Join(", ", seq.Select(TurnTracker.KeywordName));
    }
}
