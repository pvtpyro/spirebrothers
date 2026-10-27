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

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>He calls Mom a lot to chat or vent. Rant. Draw 2 cards. Rant: gain 3 Block for each Rant played earlier this turn.</summary>
public class CallMom() : DavidCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Rant];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new CalculationBaseVar(0),
        new CalculationExtraVar(3),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(RantsSoFar)
    ];

    private static decimal RantsSoFar(CardModel card, Creature? _) => DavidTracker.RantsThisTurn(card.Owner);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (RantsSoFar(this, null) > 0) await CommonActions.CardBlock(this, play);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationExtra.UpgradeValueBy(1);
}
