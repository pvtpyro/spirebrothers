using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>
/// Ancient (Archaic Tooth, replaces Cartel Special). The stimm, perfected. Gain 2 Energy and 6 Block. Grind 2 (Block).
/// Exhaust. The Stimm Lab talents that boost Cartel Special (Potency, Fast Acting) work on it too.
/// </summary>
public class CartelMasterwork() : JohnCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, BrotherKeywords.Grind];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2), new BlockVar(6, ValueProp.Move), new DynamicVar("Grind", 2)];
    protected override DynamicVar GrindTarget => DynamicVars.Block;

    private bool FastActing => StimmLab.Rank(Owner, StimmLab.Talent.FastActing) > 0;

    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("FastActing", IsMutable && Owner != null && FastActing);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        SyncGrind();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CommonActions.CardBlock(this, play);
        if (FastActing) await CardPileCmd.Draw(ctx, 1, Owner);
        await GrindAfterPlay(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}
