using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// End-of-run stats for each brother ("David hoarded 812 gold"), shown on the game over screen by RunStatsScreen.
/// The counters are bumped from game logic that runs the same on every co-op client (Wired.Check, Exact.Check,
/// Verses, Kids.Act, ...), so every brother's numbers match on every screen. Saved with the run like Ages.RunAge;
/// MainFile.Initialize touches this class so the fields register in time.
/// </summary>
public static class RunStats
{
    public static readonly SavedSpireField<Player, int> Wired = new(() => 0, "SpireBrothersStatWired");
    public static readonly SavedSpireField<Player, int> Stratagems = new(() => 0, "SpireBrothersStatStratagems");
    public static readonly SavedSpireField<Player, int> Gears = new(() => 0, "SpireBrothersStatGears");
    public static readonly SavedSpireField<Player, int> Rants = new(() => 0, "SpireBrothersStatRants");
    public static readonly SavedSpireField<Player, int> Exacts = new(() => 0, "SpireBrothersStatExacts");
    public static readonly SavedSpireField<Player, int> Healed = new(() => 0, "SpireBrothersStatHealed");
    public static readonly SavedSpireField<Player, int> VersesSung = new(() => 0, "SpireBrothersStatVerses");
    public static readonly SavedSpireField<Player, int> BiggestChorus = new(() => 0, "SpireBrothersStatChorus");
    public static readonly SavedSpireField<Player, int> KidDamage = new(() => 0, "SpireBrothersStatKidDamage");
    public static readonly SavedSpireField<Player, int> Scripts = new(() => 0, "SpireBrothersStatScripts");
    public static readonly SavedSpireField<Player, int> ToxinApplied = new(() => 0, "SpireBrothersStatToxin");
    public static readonly SavedSpireField<Player, int> ShroudTurns = new(() => 0, "SpireBrothersStatShroud");
    public static readonly SavedSpireField<Player, int> Bites = new(() => 0, "SpireBrothersStatBites");
    public static readonly SavedSpireField<Player, int> LawsEnforced = new(() => 0, "SpireBrothersStatLaws");
    public static readonly SavedSpireField<Player, int> MetalSpent = new(() => 0, "SpireBrothersStatMetal");
    public static readonly SavedSpireField<Player, int> EmberApplied = new(() => 0, "SpireBrothersStatEmber");
    public static readonly SavedSpireField<Player, int> Shots = new(() => 0, "SpireBrothersStatShots");
    public static readonly SavedSpireField<Player, int> Spits = new(() => 0, "SpireBrothersStatSpits");
    public static readonly SavedSpireField<Player, int> Zapped = new(() => 0, "SpireBrothersStatZapped");

    public static void Add(SavedSpireField<Player, int> stat, Player? player, int amount = 1)
    {
        if (player != null && amount > 0) stat.Set(player, stat.Get(player) + amount);
    }

    /// <summary>Keeps the highest value seen.</summary>
    public static void Max(SavedSpireField<Player, int> stat, Player? player, int value)
    {
        if (player != null && value > stat.Get(player)) stat.Set(player, value);
    }

    /// <summary>The most gold the player carried between rooms, from the run's own history.</summary>
    public static int PeakGold(IRunState runState, Player player) =>
        runState.MapPointHistory.SelectMany(act => act)
            .Select(entry => entry.PlayerStats.FirstOrDefault(p => p.PlayerId == player.NetId)?.CurrentGold ?? 0)
            .Append(player.Gold)
            .Max();

    /// <summary>
    /// A brother's stat lines, best first, skipping any still at 0. Empty for other characters. Text lives in
    /// powers.json as SPIREBROTHERS-RUN_STATS.&lt;KEY&gt;, with the number as {Amount}.
    /// </summary>
    public static List<string> Lines(IRunState runState, Player player)
    {
        var lines = new List<(string Key, int Amount)>();
        switch (player.Character)
        {
            case Character.Daniel:
                lines.Add(("WIRED", Wired.Get(player)));
                lines.Add(("STRATAGEMS", Stratagems.Get(player)));
                lines.Add(("GEARS", Gears.Get(player)));
                break;
            case Character.David:
                lines.Add(("GOLD", PeakGold(runState, player)));
                lines.Add(("RANTS", Rants.Get(player)));
                lines.Add(("EXACT", Exacts.Get(player)));
                break;
            case Character.Joshua:
                lines.Add((runState.Players.Count > 1 ? "HEALED" : "HEALED_SOLO", Healed.Get(player)));
                lines.Add(("CHORUS", BiggestChorus.Get(player)));
                lines.Add(("VERSES", VersesSung.Get(player)));
                break;
            case Character.Tim:
                lines.Add(("KIDS", KidDamage.Get(player)));
                lines.Add(($"AGE{Ages.Get(player)}", Ages.Get(player)));
                lines.Add(("SCRIPTS", Scripts.Get(player)));
                break;
            case Character.John:
                lines.Add(("GRINDS", Grind.Times.Get(player)));
                lines.Add(("TOXIN", ToxinApplied.Get(player)));
                lines.Add(("SHROUD", ShroudTurns.Get(player)));
                break;
            case Character.Huffman:
                lines.Add(("BITES", Bites.Get(player)));
                lines.Add(("LAWS", LawsEnforced.Get(player)));
                lines.Add(("METAL", MetalSpent.Get(player)));
                break;
            case Character.Brandi:
                lines.Add(("EMBER", EmberApplied.Get(player)));
                lines.Add(("SHOTS", Shots.Get(player)));
                lines.Add(("SPITS", Spits.Get(player)));
                lines.Add(("ZAPPED", Zapped.Get(player)));
                break;
        }
        return lines.Where(l => l.Amount > 0).Select(l =>
        {
            var text = new LocString("powers", $"SPIREBROTHERS-RUN_STATS.{l.Key}");
            text.Add("Amount", l.Amount);
            return text.GetFormattedText();
        }).ToList();
    }
}

/// <summary>
/// Counts card plays for RunStats, and remembers whose card or potion is resolving so HealCredit can give the
/// healing to that player (Joshua's "healed the team" stat).
/// </summary>
public class RunStatsTracker() : CustomSingletonModel(HookType.Combat)
{
    // A stack, since a card can play another card mid-resolution (Script replays, auto-plays).
    internal static readonly List<Player> Sources = new();

    public override Task BeforeCombatStart()
    {
        Sources.Clear();
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner is { } owner) Sources.Add(owner);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner == null) return Task.CompletedTask;
        int index = Sources.LastIndexOf(owner);
        if (index >= 0) Sources.RemoveAt(index);

        if (cardPlay.Card.Keywords.Contains(BrotherKeywords.Stratagem)) RunStats.Add(RunStats.Stratagems, owner);
        if (cardPlay.Card is Gear) RunStats.Add(RunStats.Gears, owner);
        return Task.CompletedTask;
    }

    public override Task BeforePotionUsed(PotionModel potion, Creature? target)
    {
        if (potion.Owner is { } owner) Sources.Add(owner);
        return Task.CompletedTask;
    }

    public override Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        int index = potion.Owner == null ? -1 : Sources.LastIndexOf(potion.Owner);
        if (index >= 0) Sources.RemoveAt(index);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Sources.Clear();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Healing a player from someone's card or potion counts toward that someone's Healed stat. Only the HP actually
/// restored counts, not overhealing. Heal changes HP before its first await, so the prefix sees the HP beforehand.
/// </summary>
[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Heal))]
public static class HealCredit
{
    [HarmonyPrefix]
    private static void BeforeHeal(Creature creature, decimal amount)
    {
        try
        {
            if (RunStatsTracker.Sources.Count == 0 || creature is not { IsPlayer: true } || amount <= 0) return;
            int healed = (int)Math.Min(amount, creature.MaxHp - creature.CurrentHp);
            RunStats.Add(RunStats.Healed, RunStatsTracker.Sources[^1], healed);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Heal stat failed: {e}");
        }
    }
}
