using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// David. At the start of its turn (like Poison), the creature loses HP equal to Bleed (ignores Block), then Bleed halves.
/// With Deep Cuts on any player, Bleed goes down by 1 instead.
/// </summary>
public class BleedPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Like Poison: at the start of the creature's turn, so it can die before it acts. Must be AfterSideTurnStart, not
    // BeforeSideTurnStart: the game clears Block on every creature in between, and an enemy killed before that has
    // already been removed from combat (CombatState null), which threw and froze the turn when other enemies were left.
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || !Owner.IsAlive || Amount <= 0) return;
        var choiceContext = new ThrowingPlayerChoiceContext();
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (!Owner.IsAlive) return;

        bool deepCuts = CombatState?.Players.Any(p => p.Creature.GetPower<DeepCutsPower>() != null) ?? false;
        int next = deepCuts ? Amount - 1 : Amount / 2;
        if (next <= 0) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(choiceContext, this, next - Amount, null, null);
    }
}
