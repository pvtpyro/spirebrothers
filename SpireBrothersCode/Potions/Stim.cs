using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Helldivers 2. Heal 30% of your Max HP and gain 2 energy.</summary>
public class Stim : DanielPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HealPercent", 30), new EnergyVar(2)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var creature = Owner.Creature;
        await CreatureCmd.Heal(creature, Math.Max(1, creature.MaxHp * DynamicVars["HealPercent"].IntValue / 100));
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
