using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>David's Sunday walks with Dad. At the start of your turn, heal Amount HP. Every 7th turn, also draw 2 cards.</summary>
public class SundayWalkPower : BrothersPower
{
    public const int BaseHeal = 2, SundayCards = 2, DaysPerWeek = 7;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || !Owner.IsAlive) return;
        Flash();
        await CreatureCmd.Heal(Owner, Amount);
        if (CombatState != null && CombatState.RoundNumber % DaysPerWeek == 0)
            await CardPileCmd.Draw(choiceContext, SundayCards, player);
    }
}
