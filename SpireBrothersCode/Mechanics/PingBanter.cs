using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// The co-op Ping button (after you end your turn) shows one fixed line per character. For a brother, this picks a line
/// by who pinged and who is still taking their turn, from SPIREBROTHERS-BANTER.PING.* in powers.json:
/// <c>PING.&lt;PINGER&gt;_&lt;WAITEE&gt;</c> when one brother is left, <c>PING.&lt;PINGER&gt;.GROUP</c> when two or more are,
/// plus <c>PING.&lt;PINGER&gt;</c> (general lines, also used when the one left isn't a brother), and
/// <c>PING.DEAD.&lt;PINGER&gt;</c> when the pinger is dead. Empty pools fall back to the game's own line.
/// Each client shows the bubble itself, so lines go through a shuffled order seeded by the run and the pinger:
/// everyone sees the same line, and a line doesn't repeat until the rest of its pool has been said.
/// </summary>
[HarmonyPatch(typeof(FlavorSynchronizer), "CreateEndTurnPingDialogueIfNecessary")]
public static class PingBanter
{
    private const double SecondsMin = 2.5, SecondsPerChar = 0.08;

    private static readonly AccessTools.FieldRef<FlavorSynchronizer, Dictionary<Player, NSpeechBubbleVfx?>> Bubbles =
        AccessTools.FieldRefAccess<FlavorSynchronizer, Dictionary<Player, NSpeechBubbleVfx?>>("_endTurnPingDialogues");

    // How many times each player has pinged with each set of pools (kept in memory for the run).
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Player, Dictionary<string, int>> Said = new();

    [HarmonyPrefix]
    private static bool Prefix(FlavorSynchronizer __instance, Player player)
    {
        try
        {
            if (NRun.Instance == null || !Banter.Talks(player)) return true;
            string? text = PickLine(player);
            if (text == null) return true;

            var bubbles = Bubbles(__instance);
            if (bubbles.TryGetValue(player, out var old) && old != null && Godot.GodotObject.IsInstanceValid(old))
                old.QueueFreeSafely();
            double seconds = Math.Max(SecondsMin, text.Length * SecondsPerChar);
            var bubble = NSpeechBubbleVfx.Create(text, player.Creature, seconds, player.Character.SpeechBubbleColor);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(bubble);
            bubbles[player] = bubble;
            return false;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Ping banter failed: {e}");
            return true;
        }
    }

    private static string? PickLine(Player pinger)
    {
        string name = Banter.Name(pinger);
        string[] pools;
        if (pinger.Creature.IsDead)
        {
            pools = ["PING.DEAD." + name];
        }
        else
        {
            var combat = pinger.Creature.CombatState;
            if (combat == null) return null;
            var waiting = combat.Players
                .Where(p => p != pinger && !CombatManager.Instance.IsPlayerReadyToEndTurn(p)).ToList();
            if (waiting.Count >= 2) pools = [$"PING.{name}.GROUP", "PING." + name];
            else if (waiting.Count == 1 && Banter.Talks(waiting[0]))
                pools = [$"PING.{name}_{Banter.Name(waiting[0])}", "PING." + name];
            else pools = ["PING." + name];
        }

        var lines = pools.SelectMany(k => Enumerable.Range(0, Banter.PoolSize(k)).Select(i => $"{Banter.Prefix}{k}.{i}"))
            .ToList();
        if (lines.Count == 0) return null;
        return new LocString("powers", lines[Next(pinger, string.Join("+", pools), lines.Count)]).GetFormattedText();
    }

    /// <summary>The next index in a shuffled order of [0, size), reshuffled each time the whole pool has been said.</summary>
    private static int Next(Player pinger, string key, int size)
    {
        var counts = Said.GetOrCreateValue(pinger);
        counts.TryGetValue(key, out int n);
        counts[key] = n + 1;

        // string.GetHashCode changes between runs of the game, so hash the key by hand to match on every client.
        int hash = 17;
        foreach (char c in key) hash = unchecked(hash * 31 + c);
        var rng = new Random(unchecked((int)pinger.RunState.Rng.CombatTargets.Seed * 31 + hash
            + (int)pinger.NetId * 101 + n / size * 7919));
        var order = Enumerable.Range(0, size).ToArray();
        for (int i = size - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }
        return order[n % size];
    }
}
