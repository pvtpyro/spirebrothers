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

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>Systems. Discard any number of cards, then draw that many. Upgrade: costs 0.</summary>
public class Respec() : JohnCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int inHand = PileType.Hand.GetPile(Owner).Cards.Count;
        if (inHand == 0) return;
        var picked = (await CardSelectCmd.FromHandForDiscard(ctx, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, inHand), null, this)).ToList();
        if (picked.Count == 0) return;
        await CardCmd.DiscardAndDraw(ctx, picked, picked.Count);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
