using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Cards;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Huffman (his dog). The Mastiff gains 4 Bite and bites right now.</summary>
public class BagOfKibble : HuffmanPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bite", 4)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Mastiff.GainBite(choiceContext, Owner, DynamicVars["Bite"].IntValue, null);
        await Mastiff.Attack(choiceContext, Owner);
    }
}
