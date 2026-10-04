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
/// DeathLink (Archipelago). Whenever an enemy dies, ALL other enemies lose HP equal to Amount% of its Max HP.
/// </summary>
public class DeathLinkPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || !creature.IsEnemy || !Owner.IsAlive || CombatState == null) return;
        int loss = (int)Math.Max(1m, Math.Floor(creature.MaxHp * (Amount / 100m)));
        var others = CombatState.HittableEnemies.Where(e => e != creature && e.IsAlive).ToList();
        if (others.Count == 0) return;
        Flash();
        foreach (var enemy in others)
            await CreatureCmd.Damage(choiceContext, enemy, loss, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
    }
}
