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

/// <summary>Ballads (the crowd holds up lighters). Chorus: apply 1 Heartstrings to ALL enemies per Verse.</summary>
public class LightersUp() : JoshuaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Chorus];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HeartstringsPerVerse", 1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HeartstringsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int verses = await Chorus(ctx);
        int stacks = verses * DynamicVars["HeartstringsPerVerse"].IntValue;
        if (stacks > 0 && CombatState != null)
            await PowerCmd.Apply<HeartstringsPower>(ctx, CombatState.HittableEnemies, stacks, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
