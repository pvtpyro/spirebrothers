using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>David's road trip fuel. This turn, Exact also triggers with 1 energy left.</summary>
public class GasStationCoffee : DavidPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Exact)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<GasStationCoffeePower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
    }
}
