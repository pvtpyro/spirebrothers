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

/// <summary>Enshrouded. While John is in the Shroud, deal Amount damage to ALL enemies at the end of his turn (done in ShroudPower).</summary>
public class ShroudWalkerPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task Burn(PlayerChoiceContext ctx)
    {
        if (Owner.CombatState is not { } combat) return;
        Flash();
        await CreatureCmd.Damage(ctx, combat.HittableEnemies, Amount, ValueProp.Unpowered, Owner, null);
    }
}
