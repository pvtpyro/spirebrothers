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

/// <summary>Titanfall. Pilot: deal 3 to a random enemy 3 times. Titan (Predator Cannon): 5 times.</summary>
public class SmartPistol() : DannyCard(1, CardType.Attack, CardRarity.Basic, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("TitanHits", 5)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play) =>
        await Hit(ctx, play, "Damage", DynamicVars["Hits"].IntValue, "vfx/vfx_attack_slash");

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) =>
        await Hit(ctx, play, "Damage", DynamicVars["TitanHits"].IntValue);

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1);
        DynamicVars["TitanHits"].UpgradeValueBy(2);
    }
}
