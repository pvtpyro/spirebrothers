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

/// <summary>Titanfall. Pilot (Headshot): deal 30. Titan (Laser Core): deal 22 to ALL enemies.</summary>
public class LaserCore() : DannyCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30, ValueProp.Move), new DamageVar("TitanDamage", 22, ValueProp.Move)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "Damage", vfx: "vfx/vfx_dramatic_stab");

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "TitanDamage", vfx: "vfx/vfx_attack_lightning");

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
        DynamicVars["TitanDamage"].UpgradeValueBy(6);
    }
}
