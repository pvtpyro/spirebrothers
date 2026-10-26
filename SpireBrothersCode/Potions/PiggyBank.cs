using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Every coin David has ever not spent. Gain 100 Gold. Usable any time.</summary>
public class PiggyBank : DavidPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.AnyTime;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Gold", 100)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PlayerCmd.GainGold(DynamicVars["Gold"].IntValue, Owner);
    }
}
