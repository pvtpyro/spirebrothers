using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Danny (Titanfall: electric smoke). Apply 2 Weak to ALL enemies.</summary>
public class SmokeCanister : DannyPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AllEnemies;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(2)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        foreach (var enemy in Owner.Creature.CombatState?.HittableEnemies.Where(e => e.IsAlive).ToList() ?? [])
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars["WeakPower"].BaseValue, Owner.Creature, null);
    }
}
