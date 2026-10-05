using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Draw Amount fewer cards at the start of your next turn, then this goes away.</summary>
public class DrawLessNextTurnPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _applied;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player.Creature != Owner) return count;
        _applied = true;
        return Math.Max(0, count - Amount);
    }

    // Removed at the end of the turn it shrank, so the next turn's opening draw is the one affected.
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (_applied && participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
