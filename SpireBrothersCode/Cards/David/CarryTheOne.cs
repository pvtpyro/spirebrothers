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

/// <summary>Gain 7 Block. Exact: gain 4 more.</summary>
public class CarryTheOne() : DavidCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Exact];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7, ValueProp.Move), new DynamicVar("ExactBlock", 4)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int exact = await Exact.Check(ctx, this);
        await CommonActions.CardBlock(this, play);
        for (int i = 0; i < exact; i++) await CommonActions.CardBlock(this, DynamicVars["ExactBlock"], play);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}
