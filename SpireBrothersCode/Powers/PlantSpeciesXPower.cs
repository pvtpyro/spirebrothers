using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>ARK. At the end of your turn, apply Amount Bleed to a random enemy.</summary>
public class PlantSpeciesXPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || CombatState == null || Owner.Player == null) return;
        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) return;
        Creature target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies)!;
        Flash();
        await PowerCmd.Apply<BleedPower>(choiceContext, target, Amount, Owner, null);
    }
}
