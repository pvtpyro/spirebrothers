using BaseLib.Abstracts;
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

/// <summary>
/// Starter. His custom Hive Scum stimm, tuned up over the run. Gain 1 Energy and 3 Block. Grind 1 (Block). Exhaust.
/// Stimm Lab talents: Potency adds Block (JohnTracker), Fast Acting adds a draw.
/// </summary>
public class CartelSpecial() : JohnCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self), ITranscendenceCard
{
    // Archaic Tooth swaps it for this.
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<CartelMasterwork>();

    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, BrotherKeywords.Grind];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new BlockVar(3, ValueProp.Move), new DynamicVar("Grind", 1)];
    protected override DynamicVar GrindTarget => DynamicVars.Block;

    private bool FastActing => StimmLab.Rank(Owner, StimmLab.Talent.FastActing) > 0;

    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("FastActing", Owner != null && FastActing);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        SyncGrind();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CommonActions.CardBlock(this, play);
        if (FastActing) await CardPileCmd.Draw(ctx, 1, Owner);
        await GrindAfterPlay(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2);
}
