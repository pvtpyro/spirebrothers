using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Bribes work. Gain 2 Kids.</summary>
public class JuiceBox : TimPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Kids", 2)];
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Kids)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Kids.Gain(choiceContext, Owner, DynamicVars["Kids"].IntValue, null);
    }
}
