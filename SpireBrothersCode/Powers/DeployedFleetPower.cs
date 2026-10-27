using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Space Base. At the start of your turn, Roll Amount times: each roll gives Block and hits a random enemy for half the roll (rounded up).</summary>
public class DeployedFleetPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null) return;
        for (int i = 0; i < Amount; i++)
        {
            int roll = await Dice.Roll(choiceContext, player);
            int payout = (roll + 1) / 2;
            Flash();
            await CreatureCmd.GainBlock(Owner, payout, ValueProp.Unpowered, null);
            var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) continue;
            var target = player.RunState.Rng.CombatTargets.NextItem(enemies)!;
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_blunt");
            await CreatureCmd.Damage(choiceContext, target, payout, ValueProp.Unpowered, Owner);
        }
    }
}
