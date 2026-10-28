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
/// Enshrouded. John is in the Shroud for Amount more turns (this one included): his Attacks deal 50% more damage, and
/// he loses 1 HP at the end of each of his turns. Counts down at the end of his turn; at 0 he leaves.
/// </summary>
public class ShroudPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 1m;
        return Shroud.DamageMultiplier;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player is not { } player) return;
        RunStats.Add(RunStats.ShroudTurns, player);
        // Shroud Walker: the fog itself hurts his enemies.
        if (Owner.GetPower<ShroudWalkerPower>() is { } walker) await walker.Burn(choiceContext);
        int loss = Shroud.HpLoss(player);
        if (loss > 0)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, Owner, loss, ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
        }
        if (Amount <= 1) await Shroud.Leave(choiceContext, player);
        else await PowerCmd.Decrement(this);
    }
}
