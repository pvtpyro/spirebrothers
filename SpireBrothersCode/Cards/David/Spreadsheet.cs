using BaseLib.Cards.Variables;
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

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>Draw 2 cards. Exact: draw them next turn instead (you are out of energy), plus 1 extra.</summary>
public class Spreadsheet() : DavidCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Exact];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("ExactCards", 1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int exact = await Exact.Check(ctx, this);
        // Exact means you are out of energy, so the whole draw waits until next turn when you can use it.
        if (exact == 0) await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        else await DrawNextTurn(ctx, DynamicVars.Cards.BaseValue + exact * DynamicVars["ExactCards"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
