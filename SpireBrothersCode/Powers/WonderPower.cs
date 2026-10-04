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

/// <summary>
/// Wonder (Age of Empires wonder victory). Amount is the damage. The icon counts down the turns; at the start of
/// the 5th turn after it was played, deal Amount damage to ALL enemies. Playing another Wonder adds its damage.
/// </summary>
public class WonderPower : BrothersPower
{
    public const int Turns = 5;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _turnsLeft = Turns;
    public override int DisplayAmount => _turnsLeft;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null) return;
        _turnsLeft--;
        InvokeDisplayAmountChanged();
        if (_turnsLeft > 0) return;
        Flash();
        foreach (var enemy in CombatState.HittableEnemies.Where(e => e.IsAlive).ToList())
            await CreatureCmd.Damage(choiceContext, enemy, Amount, ValueProp.Unpowered, Owner);
        await PowerCmd.Remove(this);
    }
}
