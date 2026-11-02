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

/// <summary>Titanfall. Pilot: gain 6 Block, draw 1. Titan (Dash): Repair 4, draw 1.</summary>
public class WallRun() : DannyCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move), new CardsVar(1), new DynamicVar("Repair", 4)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Repair)];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await Draw(ctx, DynamicVars.Cards.BaseValue);
    }

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await Repair(DynamicVars["Repair"].BaseValue);
        await Draw(ctx, DynamicVars.Cards.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["Repair"].UpgradeValueBy(2);
    }
}
