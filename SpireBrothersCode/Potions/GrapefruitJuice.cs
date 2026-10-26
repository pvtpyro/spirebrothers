using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Daniel's favorite. Heal 8 HP and gain 1 energy. (The Grapefruit relic makes it even better.)</summary>
public class GrapefruitJuice : DanielPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(8), new EnergyVar(1)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
