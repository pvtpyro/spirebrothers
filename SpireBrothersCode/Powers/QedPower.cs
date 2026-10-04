using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Whenever you trigger Exact, draw Amount cards.</summary>
public class QedPower : BrothersPower, IExactListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnExact(PlayerChoiceContext choiceContext, Player player)
    {
        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, player);
    }
}
