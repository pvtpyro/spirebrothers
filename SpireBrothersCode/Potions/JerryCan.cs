using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Brandi, Private Pyro. Apply 6 Ember to ALL enemies.</summary>
public class JerryCan : BrandiPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AllEnemies;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ember", 6)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Ember.Apply(choiceContext, Ember.Enemies(Owner.Creature.CombatState), DynamicVars["Ember"].IntValue, Owner, null);
    }
}
