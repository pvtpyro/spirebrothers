using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// David's Rants. Until the end of its turn, its attacks deal Amount less damage per hit, then 25% less (like Weak;
/// the game applies additive modifiers before multiplicative ones). Done as a damage modifier rather than Strength
/// so Artifact can't make it backfire.
/// </summary>
public class RantedAtPower : BrothersPower
{
    public const decimal Multiplier = 0.75m;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 0m;
        return -Amount;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 1m;
        return Multiplier;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
