using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Whenever you play a Rant card, ALL enemies lose Amount Strength (permanently).</summary>
public class VoidStaresBackPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || !cardPlay.Card.Keywords.Contains(BrotherKeywords.Rant)) return;
        if (CombatState == null) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, CombatState.HittableEnemies, -Amount, Owner, null);
    }
}
