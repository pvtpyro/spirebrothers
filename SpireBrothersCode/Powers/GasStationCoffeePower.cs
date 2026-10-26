using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>From Gas Station Coffee. This turn, Exact also triggers with 1 energy left (read by Mechanics.Exact).</summary>
public class GasStationCoffeePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
