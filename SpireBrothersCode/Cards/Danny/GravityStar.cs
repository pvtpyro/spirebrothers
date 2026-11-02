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

/// <summary>Titanfall. Pilot: apply 1 Weak and 1 Vulnerable to ALL enemies. Titan (Arc Wave): deal 7 to ALL enemies, apply 1 Weak to ALL.</summary>
public class GravityStar() : DannyCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>(1), new PowerVar<VulnerablePower>(1), new DamageVar("TitanDamage", 7, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        await ApplyAll<WeakPower>(ctx, DynamicVars["WeakPower"].BaseValue);
        await ApplyAll<VulnerablePower>(ctx, DynamicVars["VulnerablePower"].BaseValue);
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await Hit(ctx, play, "TitanDamage", vfx: "vfx/vfx_attack_lightning");
        await ApplyAll<WeakPower>(ctx, DynamicVars["WeakPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
        DynamicVars["TitanDamage"].UpgradeValueBy(3);
    }
}
