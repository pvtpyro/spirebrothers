using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Japanese good-luck charm. Whenever any player heals during combat, they also gain 3 Block.
/// The game's heal hook doesn't say who did the healing, so it covers every heal on a player.
/// </summary>
public class Omamori : JoshuaRelic
{
    public const int BlockAmount = 3;
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta <= 0 || !creature.IsPlayer || !creature.IsAlive || creature.CombatState == null) return;
        Flash();
        await CreatureCmd.GainBlock(creature, BlockAmount, ValueProp.Unpowered, null);
    }
}
