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

/// <summary>Share. A player gains 1 Block per 10 Gold you have (max 25).</summary>
public class UntouchedSavings() : DavidCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hoard, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("MaxBlock", 25),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(CappedTens)
    ];

    // 1 per 10 Gold, capped at MaxBlock. A CalculatedBlock so the card can preview the total with Dexterity / Frail.
    private static decimal CappedTens(CardModel card, Creature? _) =>
        Math.Min(card.DynamicVars["MaxBlock"].BaseValue, (card.Owner?.Gold ?? 0) / 10);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        decimal block = DynamicVars.CalculatedBlock.Calculate(null);
        if (block > 0) await CreatureCmd.GainBlock(ShareTarget(play), block, ValueProp.Move, play);
    }

    protected override void OnUpgrade() => DynamicVars["MaxBlock"].UpgradeValueBy(10);
}
