using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Whenever you trigger Exact, draw Amount extra cards next turn (you have no energy to play them now).</summary>
public class QedPower : BrothersPower, IExactListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnExact(PlayerChoiceContext choiceContext, Player player)
    {
        Flash();
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
