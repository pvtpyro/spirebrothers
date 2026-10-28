using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Takes the Family Album's snapshots (Album): elite and boss wins, the final boss (a VICTORY group photo), and
/// defeats (AlbumDefeatPatch). Only in runs with one of our characters. A photo is drawn from pixel art when every
/// body in it has art (IPhotoArt), and is a real screenshot otherwise (vanilla characters or foes). Captions are
/// written from the fight (SPIREBROTHERS-ALBUM.* in powers.json). Each client keeps its own album; nothing here touches
/// the run, so co-op can't desync.
/// </summary>
public class AlbumTracker() : CustomSingletonModel(HookType.Combat)
{
    internal const string Loc = "SPIREBROTHERS-ALBUM.";

    /// <summary>A drawn photo has room for this many foes; the rest of a horde stays out of frame.</summary>
    private const int MaxFoes = 4;

    /// <summary>The fight in progress: who it was against and how much HP the party had at the start.</summary>
    private sealed class Fight
    {
        public required ICombatState Combat;
        public required List<Creature> Foes;
        public int StartHp;
    }

    private static Fight? _fight;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1 || _fight?.Combat == combat) return Task.CompletedTask;
        _fight = new Fight
        {
            Combat = combat,
            Foes = combat.Enemies.Where(e => e.IsMonster).ToList(),
            StartHp = combat.Players.Sum(p => p.Creature.CurrentHp),
        };
        return Task.CompletedTask;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        var combat = room.CombatState;
        if (combat == null || !Counts(combat.Players)) return;
        bool final = IsFinalBoss(room);
        if (!final && room.RoomType is not (RoomType.Elite or RoomType.Boss)) return;
        try
        {
            // Let the frame with the last blow (and any win line) draw first, in case this one is a screenshot.
            if (Engine.GetMainLoop() is SceneTree tree)
                await tree.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
            Album.Add(final ? Victory(combat) : Win(combat, room.RoomType == RoomType.Boss ? "BOSS" : "ELITE"));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Family Album: couldn't take the {room.RoomType} photo: {e}");
        }
    }

    /// <summary>The same test the game uses for the run's last boss (CombatManager, where it sets WinTime).</summary>
    private static bool IsFinalBoss(CombatRoom room)
    {
        if (room.RoomType != RoomType.Boss || room.CombatState?.RunState is not RunState run) return false;
        if (run.CurrentActIndex != run.Acts.Count - 1) return false;
        var map = run.Map;
        var last = map.SecondBossMapPoint ?? map.BossMapPoint;
        return last != null && run.CurrentMapCoord == last.coord;
    }

    /// <summary>Only runs with a brother or friend in them go in the album.</summary>
    internal static bool Counts(IEnumerable<Player> players) => players.Any(p => p.Character is BrotherCharacter);

    private static Snapshot Win(ICombatState combat, string kind)
    {
        var shot = NewSnapshot(kind, combat.RunState);
        var foes = Foes(combat);
        string foe = MainFoeName(foes);
        int fallen = combat.Players.Count(p => p.Creature.IsDead);
        int hpNow = combat.Players.Sum(p => p.Creature.CurrentHp);
        int hpMax = combat.Players.Sum(p => p.Creature.MaxHp);
        int lost = Math.Max(0, (_fight?.Combat == combat ? _fight.StartHp : hpNow) - hpNow);

        string situation =
            fallen > 0 ? "FALLEN" :
            lost == 0 ? "FLAWLESS" :
            hpNow * 4 < hpMax ? "CLOSE" :
            combat.RoundNumber <= 1 ? "ONE_TURN" :
            "TURNS";
        shot.Caption = Caption(kind, situation, l =>
        {
            l.Add("Foe", foe);
            l.Add("Turns", combat.RoundNumber);
            l.Add("Fallen", Names(combat.Players.Where(p => p.Creature.IsDead)));
        });
        Picture(shot, combat.Players, foes, combat);
        return shot;
    }

    /// <summary>The run's last boss: a group photo of everyone at the top, no foe in it.</summary>
    private static Snapshot Victory(ICombatState combat)
    {
        var shot = NewSnapshot("VICTORY", combat.RunState);
        var players = combat.Players.ToList();
        bool allFour = players.Count(BrothersInArms.IsBrother) == 4;
        shot.Caption = Caption("VICTORY", allFour ? "ALL_FOUR" : players.Count == 1 ? "SOLO" : "PARTY",
            l => l.Add("Party", Names(players)));
        Picture(shot, players, new List<Creature>(), combat);
        return shot;
    }

    /// <summary>Called by AlbumDefeatPatch just before the game over screen opens over the party's last moment.</summary>
    internal static void Defeat(RunState run)
    {
        if (!Counts(run.Players)) return;
        var shot = NewSnapshot("DEFEAT", run);
        var combat = _fight?.Combat;
        bool inCombat = combat != null && run.CurrentRoom is CombatRoom room && room.CombatState == combat;
        if (inCombat)
        {
            var foes = Foes(combat!);
            shot.Caption = Caption("DEFEAT", run.Players.Count == 1 ? "FOE_SOLO" : "FOE", l => l.Add("Foe", MainFoeName(foes)));
            Picture(shot, run.Players, foes, combat!);
        }
        else
        {
            // Out of combat (an event, the map, giving up HP at a rest site...) there's nothing to draw a foe from.
            shot.Caption = Caption("DEFEAT", "NO_FOE", l => l.Add("Floor", shot.Floor));
            shot.Shot = Album.SaveScreenshot();
        }
        Album.Add(shot);
    }

    private static Snapshot NewSnapshot(string kind, IRunState run) => new()
    {
        Kind = kind,
        Time = DateTimeOffset.Now.ToUnixTimeSeconds(),
        Act = run.CurrentActIndex + 1,
        Floor = run.TotalFloor,
    };

    /// <summary>The foes this fight started with, alive or not (fallen ones lie in the picture).</summary>
    private static List<Creature> Foes(ICombatState combat)
    {
        var foes = _fight?.Combat == combat ? _fight.Foes : combat.Enemies.Where(e => e.IsMonster).ToList();
        return foes.Where(f => !f.IsPet).ToList();
    }

    /// <summary>The one a caption names: the biggest foe (Goliath, not his Shield Bearer).</summary>
    private static string MainFoeName(List<Creature> foes) =>
        foes.OrderByDescending(f => f.MaxHp).FirstOrDefault()?.Monster?.Title.GetFormattedText() ?? "";

    /// <summary>Draws the photo when every body has art, or takes a screenshot when anyone doesn't.</summary>
    private static void Picture(Snapshot shot, IEnumerable<Player> players, List<Creature> foes, ICombatState combat)
    {
        var party = new List<PhotoFigure>();
        bool drawable = true;
        foreach (var player in players)
        {
            if (player.Character is not IPhotoArt art) { drawable = false; break; }
            party.Add(Figure(art, player.Creature.IsDead));
            // Pets with art stand beside their owner (the Mastiff); ones without (Tim's Kids) are left out.
            foreach (var pet in combat.Allies.Where(a => a.IsPet && a.PetOwner == player && a.IsAlive))
                if (pet.Monster is IPhotoArt petArt) party.Add(Figure(petArt, false));
        }
        var drawnFoes = new List<PhotoFigure>();
        foreach (var foe in foes)
        {
            if (foe.Monster is not IPhotoArt art) { drawable = false; break; }
            drawnFoes.Add(Figure(art, foe.IsDead));
        }
        if (drawable)
        {
            shot.Party = party;
            shot.Foes = drawnFoes.Take(MaxFoes).ToList();
        }
        else
        {
            shot.Shot = Album.SaveScreenshot();
        }
    }

    private static PhotoFigure Figure(IPhotoArt art, bool fallen) => new()
    {
        Art = art.PhotoArt,
        Height = art.PhotoHeight,
        Fallen = fallen,
        TipOver = art.PhotoTipOver,
    };

    /// <summary>"Daniel", "Daniel and David", "Daniel, David, and Joshua".</summary>
    private static string Names(IEnumerable<Player> players)
    {
        var names = players.Select(p => p.Character.Title.GetFormattedText()).ToList();
        return names.Count switch
        {
            0 => "",
            1 => names[0],
            2 => $"{names[0]} {And()} {names[1]}",
            _ => $"{string.Join(", ", names.Take(names.Count - 1))}, {And()} {names[^1]}",
        };
    }

    private static string And() => new LocString("powers", Loc + "AND").GetFormattedText();

    /// <summary>The caption for this kind of photo and situation: ALBUM.&lt;KIND&gt;.&lt;SITUATION&gt;, else ALBUM.&lt;SITUATION&gt;.</summary>
    private static string Caption(string kind, string situation, Action<LocString> fill)
    {
        string key = $"{Loc}{kind}.{situation}";
        if (!LocString.Exists("powers", key)) key = $"{Loc}{situation}";
        var text = new LocString("powers", key);
        fill(text);
        return text.GetFormattedText();
    }
}

/// <summary>
/// A defeat's snapshot, taken by GameOverWaitPatch right before the game over screen covers the party's last moment.
/// In combat it first waits for the bodies to finish falling over (the game opens the screen the instant the last one
/// dies, before the death animation has played). Giving up doesn't count, and neither does the end of a won run (the
/// game "kills" everyone after the Architect).
/// </summary>
public static class AlbumDefeat
{
    /// <summary>The die frames plus PixelBody's tip-over, with a beat to spare.</summary>
    private const float FallTime = 1.5f;

    private static readonly AccessTools.FieldRef<NRun, RunState> RunStateField = AccessTools.FieldRefAccess<NRun, RunState>("_state");

    private static RunState? Run(NRun screen)
    {
        var manager = RunManager.Instance;
        if (manager.IsAbandoned || manager.WinTime > 0) return null;
        return RunStateField(screen) is { } run && AlbumTracker.Counts(run.Players) ? run : null;
    }

    /// <summary>How long to hold the game over screen so the photo shows the party fallen.</summary>
    public static float Wait(NRun screen)
    {
        try { return Run(screen)?.CurrentRoom is CombatRoom ? FallTime : 0; }
        catch { return 0; }
    }

    public static void Take(NRun screen)
    {
        try
        {
            if (Run(screen) is { } run) AlbumTracker.Defeat(run);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Family Album: couldn't take the defeat photo: {e}");
        }
    }
}
