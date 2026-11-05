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
/// Enshrouded. John is in the Shroud at depth Amount: his Attacks deal 50% more damage. At the end of his turn he loses 1
/// HP, then the depth grows by 1 (up to Shroud.MaxDepth). He stays until a card takes him out (Mechanics/Shroud).
/// </summary>
public class ShroudPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 1m;
        return Shroud.Multiplier(Owner.Player);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player is not { } player) return;
        RunStats.Add(RunStats.ShroudTurns, player);
        // Shroud Walker: the fog itself hurts his enemies.
        if (Owner.GetPower<ShroudWalkerPower>() is { } walker) await walker.Burn(choiceContext);
        int loss = Shroud.HpLossPerTurn;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, loss, ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
        if (!Owner.IsAlive) return;
        // Flame Altar Upgrade: cover that grows the deeper he goes.
        if (Owner.GetPower<FlameAltarUpgradePower>() is { } altar) await altar.OnShroudTurnEnd(Amount);
        if (Amount < Shroud.MaxDepth) await PowerCmd.ModifyAmount(choiceContext, this, 1, Owner, null);
    }
}
