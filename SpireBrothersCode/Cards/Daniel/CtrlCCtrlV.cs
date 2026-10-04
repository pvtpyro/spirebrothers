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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Add a copy of a card in your hand to your hand. Exhaust. Upgrade: no longer Exhausts.</summary>
public class CtrlCCtrlV() : DanielCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var picked = await CardSelectCmd.FromHand(ctx, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        var card = picked.FirstOrDefault();
        if (card != null) await CardPileCmd.AddGeneratedCardToCombat(card.CreateClone(), PileType.Hand, Owner);
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}
