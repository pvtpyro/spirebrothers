using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// In memory of Brandi's late husband: his dog tags, stamped 33. She always starts with them, next to the Zippo. Once
/// per combat, when her HP drops below 33%, she gains 33 Block. Never offered anywhere else, and never upgraded.
/// </summary>
public class DogTags : BrandiRelic
{
    public const int Number = 33;
    private bool _used;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => false;
    public override RelicModel? GetUpgradeReplacement() => null;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (_used || delta >= 0 || Owner == null || creature != Owner.Creature || !creature.IsAlive) return;
        if (creature.CombatState == null || creature.CurrentHp * 100 >= creature.MaxHp * Number) return;
        _used = true;
        Flash();
        await CreatureCmd.GainBlock(creature, Number, ValueProp.Unpowered, null);
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _used = false;
        return Task.CompletedTask;
    }
}
