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

/// <summary>Share. A player gains 6 Block. Hoard: +1 Block per 50 Gold.</summary>
public class InterestPayment() : DavidCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hoard, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6),
        new CalculationExtraVar(1),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(GoldFifties)
    ];

    private static decimal GoldFifties(CardModel card, Creature? _) => (card.Owner?.Gold ?? 0) / 50;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        decimal block = ((CalculatedVar)DynamicVars["CalculatedBlock"]).Calculate(null);
        await CreatureCmd.GainBlock(ShareTarget(play), block, ValueProp.Move, play);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3);
}
