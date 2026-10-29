using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;
using SpireBrothers.SpireBrothersCode.Monsters;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Brother banter: speech bubbles at combat start, when a brother falls or is saved, at big moments, and after boss and
/// elite wins. Lines are SPIREBROTHERS-BANTER.* in powers.json (draft list in BANTER.md); a pool is <c>KEY.0</c>,
/// <c>KEY.1</c>, ... and any number of lines works, since the count is read from the loc file.
/// Picks are seeded from the run and a counter that every co-op client advances in the same order, so everyone sees
/// the same line. Display only: it never touches the run's RNG.
/// </summary>
public class Banter() : CustomSingletonModel(HookType.Combat)
{
    internal const string Prefix = "SPIREBROTHERS-BANTER.";

    // Chance per combat that someone talks at the start: 1 in 3 with brothers, 1 in 4 alone. Elites: 1 in 2.
    private const int PartyStartChance = 3, SoloStartChance = 4, EliteChance = 2;

    public const int GearsMoment = 5, RantsMoment = 4, ChorusMoment = 8, GoldMoment = 300;

    /// <summary>How long a bubble stays up: at least SecondsMin, or SecondsPerChar per character for long lines.</summary>
    private const double SecondsMin = 4.0, SecondsPerChar = 0.11;

    private sealed class Counter { public int Next; }
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<IRunState, Counter> Rolls = new();

    // Big moments already said this combat (per player), and David's gold line (once per run, kept in memory only).
    private static readonly SpireField<PlayerCombatState, HashSet<string>> SaidThisCombat = new(() => new());
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Player, object> SaidGold = new();

    private static readonly Dictionary<string, int> PoolSizes = new();

    /// <summary>
    /// Shows a speech bubble over a creature, long enough to read. Every bubble in the mod comes through here (insults,
    /// the Bible elites, Blockbeard, dice rolls), so it also notes the time for the rewards and game over screens.
    /// </summary>
    public static void Bubble(Creature speaker, string text)
    {
        // A face (Brandi) is read at a glance; the [img] tag's length says nothing about that.
        double seconds = text.StartsWith("[img") ? SecondsMin : Math.Max(SecondsMin, text.Length * SecondsPerChar);
        var bubble = NSpeechBubbleVfx.Create(text, speaker, seconds, VfxColor.White);
        if (bubble == null) return;
        speaker.GetVfxContainer()?.AddChildSafely(bubble);
        _lastBubbleAt = Godot.Time.GetTicksMsec();
    }

    /// <summary>Upper-case name used in loc keys: DANIEL, DAVID, JOSHUA, TIM.</summary>
    internal static string Name(Player p) => p.Character.GetType().Name.ToUpperInvariant();

    /// <summary>Who gets banter lines: the brothers, Brandi (in faces), and the two friends they always play with.</summary>
    public static bool Talks(Player p) =>
        BrothersInArms.IsBrother(p) || p.Character is Character.John or Character.Huffman or Character.Brandi;

    /// <summary>
    /// Brandi never says a word: whatever pool she'd speak from, she makes a face from FACES.&lt;situation&gt; instead
    /// (START.BRANDI_TIM becomes FACES.START, FALL.DAVID becomes FACES.FALL). Each line there is a smiley picture
    /// (images/faces/, drawn by friendbuttons) in an [img] tag.
    /// </summary>
    private static string[] ForSpeaker(Creature speaker, string[] pools) =>
        speaker.Player?.Character is Character.Brandi ? ["FACES." + pools[0].Split('.')[0]] : pools;

    /// <summary>A brother spoke to Brandi: a moment later she answers with a face.</summary>
    private static void FaceReply(Player brandi, IRunState run)
    {
        try
        {
            if (Godot.Engine.GetMainLoop() is not Godot.SceneTree tree) return;
            var creature = brandi.Creature;
            tree.CreateTimer(ReplyDelay).Timeout += () =>
            {
                if (creature.IsAlive && creature.CombatState != null) Say(creature, run, "FACES.REPLY");
            };
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Brandi's face reply failed: {e}");
        }
    }

    private const double ReplyDelay = 1.8;

    internal static int PoolSize(string key)
    {
        if (PoolSizes.TryGetValue(key, out int n)) return n;
        n = 0;
        while (LocString.Exists("powers", $"{Prefix}{key}.{n}")) n++;
        return PoolSizes[key] = n;
    }

    /// <summary>A number in [0, max) that every co-op client gets the same.</summary>
    private static int Roll(IRunState run, int max)
    {
        int n = Rolls.GetOrCreateValue(run).Next++;
        return new Random(unchecked((int)run.Rng.CombatTargets.Seed * 31 + n * 7919)).Next(max);
    }

    /// <summary>Says a random line from one or more pools (e.g. a shared pool plus the speaker's own lines).</summary>
    private static void Say(Creature speaker, IRunState run, params string[] pools)
    {
        try
        {
            pools = ForSpeaker(speaker, pools);
            var lines = pools.SelectMany(k => Enumerable.Range(0, PoolSize(k)).Select(i => $"{Prefix}{k}.{i}")).ToList();
            if (lines.Count == 0) return;
            Bubble(speaker, new LocString("powers", lines[Roll(run, lines.Count)]).GetFormattedText());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Banter failed ({string.Join(", ", pools)}): {e}");
        }
    }

    /// <summary>A big moment: said once per combat per player.</summary>
    public static void Moment(Player player, string key)
    {
        var state = player.PlayerCombatState;
        if (state == null || !player.Creature.IsAlive || !SaidThisCombat.Get(state).Add(key)) return;
        Say(player.Creature, player.RunState, key);
    }

    /// <summary>
    /// A shared pool, plus its FAMILY lines (Mom, Dad) when the speaker is one of the brothers. The friends talk too,
    /// but those aren't their parents.
    /// </summary>
    private static string[] WithFamily(Player speaker, string pool) =>
        BrothersInArms.IsBrother(speaker) ? [pool, pool + ".FAMILY"] : [pool];

    private static List<Player> LivingBrothers(ICombatState combat) =>
        combat.Players.Where(p => Banter.Talks(p) && p.Creature.IsAlive).ToList();

    // Combat start: one brother talks to another, or a lone brother talks to himself.
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var combat = player.Creature.CombatState;
        // Runs once per combat: on turn 1, for the first player in the list.
        if (combat == null || combat.RoundNumber != 1 || player != combat.Players.FirstOrDefault()) return Task.CompletedTask;
        // The Bible elites do their own talking (BibleElite), with their namesake brother if he's here.
        if (combat.Encounter is BibleElite) return Task.CompletedTask;
        var brothers = LivingBrothers(combat);
        if (brothers.Count == 0) return Task.CompletedTask;
        var run = player.RunState;
        var names = brothers.Select(Name).Distinct().ToList();

        if (names.Count == 1)
        {
            if (Roll(run, SoloStartChance) != 0) return Task.CompletedTask;
            var solo = brothers[Roll(run, brothers.Count)];
            Say(solo.Creature, run, "SOLO." + Name(solo));
            return Task.CompletedTask;
        }

        if (Roll(run, PartyStartChance) != 0) return Task.CompletedTask;
        var speaker = brothers[Roll(run, brothers.Count)];
        var listeners = names.Where(n => n != Name(speaker)).ToList();
        string listener = listeners[Roll(run, listeners.Count)];
        Say(speaker.Creature, run, $"START.{Name(speaker)}_{listener}");
        if (listener == "BRANDI" && brothers.FirstOrDefault(p => Name(p) == listener) is { } brandi) FaceReply(brandi, run);
        return Task.CompletedTask;
    }

    // Who reacts when each brother falls, with their own line (FALL.<who fell>). Daniel says his himself.
    private static readonly Dictionary<string, string> FallReactor = new()
    {
        ["DAVID"] = "TIM", ["TIM"] = "JOSHUA", ["JOSHUA"] = "DAVID", ["DANIEL"] = "DANIEL",
        // The friends: uncle and nephew look out for each other.
        ["JOHN"] = "HUFFMAN", ["HUFFMAN"] = "JOHN",
        // Daniel looks out for his wife.
        ["BRANDI"] = "DANIEL",
    };

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.IsMonster && creature.CombatState?.Encounter is BibleElite elite)
        {
            elite.AfterDeath(creature);
            return Task.CompletedTask;
        }
        var fallen = creature.Player;
        var combat = creature.CombatState;
        if (fallen == null || combat == null || !Banter.Talks(fallen)) return Task.CompletedTask;
        var run = fallen.RunState;
        string name = Name(fallen);

        if (name == FallReactor[name])
        {
            Say(creature, run, "FALL." + name);
            return Task.CompletedTask;
        }
        var others = LivingBrothers(combat).Where(p => p != fallen).ToList();
        if (others.Count == 0) return Task.CompletedTask;
        var reactor = others.FirstOrDefault(p => Name(p) == FallReactor[name]);
        if (reactor != null) Say(reactor.Creature, run, "FALL." + name);
        else
        {
            // "I'm telling Mom" is for a brother who fell, not one of the friends.
            var reactorPlayer = others[Roll(run, others.Count)];
            Say(reactorPlayer.Creature, run, BrothersInArms.IsBrother(fallen) ? WithFamily(reactorPlayer, "FALL") : ["FALL"]);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var owner = card.Owner;
        var state = owner?.PlayerCombatState;
        if (owner == null || state == null) return Task.CompletedTask;

        if (card is StratagemOrbitalBarrage) Moment(owner, "ORBITAL");
        if (card is Gear && TurnTracker.GearsPlayed.Get(state) == GearsMoment) Moment(owner, "GEARS");

        // David's hoard: once per run, the first card he plays while holding 300+ Gold.
        if (owner.Character is Character.David && owner.Gold >= GoldMoment && !SaidGold.TryGetValue(owner, out _)
            && owner.Creature.IsAlive)
        {
            SaidGold.AddOrUpdate(owner, true);
            Say(owner.Creature, owner.RunState, "GOLD");
        }
        return Task.CompletedTask;
    }

    // The rewards and game over screens cover the bubbles. So if a bubble went up in the last ClosingHold seconds (a win
    // line, a fall, the insult that came with the killing blow...), they wait until it has been up that long: the rewards
    // screen opens once every AfterCombatVictory hook is done, so this hook holds; the game over screen is held by
    // GameOverWaitPatch. Each client only delays its own screen, so co-op stays in sync.
    private const float ClosingHold = 3.5f;
    private static ulong _lastBubbleAt;

    /// <summary>Seconds left before the last bubble has been readable long enough (0 if none). Clears the mark.</summary>
    internal static float TakeClosingWait()
    {
        if (_lastBubbleAt == 0) return 0;
        float shown = (Godot.Time.GetTicksMsec() - _lastBubbleAt) / 1000f;
        _lastBubbleAt = 0;
        return Math.Max(0, ClosingHold - shown);
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        SayVictoryLine(room);
        float wait = TakeClosingWait();
        if (wait > 0) await MegaCrit.Sts2.Core.Commands.Cmd.Wait(wait, ignoreCombatEnd: true);
    }

    private static void SayVictoryLine(CombatRoom room)
    {
        var combat = room.CombatState;
        if (combat == null || room.RoomType is not (RoomType.Boss or RoomType.Elite)) return;
        var brothers = LivingBrothers(combat);
        if (brothers.Count == 0) return;
        // The namesake brother already had the last word.
        if (combat.Encounter is BibleElite bible && bible.HeroHere(combat)) return;
        var run = brothers[0].RunState;
        if (room.RoomType == RoomType.Elite && Roll(run, EliteChance) != 0) return;
        var speaker = brothers[Roll(run, brothers.Count)];
        string pool = room.RoomType == RoomType.Boss ? "BOSS" : "ELITE";
        Say(speaker.Creature, run, [.. WithFamily(speaker, pool), $"{pool}.{Name(speaker)}"]);
    }

    /// <summary>Called by BanterSavedPatch when something stops a brother from dying.</summary>
    public static void Saved(Creature creature)
    {
        var player = creature.Player;
        if (player == null || !Banter.Talks(player)) return;
        Say(creature, player.RunState, "SAVED", "SAVED." + Name(player));
    }
}

/// <summary>
/// Only the model that prevented a death hears about it (Hook.AfterPreventingDeath calls just that one), so banter
/// listens in here: a brother cheated death (Nobody Gets Left Behind, I'm Not Done Yet, Lizard Tail, ...).
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterPreventingDeath))]
public static class BanterSavedPatch
{
    [HarmonyPostfix]
    private static void Postfix(Creature creature) => Banter.Saved(creature);
}

/// <summary>
/// The game over screen opens in the same step as the last brother's death (or giving up), right over his fall line.
/// If a bubble just went up, open it a moment later instead (Banter.TakeClosingWait). It also waits for a defeat in
/// combat to finish falling over, and takes the Family Album's defeat photo just before it opens (AlbumDefeat).
/// </summary>
[HarmonyPatch(typeof(NRun), nameof(NRun.ShowGameOverScreen))]
public static class GameOverWaitPatch
{
    private static bool _delayed;

    [HarmonyPrefix]
    private static bool Prefix(NRun __instance, SerializableRun serializableRun)
    {
        if (_delayed) return true;
        float wait = Math.Max(Banter.TakeClosingWait(), AlbumDefeat.Wait(__instance));
        if (wait <= 0 || Godot.Engine.GetMainLoop() is not Godot.SceneTree tree)
        {
            AlbumDefeat.Take(__instance);
            return true;
        }
        tree.CreateTimer(wait).Timeout += () =>
        {
            AlbumDefeat.Take(__instance);
            _delayed = true;
            try { __instance.ShowGameOverScreen(serializableRun); }
            finally { _delayed = false; }
        };
        return false;
    }
}