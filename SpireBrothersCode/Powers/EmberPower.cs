using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Brandi, Private Pyro. At the start of its turn (like Poison) the creature takes damage equal to its Ember (ignores
/// Block), then the Ember grows by 1, up to Ember.GrowthCap: fires spread. On a player it goes down by 1 instead.
/// </summary>
public class EmberPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // The Ember it had when its side's turn began. Only that much burns: Ember that jumps over from an enemy that just
    // burned to death waits for the next turn (it used to burn right away, so a 14 on the back enemy hit for 42).
    private int _burnsThisTurn;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner)) _burnsThisTurn = Amount;
        return Task.CompletedTask;
    }

    // AfterSideTurnStart, not BeforeSideTurnStart, for the same reason as Bleed: Block is cleared in between, and an
    // enemy killed before that has already left combat.
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        var burn = Math.Min(Amount, _burnsThisTurn);
        _burnsThisTurn = 0;
        if (!Owner.IsAlive || burn <= 0) return;
        var choiceContext = new ThrowingPlayerChoiceContext();
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, burn, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (!Owner.IsAlive) return;

        if (Owner.IsPlayer)
        {
            if (Amount <= 1) await PowerCmd.Remove(this);
            else await PowerCmd.Decrement(this);
        }
        else if (Amount < Ember.GrowthCap)
            await PowerCmd.ModifyAmount(choiceContext, this, 1, null, null);
    }
}