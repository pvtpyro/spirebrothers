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

/// <summary>Titanfall. Pilot: next turn, deal 16 to ALL enemies. Titan (Rocket Salvo): deal 3 to a random enemy 6 times.</summary>
public class SatchelCharge() : DannyCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Satchel", 16), new DamageVar("TitanDamage", 3, ValueProp.Move), new DynamicVar("TitanHits", 6)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<SatchelChargePower>(ctx, Owner.Creature, DynamicVars["Satchel"].BaseValue);

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) =>
        await Hit(ctx, play, "TitanDamage", DynamicVars["TitanHits"].IntValue, "vfx/vfx_fire_burst");

    protected override void OnUpgrade()
    {
        DynamicVars["Satchel"].UpgradeValueBy(6);
        DynamicVars["TitanHits"].UpgradeValueBy(2);
    }
}
