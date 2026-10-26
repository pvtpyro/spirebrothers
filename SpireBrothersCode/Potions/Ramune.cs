using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>The marble-bottle soda from Japan. Gain 3 Verses.</summary>
public class Ramune : JoshuaPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Verses", 3)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Verses.Gain(choiceContext, Owner, DynamicVars["Verses"].BaseValue, null);
    }
}
