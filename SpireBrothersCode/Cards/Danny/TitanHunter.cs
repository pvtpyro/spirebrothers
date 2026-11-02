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

/// <summary>Titanfall. Pilot: gain 50% more Titan Meter. Titan (Fast Repair): at the start of your turn, Repair 3.</summary>
public class TitanHunter() : DannyCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Repair", 3)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter), HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<TitanHunterPower>(ctx, Owner.Creature, 50);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Apply<FastRepairPower>(ctx, Owner.Creature, DynamicVars["Repair"].BaseValue);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
