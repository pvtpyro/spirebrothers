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

/// <summary>Titanfall. Pilot: your Titan has 12 more HP this combat. Titan (Nuclear Ejection): when your Titan is destroyed, deal 15 to ALL enemies.</summary>
public class TitanKit() : DannyCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Hull", 12), new DynamicVar("Nuke", 15)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<TitanKitPower>(ctx, Owner.Creature, DynamicVars["Hull"].BaseValue);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Apply<NuclearEjectionPower>(ctx, Owner.Creature, DynamicVars["Nuke"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars["Hull"].UpgradeValueBy(6);
        DynamicVars["Nuke"].UpgradeValueBy(5);
    }
}
