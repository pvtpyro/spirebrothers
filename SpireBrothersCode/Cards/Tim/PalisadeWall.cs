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

/// <summary>Age of Empires. Gain 7 Block. Castle Age or later: gain 3 more.</summary>
public class PalisadeWall() : TimCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override int? AgeBonusAt => Ages.Castle;
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Age];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(7),
        new CalculationExtraVar(3),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(CastleOrLater)
    ];

    private static decimal CastleOrLater(CardModel card, Creature? _) => Ages.Get(card.Owner) >= Ages.Castle ? 1 : 0;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3);
}
