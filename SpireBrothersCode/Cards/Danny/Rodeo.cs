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

/// <summary>Titanfall. Pilot: deal 10; its damage fills the Titan Meter twice. Titan (Rocket Punch): deal 13.</summary>
public class Rodeo() : DannyCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move), new DamageVar("TitanDamage", 13, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        var attack = await Hit(ctx, play, "Damage", vfx: "vfx/vfx_attack_slash");
        // The hit already filled the meter once (DannyTracker); this is the second time.
        await GainMeter(attack.Results.SelectMany(r => r).Sum(r => r.TotalDamage));
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "TitanDamage");

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["TitanDamage"].UpgradeValueBy(4);
    }
}
