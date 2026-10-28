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

/// <summary>John (WoW raid flasks). Every Grind card in your hand Grinds once, even if it already did this combat.</summary>
public class RaidFlask : JohnPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        foreach (var card in PileType.Hand.GetPile(Owner).Cards.OfType<JohnCard>().Where(c => c.HasGrind).ToList())
            await card.GrindTimes(choiceContext, 1);
    }
}
