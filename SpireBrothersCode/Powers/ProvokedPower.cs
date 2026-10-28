using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Enderman's temper. The number is how many times the party has hit him this turn; on the third, he's provoked and
/// his next move becomes Ender Rage (Enderman.Provoke). Resets at the start of the party's turn. Type None so it can't be
/// stripped.
/// </summary>
public class ProvokedPower : BrothersPower
{
    public const int HitsToProvoke = 3;

    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _hits;

    public override int DisplayAmount => _hits;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player && _hits != 0)
        {
            _hits = 0;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer is not { IsPlayer: true } || !Owner.IsAlive) return Task.CompletedTask;
        _hits++;
        InvokeDisplayAmountChanged();
        if (_hits == HitsToProvoke && Owner.Monster is Enderman enderman)
        {
            Flash();
            enderman.Provoke();
        }
        return Task.CompletedTask;
    }
}
