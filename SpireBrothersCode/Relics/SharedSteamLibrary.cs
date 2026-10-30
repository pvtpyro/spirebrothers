using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Shared (all brothers, and Brandi). At the start of each combat, add a random card from another brother's (or Brandi's) pool to your hand.
/// It costs 0 this turn. Picked on the run RNG so co-op stays in sync.
/// </summary>
public class SharedSteamLibrary : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;

        CardPoolModel[] pools =
        [
            ModelDb.CardPool<DanielCardPool>(), ModelDb.CardPool<DavidCardPool>(),
            ModelDb.CardPool<JoshuaCardPool>(), ModelDb.CardPool<TimCardPool>(), ModelDb.CardPool<BrandiCardPool>(),
        ];
        var cards = pools
            .Where(p => p != player.Character.CardPool)
            .SelectMany(p => p.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(c => (c.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare) && DuoCards.Allowed(c, player))
            .ToList();
        var card = CardFactory.GetDistinctForCombat(player, cards, 1, player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (card == null) return;
        Flash();
        card.EnergyCost.SetThisTurn(0);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
    }
}
