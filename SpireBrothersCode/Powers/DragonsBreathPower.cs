using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Ender Dragon's lingering purple cloud, on players. Like Poison: at the start of the owner's turn they lose HP equal
/// to it (ignores Block), then it goes down by 1. Same timing as BleedPower (AfterSideTurnStart; see the note there).
/// </summary>
public class DragonsBreathPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || !Owner.IsAlive || Amount <= 0) return;
        var ctx = new ThrowingPlayerChoiceContext();
        Flash();
        await CreatureCmd.Damage(ctx, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (!Owner.IsAlive) return;
        if (Amount <= 1) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(ctx, this, -1, null, null);
    }
}
