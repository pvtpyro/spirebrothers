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

/// <summary>Titanfall. Pilot: deal 9. Titan (Leadwall): deal 5 to ALL enemies twice. Retain.</summary>
public class WingmanElite() : DannyCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move), new DamageVar("TitanDamage", 5, ValueProp.Move)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "Damage", vfx: "vfx/vfx_dramatic_stab");

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => await Hit(ctx, play, "TitanDamage", 2);

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["TitanDamage"].UpgradeValueBy(1);
    }
}
