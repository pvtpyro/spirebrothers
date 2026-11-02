using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Nuclear Ejection (Titan side). When his Titan is destroyed, deal Amount damage to ALL enemies (called from Titan.AfterDestroyed).</summary>
public class NuclearEjectionPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task Detonate(PlayerChoiceContext ctx)
    {
        if (Owner.Player is not { } player) return;
        Flash();
        await Titan.HitAll(ctx, player, Amount);
    }
}
