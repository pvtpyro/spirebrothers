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

/// <summary>Titanfall. Pilot: gain 7 Block and 6 Titan Meter. Titan (Hold the Point): Repair 5, apply 1 Weak to ALL enemies.</summary>
public class Hardpoint() : DannyCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7, ValueProp.Move), new DynamicVar("Meter", 6), new DynamicVar("Repair", 5), new PowerVar<WeakPower>(1)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter), HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await GainMeter(DynamicVars["Meter"].BaseValue);
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await Repair(DynamicVars["Repair"].BaseValue);
        await ApplyAll<WeakPower>(ctx, DynamicVars["WeakPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["Meter"].UpgradeValueBy(3);
        DynamicVars["Repair"].UpgradeValueBy(2);
    }
}
