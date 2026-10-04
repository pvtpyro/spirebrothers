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

/// <summary>Tim's Kids (max 8). At the end of your turn, each Kid deals 1 damage to a random enemy. See Mechanics/Kids.cs.</summary>
public class KidsPower : BrothersPower
{
    // Type None so enemy buff-stripping can't take the kids.
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player == null) return;
        if (Owner.GetPower<DateNightPower>() != null) return; // they're at Grandma's
        Flash();
        await Kids.Act(choiceContext, Owner.Player);
    }
}
