using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Sustain Pedal. At the end of your turn, gain Amount Block for each Verse you have.</summary>
public class SustainPedalPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || !Owner.IsAlive) return;
        int verses = Verses.Count(Owner.Player);
        if (verses <= 0) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount * verses, ValueProp.Unpowered, null);
    }
}
