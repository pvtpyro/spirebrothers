using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Thousand Cranes (senbazuru). The first time ANY player would die this combat, they heal Amount% of their
/// Max HP instead, then this goes away. Same hooks as Lizard Tail, but it watches every player.
/// </summary>
public class ThousandCranesPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _used;

    public override bool ShouldDieLate(Creature creature)
    {
        if (_used || !creature.IsPlayer || CombatState == null) return true;
        return !CombatState.Players.Any(p => p.Creature == creature);
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        _used = true;
        Flash();
        decimal amount = Math.Max(1m, creature.MaxHp * (Amount / 100m));
        await CreatureCmd.Heal(creature, amount);
        await PowerCmd.Remove(this);
    }
}
