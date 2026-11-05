using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Enshrouded. John stays in the Shroud until a card takes him out (no countdown since 2026-11-05: Daniel wanted
/// control over it). While he's in it his Attacks deal 50% more damage and he loses 1 HP at the end of each of his turns.
/// The Shroud has a depth (ShroudPower's amount) that grows by 1 each turn, up to MaxDepth; it costs nothing extra (a
/// growing HP cost was tried on 2026-11-05 and was too punishing), it only powers the payoff cards (Burst Out, Back to the Altar).
/// </summary>
public static class Shroud
{
    public const decimal DamageMultiplier = 1.5m;
    // Concentration Stimm (Stimm Lab talent, the old Extended Release): +10% per rank.
    public const decimal TalentBonusPerRank = 0.1m;
    public const int HpLossPerTurn = 1;
    public const int StartDepth = 1;
    public const int MaxDepth = 5;

    public static bool In(Player? player) => player?.Creature?.GetPower<ShroudPower>() != null;

    /// <summary>How deep he is (1 to MaxDepth). 0 when he's out.</summary>
    public static int Depth(Player? player) => player?.Creature?.GetPowerAmount<ShroudPower>() ?? 0;

    public static decimal Multiplier(Player? player) =>
        DamageMultiplier + TalentBonusPerRank * StimmLab.Rank(player, StimmLab.Talent.ExtendedRelease);

    /// <summary>Steps into the Shroud at depth 1. Does nothing if he's already in it.</summary>
    public static async Task Enter(PlayerChoiceContext ctx, Player player, CardModel? card)
    {
        if (In(player)) return;
        Sounds.Play(Sounds.JohnShroud);
        await PowerCmd.Apply<ShroudPower>(ctx, player.Creature, StartDepth, player.Creature, card);
        // Survivor's Lantern: a little cover on the way in.
        if (player.GetRelic<Relics.SurvivorsLantern>() is { } lantern) await lantern.OnEnterShroud();
    }

    /// <summary>Moves the depth up or down (Flame Shard raises it), between 1 and MaxDepth. Only while he's in the Shroud.</summary>
    public static async Task ChangeDepth(PlayerChoiceContext ctx, Player player, int by, CardModel? card)
    {
        var power = player.Creature.GetPower<ShroudPower>();
        if (power == null || by == 0) return;
        int target = Math.Clamp(power.Amount + by, StartDepth, MaxDepth);
        if (target != power.Amount) await PowerCmd.ModifyAmount(ctx, power, target - power.Amount, player.Creature, card);
    }

    /// <summary>Steps out of the Shroud and returns the depth he was at. Shroud Lungs and Elixir Well react to it.</summary>
    public static async Task<int> Leave(PlayerChoiceContext ctx, Player player)
    {
        var power = player.Creature.GetPower<ShroudPower>();
        if (power == null) return 0;
        int depth = power.Amount;
        await PowerCmd.Remove(power);
        // Shroud Lungs (Stimm Lab talent): breathe out, heal what the last turn would have cost.
        if (StimmLab.Rank(player, StimmLab.Talent.ShroudLungs) > 0) await CreatureCmd.Heal(player.Creature, depth);
        if (player.Creature.GetPower<ElixirWellPower>() is { } well) await well.OnLeftShroud(ctx);
        return depth;
    }
}
