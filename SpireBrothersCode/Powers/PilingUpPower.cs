using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Chores' power: if nobody in the party hit this chore during their turn, it gains Amount Strength as the enemies'
/// turn starts. The icon shows the Strength it's about to gain and drops to 0 (with a flash) once anyone, or anyone's
/// pet, has hit it this turn, so co-op players can see which chores are done. Type None so it can't be stripped.
/// </summary>
public class PilingUpPower : BrothersPower
{
    public const int StrengthPerTurn = 2;

    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _hitThisTurn;

    public override int DisplayAmount => _hitThisTurn ? 0 : Amount;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || _hitThisTurn || dealer == null || !(dealer.IsPlayer || dealer.PetOwner != null)) return Task.CompletedTask;
        _hitThisTurn = true;
        Flash();
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _hitThisTurn = false;
            InvokeDisplayAmountChanged();
            return;
        }
        if (_hitThisTurn || !Owner.IsAlive) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
