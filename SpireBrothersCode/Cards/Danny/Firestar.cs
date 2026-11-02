using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Danny;

/// <summary>Titanfall. Pilot: deal 5, apply 2 Poison. Titan (Thermite): deal 4 to ALL enemies and apply 2 Poison to ALL.</summary>
public class Firestar() : DannyCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move), new PowerVar<PoisonPower>(2),
        new DamageVar("TitanDamage", 4, ValueProp.Move), new PowerVar<PoisonPower>("TitanPoison", 2)
    ];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, "Damage", vfx: "vfx/vfx_fire_burst");
        if (play.Target is { IsAlive: true } target) await Apply<PoisonPower>(ctx, target, DynamicVars["PoisonPower"].BaseValue);
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, "TitanDamage", vfx: "vfx/vfx_fire_burst");
        await ApplyAll<PoisonPower>(ctx, DynamicVars["TitanPoison"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["PoisonPower"].UpgradeValueBy(1);
        DynamicVars["TitanDamage"].UpgradeValueBy(1);
        DynamicVars["TitanPoison"].UpgradeValueBy(1);
    }
}
