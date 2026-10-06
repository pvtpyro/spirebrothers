using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Daniel's Machine Gun Sentry (Helldivers 2). At the end of your turn, deal 3 damage to a random enemy Amount times.
/// Targets use the run RNG so co-op stays in sync.
/// </summary>
public class MachineGunSentryPower : BrothersPower
{
    public const int DamagePerShot = 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || !Owner.IsAlive || Owner.Player == null || CombatState == null) return;
        Flash();
        var rng = Owner.Player.RunState.Rng.CombatTargets;
        for (int i = 0; i < Amount; i++)
        {
            var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            var target = rng.NextItem(enemies)!;
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_blunt");
            await CreatureCmd.Damage(choiceContext, target, DamagePerShot, ValueProp.Unpowered, Owner);
        }
    }
}
