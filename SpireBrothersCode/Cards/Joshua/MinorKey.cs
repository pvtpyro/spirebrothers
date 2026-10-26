using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Duo with David: "I wrote you a song. It's in a minor key." Chorus: apply 1 Bleed per Verse to ALL enemies.</summary>
public class MinorKey() : JoshuaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.David);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Chorus];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BleedPerVerse", 1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BleedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int verses = await Chorus(ctx);
        int bleed = verses * DynamicVars["BleedPerVerse"].IntValue;
        if (bleed > 0) await PowerCmd.Apply<BleedPower>(ctx, CombatState!.HittableEnemies, bleed, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
