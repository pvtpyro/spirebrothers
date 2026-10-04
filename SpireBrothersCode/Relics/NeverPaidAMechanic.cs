using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>At the end of each combat, heal 5 HP.</summary>
public class NeverPaidAMechanic : DanielRelic
{
    public const int HealAmount = 5;
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (!Owner.Creature.IsAlive) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, HealAmount);
    }
}
