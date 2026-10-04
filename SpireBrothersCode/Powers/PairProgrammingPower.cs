using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>At the start of your turn, draw Amount extra cards. Can be given to a teammate via Share.</summary>
public class PairProgrammingPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, player);
    }
}
