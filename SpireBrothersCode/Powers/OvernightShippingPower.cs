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
/// Tim's Overnight Shipping. At the start of your turn, Amount times: if you have 10 Gold, spend it (through TimGold,
/// so Burning a Hole reacts) to gain 1 energy and draw 1 card.
/// </summary>
public class OvernightShippingPower : BrothersPower
{
    public const int GoldCost = 10;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        for (int i = 0; i < Amount && player.Gold >= GoldCost; i++)
        {
            Flash();
            await TimGold.Spend(choiceContext, player, GoldCost);
            await PlayerCmd.GainEnergy(1, player);
            await CardPileCmd.Draw(choiceContext, 1, player);
        }
    }
}
