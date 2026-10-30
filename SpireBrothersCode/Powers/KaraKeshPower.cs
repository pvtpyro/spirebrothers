using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On Apophis while he charges the hand device (Raise the Kara Kesh), until the Ribbon Device fires. Amount is the
/// damage left to knock him out of it: every hit that lands counts, Block or not. At 0 he's stunned, the beam is
/// cancelled, and he goes on to Jaffa, Kree! (Apophis.Interrupt).
/// </summary>
public class KaraKeshPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || Owner.IsDead || Owner.Monster is not Apophis apophis) return;
        int landed = result.BlockedDamage + result.UnblockedDamage;
        if (landed <= 0) return;
        Flash();
        if (Amount > landed)
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -landed, null, null);
            return;
        }
        await PowerCmd.Remove(this);
        await apophis.Interrupt();
    }
}
