using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim loves to spend money. His cards pay gold through here (Age Up, Impulse Buy, Top of the Line) so Burning a Hole
/// can react: the game has no "gold lost" hook to listen to.
/// </summary>
public static class TimGold
{
    /// <summary>Pay up to <paramref name="amount"/> Gold (never below 0). Returns how much was actually spent.</summary>
    public static async Task<int> Spend(PlayerChoiceContext ctx, Player player, int amount)
    {
        int spent = Math.Min(amount, player.Gold);
        if (spent <= 0) return 0;
        await PlayerCmd.SetGold(player.Gold - spent, player);
        foreach (var power in player.Creature.Powers.OfType<BurningAHolePower>().ToList())
            await power.OnGoldSpent(ctx, spent);
        return spent;
    }
}
