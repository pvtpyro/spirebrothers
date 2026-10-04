using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Satisfactory. If you end your turn with exactly 0 energy, gain Amount energy and draw Amount extra cards next turn.</summary>
public class PerfectEfficiencyPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player?.PlayerCombatState?.Energy != 0) return;
        Flash();
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
