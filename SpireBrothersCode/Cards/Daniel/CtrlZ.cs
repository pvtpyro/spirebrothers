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

/// <summary>Put a card from your discard pile into your hand. Upgrade: costs 0.</summary>
public class CtrlZ() : DanielCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var picked = await CardSelectCmd.FromCombatPile(context: ctx, pile: PileType.Discard.GetPile(Owner), player: Owner,
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1));
        var card = picked.FirstOrDefault();
        if (card != null) await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
