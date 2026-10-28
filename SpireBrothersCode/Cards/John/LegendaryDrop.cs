using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>Loot. Choose 1 of 3 random Rare cards from your pool and add it to your hand. It costs 0 this turn. Exhaust.</summary>
public class LegendaryDrop() : JohnCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public const int Choices = 3;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var rares = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity == CardRarity.Rare && c is not LegendaryDrop && DuoCards.Allowed(c, Owner))
            .ToList();
        var offered = CardFactory.GetDistinctForCombat(Owner, rares, Choices, Owner.RunState.Rng.CombatCardGeneration).ToList();
        if (offered.Count == 0) return;
        var card = await CardSelectCmd.FromChooseACardScreen(ctx, offered, Owner);
        if (card == null) return;
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
