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

/// <summary>Family (school pickup). Deal 6 damage. If you have 4+ Kids, deal 4 more.</summary>
public class CarpoolLine() : TimCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override int KidsThreshold => 4;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Kids];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6),
        new ExtraDamageVar(4),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(EnoughKids)
    ];

    private static decimal EnoughKids(CardModel card, Creature? _) => Kids.Count(card.Owner) >= 4 ? 1 : 0;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
}
