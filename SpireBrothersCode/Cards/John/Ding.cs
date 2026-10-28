using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>WoW (the level-up sound). Gain 3 Block. Draw 1 card. Grind 1 (Block).</summary>
public class Ding() : JohnCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Grind];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new CardsVar(1), new DynamicVar("Grind", 1)];
    protected override DynamicVar GrindTarget => DynamicVars.Block;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        SyncGrind();
        await CommonActions.CardBlock(this, play);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        await GrindAfterPlay(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2);
}
