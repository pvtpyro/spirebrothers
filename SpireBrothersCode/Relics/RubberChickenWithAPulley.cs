using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Monkey Island: at the start of each combat, add a Snappy Comeback to your hand.</summary>
public class RubberChickenWithAPulley : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        var card = combat.CreateCard<SnappyComeback>(player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
    }
}
