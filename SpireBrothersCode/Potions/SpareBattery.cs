using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>He always has one in a drawer somewhere. This turn, every Wired effect triggers.</summary>
public class SpareBattery : DanielPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Wired)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<SpareBatteryPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
    }
}
