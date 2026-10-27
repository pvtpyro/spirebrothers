using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>
/// Ancient (Archaic Tooth, replaces Sing Along): the whole crowd sings it back. Chorus: gain 3 Block and deal 3 damage
/// to ALL enemies per Verse. (Drafted as an ALL-players heal; he doesn't like playing support, so it hits instead.)
/// </summary>
public class EverybodySing() : JoshuaCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Chorus];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("BlockPerVerse", 3),
        new CalculationBaseVar(0),
        new ExtraDamageVar(3),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(ChorusVerses)
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int verses = await ChorusAttack(ctx, play, "vfx/vfx_heavy_blunt");
        var block = DynamicVars["BlockPerVerse"].BaseValue * verses;
        if (block > 0) await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerVerse"].UpgradeValueBy(1);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
