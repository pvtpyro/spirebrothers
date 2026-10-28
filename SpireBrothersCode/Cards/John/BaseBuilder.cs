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

/// <summary>Enshrouded base building. At the start of your turn, gain 2 Block. Grind 1 (the Block).</summary>
public class BaseBuilder() : JohnCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Grind];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BaseBuilderPower>(2), new DynamicVar("Grind", 1)];
    protected override DynamicVar GrindTarget => DynamicVars["BaseBuilderPower"];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        SyncGrind();
        await CommonActions.ApplySelf<BaseBuilderPower>(ctx, this);
        await GrindAfterPlay(ctx);
    }

    protected override void OnUpgrade() => DynamicVars["BaseBuilderPower"].UpgradeValueBy(1);
}
