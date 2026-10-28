using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Law: at the end of the enemies' turn, each enemy that didn't attack takes Amount damage.</summary>
public class NoLoiteringPower : LawPower
{
    // Enemies whose move this turn isn't an attack, noted as their turn starts.
    private List<Creature> _loitering = [];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy)
            _loitering = combatState.HittableEnemies.Where(e => e.Monster is { IntendsToAttack: false }).ToList();
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy) return;
        var guilty = _loitering.Where(IsFoe).ToList();
        _loitering = [];
        if (guilty.Count == 0) return;
        Enforce();
        for (int i = 0; i < Times; i++) await CreatureCmd.Damage(choiceContext, guilty, Amount, ValueProp.Unpowered, Owner, null);
    }
}
