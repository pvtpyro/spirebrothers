using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Somebody gave the kids candy. Gain 1 Kid, then every Kid hits a random enemy twice.</summary>
public class SugarRush : TimPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Kids", 1), new DynamicVar("Times", 2)];
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Kids)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Kids.Gain(choiceContext, Owner, DynamicVars["Kids"].IntValue, null);
        for (int i = 0; i < DynamicVars["Times"].IntValue; i++) await Kids.Act(choiceContext, Owner);
    }
}
