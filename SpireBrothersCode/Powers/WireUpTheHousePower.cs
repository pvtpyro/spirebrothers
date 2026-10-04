using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Whenever you play a Logic card, deal Amount damage to a random enemy.</summary>
public class WireUpTheHousePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (!cardPlay.Card.Keywords.Contains(BrotherKeywords.Logic)) return;
        if (CombatState == null || Owner.Player == null) return;

        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) return;

        Creature target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies)!;
        Flash();
        await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, Owner);
    }
}
