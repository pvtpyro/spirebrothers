using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Factories;

namespace SpireBrothers.SpireBrothersCode.Cards.Brandi;

/// <summary>Travel. Choose 1 of 3 random cards from other characters' pools. It costs 0 this turn. Exhaust. Upgrade: costs 0.</summary>
public class BeenEverywhere() : BrandiCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public const int Choices = 3;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        CardPoolModel[] pools =
        [
            ModelDb.CardPool<Character.DanielCardPool>(), ModelDb.CardPool<Character.DavidCardPool>(),
            ModelDb.CardPool<Character.JoshuaCardPool>(), ModelDb.CardPool<Character.TimCardPool>(),
            ModelDb.CardPool<Character.JohnCardPool>(), ModelDb.CardPool<Character.HuffmanCardPool>(),
        ];
        var cards = pools
            .Where(p => p != Owner.Character.CardPool)
            .SelectMany(p => p.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint))
            .Where(c => c.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare && DuoCards.Allowed(c, Owner))
            .ToList();
        var offered = CardFactory.GetDistinctForCombat(Owner, cards, Choices, Owner.RunState.Rng.CombatCardGeneration).ToList();
        if (offered.Count == 0) return;
        var card = await CardSelectCmd.FromChooseACardScreen(ctx, offered, Owner);
        if (card == null) return;
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
