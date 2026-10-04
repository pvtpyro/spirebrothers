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

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>AutoCAD (external reference). Choose a card in your hand. It gains Script this combat. Upgrade: costs 0.</summary>
public class Xref() : TimCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Script)];

    private static bool CanScript(CardModel c) => c.Type != CardType.Power && !c.Keywords.Contains(BrotherKeywords.Script);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var picked = await CardSelectCmd.FromHand(ctx, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), CanScript, this);
        var card = picked.FirstOrDefault();
        if (card != null) CardCmd.ApplyKeyword(card, BrotherKeywords.Script);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
