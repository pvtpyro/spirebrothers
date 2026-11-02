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

/// <summary>Titanfall. Pilot: whenever you call in your Titan, draw 3. Titan (Upgrade Core): gain 1 Energy at the start of each turn; Repair 5.</summary>
public class Warpfall() : DannyCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3), new EnergyVar(1), new DynamicVar("Repair", 5)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<WarpfallPower>(ctx, Owner.Creature, DynamicVars.Cards.BaseValue);

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<UpgradeCorePower>(ctx, Owner.Creature, DynamicVars.Energy.BaseValue);
        await Repair(DynamicVars["Repair"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["Repair"].UpgradeValueBy(7);
    }
}
