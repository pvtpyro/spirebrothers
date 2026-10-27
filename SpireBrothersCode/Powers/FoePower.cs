using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Base for the Bible elites' marker powers (Champion of Gath, Den of Lions, Seven Trumpets, Coppersmith). Put on the
/// foe who talks. On turn 1 it hands the hero his starting card and runs the opening lines (see BibleElite). The
/// subclass's loc text says what the fight is about and names the hero's card.
/// </summary>
public abstract class FoePower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;

    protected BibleElite? Elite => Owner.CombatState?.Encounter as BibleElite;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var combat = Owner.CombatState;
        if (combat == null || combat.RoundNumber != 1 || Owner.IsDead || Elite is not { } elite) return;

        if (elite.Hero(combat) == player)
        {
            Flash();
            await CardPileCmd.AddGeneratedCardToCombat(elite.MakeToken(combat, player), PileType.Hand, player);
        }
        // The opening lines, once: on the first player's turn start.
        if (player == combat.Players.FirstOrDefault()) elite.Opening(combat);
    }
}
