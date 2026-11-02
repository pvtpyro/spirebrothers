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

/// <summary>Titanfall. Pilot: gain 12 Titan Meter. Titan (Battery Swap): Repair 8.</summary>
public class BatteryPickup() : DannyCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Meter", 12), new DynamicVar("Repair", 8)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter), HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => GainMeter(DynamicVars["Meter"].BaseValue);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Repair(DynamicVars["Repair"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars["Meter"].UpgradeValueBy(4);
        DynamicVars["Repair"].UpgradeValueBy(3);
    }
}
