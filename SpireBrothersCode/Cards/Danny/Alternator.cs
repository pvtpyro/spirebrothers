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

/// <summary>Titanfall. Pilot: deal 4 twice. Titan (40mm Tracker): deal 6 twice.</summary>
public class Alternator() : DannyCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new DamageVar("TitanDamage", 6, ValueProp.Move)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "Damage", 2, "vfx/vfx_attack_slash");

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "TitanDamage", 2);

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["TitanDamage"].UpgradeValueBy(2);
    }
}
