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

/// <summary>D&amp;D. Roll. Deal 2 damage per point rolled. On a Natural 20, deal 50 instead.</summary>
public class NaturalTwenty() : JoshuaCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Roll];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("PerPoint", 2), new DamageVar(50, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { IsAlive: true } target) return;
        int roll = await Dice.Roll(ctx, Owner);
        decimal damage = roll >= Dice.Sides ? DynamicVars.Damage.BaseValue : roll * DynamicVars["PerPoint"].BaseValue;
        await DamageCmd.Attack(damage).FromCard(this).Targeting(target).WithHitFx("vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PerPoint"].UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}
