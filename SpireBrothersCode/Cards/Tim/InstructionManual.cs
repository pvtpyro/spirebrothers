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

/// <summary>Legos. Choose a card from your draw pile and put it into your hand. Upgrade: costs 0.</summary>
public class InstructionManual() : TimCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var draw = PileType.Draw.GetPile(Owner);
        if (!draw.Cards.Any()) return;
        var picked = await CardSelectCmd.FromCombatPile(ctx, draw, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1));
        var card = picked.FirstOrDefault();
        if (card != null) await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
