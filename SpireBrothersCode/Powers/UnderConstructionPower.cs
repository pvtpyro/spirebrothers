using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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

/// <summary>
/// The little GIF. At the end of Brandi's turn, her Amount most expensive cards are Retained. Picked for her (no choice
/// screen), since vanilla's pick-a-card version, Well-Laid Plans, is solo only.
/// </summary>
public class UnderConstructionPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        var keep = PileType.Hand.GetPile(player).Cards
            .Where(c => !c.ShouldRetainThisTurn)
            .OrderByDescending(c => c.EnergyCost.Canonical)
            .Take(Amount).ToList();
        if (keep.Count == 0) return Task.CompletedTask;
        Flash();
        foreach (var card in keep) card.GiveSingleTurnRetain();
        return Task.CompletedTask;
    }
}
