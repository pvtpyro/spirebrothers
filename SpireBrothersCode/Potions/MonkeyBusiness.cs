using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Monkey Island. This turn, Comebacks don't remove Insulted. Description is monkey-speak.</summary>
public class MonkeyBusiness : DanielPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
        [MonkeySpeak.Translation("potions", Id.Entry), HoverTipFactory.FromPower<InsultedPower>()];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<MonkeyBusinessPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
    }
}
