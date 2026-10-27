using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Cards;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Rubber Duck Mode (Ancient). After drawing each turn, enter the first input of a random Stratagem in hand for free,
/// or a random input if there is none. Picks use the run RNG so co-op stays in sync.
/// </summary>
public class RubberDuckModePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || player.PlayerCombatState == null) return Task.CompletedTask;
        var rng = player.RunState.Rng.CombatTargets;
        var inputs = PileType.Hand.GetPile(player).Cards
            .OfType<DanielCard>()
            .Select(c => c.StratagemInput)
            .Where(combo => combo is { Count: > 0 })
            .ToList();
        var input = inputs.Count > 0
            ? rng.NextItem(inputs)![0]
            : rng.NextItem(new[] { BrotherKeywords.Logic, BrotherKeywords.Hands })!;
        Flash();
        TurnTracker.EnterFreeInput(player, input);
        TimCard.RefreshHand(player);
        return Task.CompletedTask;
    }
}
