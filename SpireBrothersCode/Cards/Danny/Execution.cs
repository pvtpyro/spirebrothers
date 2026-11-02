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

/// <summary>Titanfall. Pilot: deal 8; if it kills, gain 10 Titan Meter. Titan (Termination): deal 12; if it kills, Repair 8.</summary>
public class Execution() : DannyCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8, ValueProp.Move), new DynamicVar("Meter", 10), new DamageVar("TitanDamage", 12, ValueProp.Move), new DynamicVar("Repair", 8)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter), HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    private static bool Killed(MegaCrit.Sts2.Core.Commands.Builders.AttackCommand attack) =>
        attack.Results.SelectMany(r => r).Any(r => r.WasTargetKilled);

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        if (Killed(await Hit(ctx, play, "Damage", vfx: "vfx/vfx_attack_slash"))) await GainMeter(DynamicVars["Meter"].BaseValue);
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        if (Killed(await Hit(ctx, play, "TitanDamage"))) await Repair(DynamicVars["Repair"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["TitanDamage"].UpgradeValueBy(4);
    }
}
