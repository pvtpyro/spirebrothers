using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// David. At the end of its turn, the creature loses HP equal to Bleed (ignores Block), then Bleed halves.
/// With Deep Cuts on any player, Bleed goes down by 1 instead.
/// </summary>
public class BleedPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Amount <= 0) return;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (!Owner.IsAlive) return;

        bool deepCuts = CombatState?.Players.Any(p => p.Creature.GetPower<DeepCutsPower>() != null) ?? false;
        int next = deepCuts ? Amount - 1 : Amount / 2;
        if (next <= 0) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(choiceContext, this, next - Amount, null, null);
    }
}
