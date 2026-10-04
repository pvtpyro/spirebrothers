using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Castle (Age of Empires). Per stack: at the start of your turn gain 5 Block; at the end, deal 3 damage to ALL enemies.</summary>
public class CastlePower : BrothersPower
{
    public const int BlockPerStack = 5, DamagePerStack = 3;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, BlockPerStack * Amount, ValueProp.Unpowered, null);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || CombatState == null) return;
        Flash();
        foreach (var enemy in CombatState.HittableEnemies.Where(e => e.IsAlive).ToList())
            await CreatureCmd.Damage(choiceContext, enemy, DamagePerStack * Amount, ValueProp.Unpowered, Owner);
    }
}
