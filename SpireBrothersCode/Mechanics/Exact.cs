using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Powers that react whenever their owner triggers Exact (Bookkeeping, Q.E.D., Return on Investment).</summary>
public interface IExactListener
{
    Task OnExact(PlayerChoiceContext choiceContext, Player player);
}

/// <summary>
/// Exact: a bonus if the player has exactly 0 energy after paying for the card. X-cost cards always count.
/// </summary>
public static class Exact
{
    /// <summary>True if playing this card right now would trigger Exact. Used for the gold glow.</summary>
    public static bool WouldTrigger(CardModel card)
    {
        var state = card.Owner?.PlayerCombatState;
        if (state == null) return false;
        return card.EnergyCost.CostsX || state.Energy == card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }

    /// <summary>
    /// Call at the start of OnPlay. Returns how many times Exact triggers: 0, 1, or 2 with Min-Max.
    /// Also runs the owner's Exact listeners once per trigger.
    /// </summary>
    public static async Task<int> Check(PlayerChoiceContext choiceContext, CardModel card)
    {
        var owner = card.Owner;
        var state = owner?.PlayerCombatState;
        if (owner == null || state == null) return 0;
        if (!card.EnergyCost.CostsX && state.Energy != 0) return 0;

        int times = owner.Creature.GetPower<MinMaxPower>() != null ? 2 : 1;
        var listeners = owner.Creature.Powers.OfType<IExactListener>().ToList();
        for (int i = 0; i < times; i++)
            foreach (var listener in listeners)
                await listener.OnExact(choiceContext, owner);
        return times;
    }
}
