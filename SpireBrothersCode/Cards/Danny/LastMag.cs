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

/// <summary>Titanfall. Pilot: deal 7, double if you are below half HP. Titan (Doomed): deal 9, double if your Titan is below half HP.</summary>
public class LastMag() : DannyCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new DamageVar("TitanDamage", 9, ValueProp.Move)];

    private static bool BelowHalf(Creature? who) => who is { IsAlive: true } && who.CurrentHp * 2 < who.MaxHp;

    private bool Desperate => InTitan ? BelowHalf(MyTitan) : BelowHalf(Owner.Creature);

    protected override bool ExtraGlow => Desperate;

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play) =>
        await Hit(ctx, play, "Damage", Desperate ? 2 : 1, "vfx/vfx_attack_slash");

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) =>
        await Hit(ctx, play, "TitanDamage", Desperate ? 2 : 1);

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["TitanDamage"].UpgradeValueBy(3);
    }
}
