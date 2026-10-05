using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Wife Aggro (Daniel, "Walkies"). At the start of your next turn, the dog bites a random enemy for Amount damage.</summary>
public class WalkiesPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null) return;
        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count > 0)
        {
            Flash();
            var target = player.RunState.Rng.CombatTargets.NextItem(enemies)!;
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_slash");
            await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, Owner);
        }
        await PowerCmd.Remove(this);
    }
}
