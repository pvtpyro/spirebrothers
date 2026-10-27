using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Powers and relics that react whenever their owner triggers Exact (Bookkeeping, Q.E.D., Return on Investment, Graphing Calculator).</summary>
public interface IExactListener
{
    Task OnExact(PlayerChoiceContext choiceContext, Player player);
}

/// <summary>
/// Exact: a bonus if the player has exactly 0 energy after paying for the card.
/// </summary>
public static class Exact
{
    /// <summary>True if playing this card right now would trigger Exact. Used for the gold glow.</summary>
    public static bool WouldTrigger(CardModel card)
    {
        var state = card.Owner?.PlayerCombatState;
        if (state == null) return false;
        if (card.EnergyCost.CostsX) return true;
        int left = state.Energy - card.EnergyCost.GetWithModifiers(CostModifiers.All);
        return left == 0 || (left == 1 && HasCoffee(card.Owner));
    }

    /// <summary>Gas Station Coffee: this turn, ending a card with 1 energy left also counts as Exact.</summary>
    private static bool HasCoffee(MegaCrit.Sts2.Core.Entities.Players.Player? owner) =>
        owner?.Creature?.GetPower<GasStationCoffeePower>() != null;

    /// <summary>
    /// Call at the start of OnPlay. Returns how many times Exact triggers: 0, 1, or 2 with Min-Max.
    /// Also runs the owner's Exact listeners once per trigger.
    /// </summary>
    public static async Task<int> Check(PlayerChoiceContext choiceContext, CardModel card)
    {
        var owner = card.Owner;
        var state = owner?.PlayerCombatState;
        if (owner == null || state == null) return 0;
        if (!card.EnergyCost.CostsX && state.Energy != 0 && !(state.Energy == 1 && HasCoffee(owner))) return 0;

        int times = owner.Creature.GetPower<MinMaxPower>() != null ? 2 : 1;
        RunStats.Add(RunStats.Exacts, owner);
        var listeners = owner.Creature.Powers.OfType<IExactListener>()
            .Concat(owner.Relics.OfType<IExactListener>()).ToList();
        for (int i = 0; i < times; i++)
            foreach (var listener in listeners)
                await listener.OnExact(choiceContext, owner);
        return times;
    }
}
