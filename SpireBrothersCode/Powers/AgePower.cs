using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Tim's Age tracker icon. The number is the current Age (1 Dark, 2 Feudal, 3 Castle, 4 Imperial);
/// hovering names it. Holds no state of its own, it just reads TimTracker.
/// </summary>
public class AgePower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Ages.Get(Owner?.Player) + 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [new HoverTip(new LocString("powers", Id.Entry + ".current"), Ages.Name(Ages.Get(Owner?.Player)))];

    /// <summary>Ages.AgeUp calls this so the icon number updates.</summary>
    public void Refresh()
    {
        Flash();
        InvokeDisplayAmountChanged();
    }
}
