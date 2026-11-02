using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Warpfall (pilot side). Whenever he calls in his Titan, draw Amount cards (called from Titan.Call).</summary>
public class WarpfallPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterTitanfall(PlayerChoiceContext ctx)
    {
        if (Owner.Player is not { } player) return;
        Flash();
        await CardPileCmd.Draw(ctx, Amount, player);
    }
}
