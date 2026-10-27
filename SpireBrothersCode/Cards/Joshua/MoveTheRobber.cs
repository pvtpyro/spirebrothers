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

/// <summary>Catan: rolled a seven, and everyone hates the robber. Roll. Deal 8 damage (half on a 1, double on a 20). Apply 2 Insulted.</summary>
public class MoveTheRobber() : JoshuaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IInsultCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Roll];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new ExtraDamageVar(8),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(RollMultiplier),
        new PowerVar<InsultedPower>(2)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await RollAttack(ctx, play, "vfx/vfx_attack_blunt");
        if (play.Target is { IsAlive: true }) await CommonActions.Apply<InsultedPower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade() => DynamicVars.ExtraDamage.UpgradeValueBy(3);
}
