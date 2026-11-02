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

/// <summary>Titanfall. Pilot: gain 3 Block, draw 1. Titan (Eject!): eject from your Titan (it is destroyed), deal 10 to ALL enemies.</summary>
public class DoubleJump() : DannyCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new CardsVar(1), new DynamicVar("Eject", 10)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await Draw(ctx, DynamicVars.Cards.BaseValue);
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await Titan.Eject(Owner);
        await Titan.HitAll(ctx, Owner, DynamicVars["Eject"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["Eject"].UpgradeValueBy(4);
    }
}
