using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// A Duo card: one brother's card that helps a specific other brother. It only shows up (rewards, shops, random
/// card generation) when that brother is in the party. Give it the Duo keyword so the hover tip explains that.
/// </summary>
public interface IDuoCard
{
    /// <summary>The brother this card works with (Character.Daniel, Character.David, ...).</summary>
    Type Partner { get; }
}

/// <summary>Keeps Duo cards out of card rewards and shops unless their partner is in the party.</summary>
public class DuoCards() : CustomSingletonModel(HookType.Run)
{
    /// <summary>True unless this is a Duo card whose partner isn't in the run (the player themselves counts).</summary>
    public static bool Allowed(CardModel card, Player player) =>
        card is not IDuoCard duo || player.RunState.Players.Any(p => IsPartner(p, duo.Partner));

    /// <summary>Living players who are this card's partner (usually one; co-op could have two of the same brother).</summary>
    public static List<Player> Partners(ICombatState? combat, Type partner) =>
        combat?.Players.Where(p => IsPartner(p, partner) && p.Creature.IsAlive).ToList() ?? [];

    // IsInstanceOfType instead of GetType() == partner, so a subclass or wrapper of the character still counts.
    private static bool IsPartner(Player p, Type partner) => partner.IsInstanceOfType(p.Character);

    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        LogFilter(player, options);
        if (options.CustomCardPool != null)
            return options.CustomCardPool.All(c => Allowed(c, player))
                ? options
                : options.WithCustomPool(options.CustomCardPool.Where(c => Allowed(c, player)));
        var filter = options.CardPoolFilter;
        return options.WithCardPools(options.CardPools.ToList(), c => (filter == null || filter(c)) && Allowed(c, player));
    }

    private static string? _lastLog;

    /// <summary>
    /// Writes "[Duo] ..." to godot.log: the party as the game sees it and which Duo cards were blocked. Co-op players
    /// reported only Daniel's Duo cards ever showing up with all four brothers in the party; this is to find out why.
    /// </summary>
    private static void LogFilter(Player player, CardCreationOptions options)
    {
        try
        {
            var cards = options.CustomCardPool ?? options.CardPools.SelectMany(p => p.AllCards);
            var duos = cards.Where(c => c is IDuoCard).ToList();
            if (duos.Count == 0) return;
            var party = string.Join(", ", player.RunState.Players.Select(p => $"{p.Character.GetType().FullName} ({p.Character.Id})"));
            var blocked = string.Join(", ", duos.Where(c => !Allowed(c, player)).Select(c => c.GetType().Name));
            var line = $"[Duo] reward for {player.Character.GetType().Name}; party: {party}; blocked: {(blocked == "" ? "none" : blocked)}";
            if (line == _lastLog) return;
            _lastLog = line;
            MainFile.Logger.Info(line);
        }
        catch (Exception e) { MainFile.Logger.Error($"Duo log failed: {e}"); }
    }

    public override IEnumerable<CardModel> ModifyMerchantCardPool(Player player, IEnumerable<CardModel> options) =>
        options.Where(c => Allowed(c, player));
}

/// <summary>
/// Keeps Duo cards out of in-combat card generation (Choices Paradox, Discovery, Attack Potion, ...) unless the partner is
/// in the party. Every vanilla generator goes through one of these two, and they know the player, so this covers them all.
/// Choices Paradox offered Feed Your Amp without Joshua in the run (2026-10-28).
/// </summary>
[HarmonyPatch(typeof(CardFactory))]
public static class DuoCardsCombatPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CardFactory.GetDistinctForCombat))]
    private static void Distinct(Player player, ref IEnumerable<CardModel> cards) => Filter(player, ref cards);

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CardFactory.GetForCombat))]
    private static void Any(Player player, ref IEnumerable<CardModel> cards) => Filter(player, ref cards);

    private static void Filter(Player player, ref IEnumerable<CardModel> cards)
    {
        var all = cards;
        cards = all.Where(c => DuoCards.Allowed(c, player)).ToList();
    }
}
