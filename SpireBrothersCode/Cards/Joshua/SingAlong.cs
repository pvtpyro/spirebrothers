using BaseLib.Cards.Variables;
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
using SpireBrothers.SpireBrothersCode.Orbs;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Starter. Chorus: gain 2 Block per Verse. (Gave Block to ALL players until 2026-10-26.)</summary>
public class SingAlong() : JoshuaCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Chorus];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockPerVerse", 2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int verses = await Chorus(ctx);
        var block = DynamicVars["BlockPerVerse"].BaseValue * verses;
        if (block > 0) await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, play);
    }

    protected override void OnUpgrade() => DynamicVars["BlockPerVerse"].UpgradeValueBy(1);
}
