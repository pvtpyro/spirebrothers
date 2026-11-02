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

/// <summary>Titanfall. Pilot: gain 1 Buffer. Titan (Energy Siphon): deal 10, Repair as much as the damage got through.</summary>
public class HoloPilot() : DannyCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>(1), new DamageVar("TitanDamage", 10, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<BufferPower>(), HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<BufferPower>(ctx, Owner.Creature, DynamicVars["BufferPower"].BaseValue);

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        var attack = await Hit(ctx, play, "TitanDamage", vfx: "vfx/vfx_attack_lightning");
        await Repair(attack.Results.SelectMany(r => r).Sum(r => r.UnblockedDamage));
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
