using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Proper Japanese matcha, whisked. Gain Verses until you have 10.</summary>
public class Matcha : JoshuaPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Verses", Verses.Max)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Verses.Gain(choiceContext, Owner, Verses.Max - Verses.Count(Owner), null);
    }
}
