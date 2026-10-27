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
using SpireBrothers.SpireBrothersCode.Orbs;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Ballads. Song. Apply 3 Heartstrings.</summary>
public class TugAtTheHeartstrings() : JoshuaCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Song];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<HeartstringsPower>(3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HeartstringsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is { IsAlive: true }) await CommonActions.Apply<HeartstringsPower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade() => DynamicVars["HeartstringsPower"].UpgradeValueBy(2);
}
