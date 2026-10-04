using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>At the start of your turn, add Amount Gears to your hand.</summary>
public class FactoryMustGrowPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null) return;
        Flash();
        for (int i = 0; i < Amount; i++)
        {
            var gear = CombatState.CreateCard<Gear>(player);
            await CardPileCmd.AddGeneratedCardToCombat(gear, PileType.Hand, player);
        }
    }
}
