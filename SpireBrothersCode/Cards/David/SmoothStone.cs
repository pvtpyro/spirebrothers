using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// Token: David starts the Goliath fight with one in hand (ChampionOfGathPower). Deal 5 damage, or 25 to Goliath.
/// Retain, Exhaust.
/// </summary>
public class SmoothStone() : DavidCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new DynamicVar("GiantDamage", 25)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target == null) return;
        decimal damage = play.Target.Monster is Goliath ? DynamicVars["GiantDamage"].BaseValue : DynamicVars.Damage.BaseValue;
        await DamageCmd.Attack(damage).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["GiantDamage"].UpgradeValueBy(5);
    }
}
