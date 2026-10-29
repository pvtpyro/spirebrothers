using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Brandi always wanted to be a sniper. Aim (AimPower) never wears off on its own; Shot attacks deal bonus damage per
/// Aim and then spend all of it (One Shot, One Kill keeps it). The bonus is in each Shot card's CalculatedDamage.
/// </summary>
public static class Aim
{
    public static int Count(Player? player) => Math.Max(0, player?.Creature?.GetPowerAmount<AimPower>() ?? 0);

    public static async Task Gain(PlayerChoiceContext ctx, Player player, int amount, CardModel? card)
    {
        if (amount <= 0 || !player.Creature.IsAlive) return;
        await PowerCmd.Apply<AimPower>(ctx, player.Creature, amount, player.Creature, card);
    }

    /// <summary>After a Shot: removes her Aim (half of it stays with <paramref name="keepHalf"/>) and says how much she had.</summary>
    public static async Task<int> Spend(PlayerChoiceContext ctx, Player player, bool keepHalf = false)
    {
        var power = player.Creature.GetPower<AimPower>();
        if (power == null) return 0;
        int had = power.Amount;
        if (player.Creature.GetPower<OneShotOneKillPower>() != null) return had;
        int keep = keepHalf ? had / 2 : 0;
        if (keep <= 0) await PowerCmd.Remove(power);
        else await PowerCmd.ModifyAmount(ctx, power, keep - had, null, null);
        return had;
    }

    /// <summary>Shot attacks she has finished playing this turn (One Shot, One Kill allows one).</summary>
    public static int ShotsThisTurn(Player player, ICombatState? combat) =>
        combat == null ? 0 : CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(combat) && e.CardPlay.Card.Owner == player && e.CardPlay.Card is BrandiCard { IsShot: true });
}
