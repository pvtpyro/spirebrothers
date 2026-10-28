using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Enshrouded. John enters the Shroud for a number of his turns (the current one counts). While he's in it his
/// Attacks deal 50% more damage, and he loses 1 HP at the end of each of his turns. The countdown is ShroudPower.
/// </summary>
public static class Shroud
{
    public const decimal DamageMultiplier = 1.5m;
    public const int HpLossPerTurn = 1;

    public static bool In(Player? player) => player?.Creature?.GetPower<ShroudPower>() != null;

    public static int TurnsLeft(Player? player) => player?.Creature?.GetPowerAmount<ShroudPower>() ?? 0;

    public static async Task Enter(PlayerChoiceContext ctx, Player player, int turns, CardModel? card)
    {
        if (turns <= 0) return;
        // Flame Altar Upgrade: every trip in lasts longer.
        turns += player.Creature.GetPowerAmount<FlameAltarUpgradePower>();
        if (!In(player)) Sounds.Play(Sounds.JohnShroud);
        await PowerCmd.Apply<ShroudPower>(ctx, player.Creature, turns, player.Creature, card);
    }

    /// <summary>Adds turns only if he's already in the Shroud (Flame Shard).</summary>
    public static async Task Extend(PlayerChoiceContext ctx, Player player, int turns, CardModel? card)
    {
        var power = player.Creature.GetPower<ShroudPower>();
        if (power != null && turns > 0) await PowerCmd.ModifyAmount(ctx, power, turns, player.Creature, card);
    }

    /// <summary>Steps out of the Shroud and returns how many turns were left. Elixir Well reacts to it.</summary>
    public static async Task<int> Leave(PlayerChoiceContext ctx, Player player)
    {
        var power = player.Creature.GetPower<ShroudPower>();
        if (power == null) return 0;
        int left = power.Amount;
        await PowerCmd.Remove(power);
        if (player.Creature.GetPower<ElixirWellPower>() is { } well) await well.OnLeftShroud(ctx);
        return left;
    }

    /// <summary>HP he loses at the end of each turn in the Shroud: less with Shroud Lungs, none with the Altar Upgrade.</summary>
    public static int HpLoss(Player player)
    {
        if (player.Creature.GetPower<FlameAltarUpgradePower>() != null || player.GetRelic<Relics.SurvivorsLantern>() != null) return 0;
        return Math.Max(0, HpLossPerTurn - StimmLab.Rank(player, StimmLab.Talent.ShroudLungs));
    }
}
