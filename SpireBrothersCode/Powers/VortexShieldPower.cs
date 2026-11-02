using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Vortex Shield (Amped Wall's Titan side). The next Amount attacks on Danny before his next turn are caught: he gains
/// exactly that much Block for the hit (not from a card, so it works in the Titan), and the same damage goes back at the
/// attacker, the way vanilla Thorns hits back. Gone at the start of his next turn.
/// </summary>
public class VortexShieldPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || dealer.Side == Owner.Side || amount <= 0 || !props.IsPoweredAttack()) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
        await CreatureCmd.Damage(choiceContext, dealer, amount, ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner, null);
        if (Amount <= 1) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner) await PowerCmd.Remove(this);
    }
}
