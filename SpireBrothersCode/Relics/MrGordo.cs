using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Buffy (her favorite show): Buffy's stuffed pig. Heal 3 HP after each combat, like vanilla Burning Blood.</summary>
public class MrGordo : BrandiRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(3)];

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner == null || Owner.Creature.IsDead) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}
