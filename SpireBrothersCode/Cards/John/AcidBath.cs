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

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>Hive Scum. Deal 6 damage. If the enemy has 5+ Toxin, apply 2 Vulnerable.</summary>
public class AcidBath() : JohnCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public const int ToxinNeeded = 5;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new PowerVar<VulnerablePower>(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ToxinPower>(), HoverTipFactory.FromPower<VulnerablePower>()];
    protected override bool ExtraGlow => CombatState?.HittableEnemies.Any(e => Toxin.On(e) >= ToxinNeeded) ?? false;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool soaked = Toxin.On(play.Target) >= ToxinNeeded;
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_slash").Execute(ctx);
        if (soaked && play.Target is { IsAlive: true }) await CommonActions.Apply<VulnerablePower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}
