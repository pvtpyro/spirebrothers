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

/// <summary>
/// David worked at McDonald's for years, and someone always ordered a few dozen cookies when you can only bake a
/// few at a time. At the start of your turn, bake 6 Cookies into your hand until all 36 are done (6 turns).
/// </summary>
public class ThreeDozenCookies() : DavidCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ThreeDozenCookiesPower>(ThreeDozenCookiesPower.OrderSize),
        new DynamicVar("Batch", ThreeDozenCookiesPower.BatchSize)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Cookie>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.ApplySelf<ThreeDozenCookiesPower>(ctx, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
