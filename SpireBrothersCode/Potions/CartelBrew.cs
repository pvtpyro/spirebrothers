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

/// <summary>John (Hive Scum). Apply 6 Toxin to ALL enemies.</summary>
public class CartelBrew : JohnPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AllEnemies;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Toxin", 6)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner.Creature.CombatState is { } combat)
            await Toxin.Apply(choiceContext, combat.HittableEnemies, DynamicVars["Toxin"].IntValue, Owner, null);
    }
}
