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

/// <summary>Titanfall. Pilot: fill your Titan Meter. Titan (Flight Core): deal 5 to a random enemy 8 times. Exhaust.</summary>
public class PrepareForTitanfall() : DannyCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self, TargetType.RandomEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar("TitanDamage", 5, ValueProp.Move), new DynamicVar("TitanHits", 8)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => GainMeter(Titan.MeterSize);

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play) =>
        await Hit(ctx, play, "TitanDamage", DynamicVars["TitanHits"].IntValue, "vfx/vfx_fire_burst");

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["TitanHits"].UpgradeValueBy(2);
    }
}
