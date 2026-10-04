using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Factorio. Whenever you play a Gear, deal Amount damage to a random enemy.</summary>
public class AssemblyLinePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || cardPlay.Card is not Gear) return;
        if (CombatState == null || Owner.Player == null) return;
        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) return;
        Creature target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies)!;
        Flash();
        await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, Owner);
    }
}
