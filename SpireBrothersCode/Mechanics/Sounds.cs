using BaseLib.Abstracts;
using BaseLib.Audio;
using Godot;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>A Joshua card that heals ALL players. Its play sounds the group-heal cue.</summary>
public interface IGroupHealCard { }

/// <summary>
/// Brother sound cues. Each plays SpireBrothers/audio/&lt;name&gt;.ogg (or .wav) if that file exists, otherwise a
/// built-in game sound (or nothing). So the brothers' own recordings can be dropped in any time with no code change.
/// Display only, played on each client. The cue list is in TODO.md #6.
/// </summary>
public class Sounds() : CustomSingletonModel(HookType.Run)
{
    public const string DanielWired = "daniel_wired", DanielBarrage = "daniel_barrage";
    public const string DavidGold = "david_gold", DavidRant = "david_rant";
    public const string JoshuaChorus = "joshua_chorus", JoshuaHeal = "joshua_heal";
    public const string TimKids = "tim_kids", TimAgeUp = "tim_age_up";
    public const string JohnStimm = "john_stimm", JohnShroud = "john_shroud";
    public const string HuffmanBark = "huffman_bark", HuffmanLaw = "huffman_law";
    public const string BrandiShot = "brandi_shot", BrandiZap = "brandi_zap", BrandiEmber = "brandi_ember", BrandiSpit = "brandi_spit";

    // Built-in game sounds used until a recording exists. Cues missing here stay silent.
    private static readonly Dictionary<string, string> Fallbacks = new()
    {
        [DanielWired] = "event:/sfx/characters/defect/defect_lightning_channel",
        [DavidGold] = "event:/sfx/ui/gold/gold_2",
        [JoshuaHeal] = FmodSfx.heal,
        [JohnStimm] = "event:/sfx/buff",
        [HuffmanBark] = "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_bite",
        // The game has no guns; the crossbow reload's clack is the closest thing to a bolt action.
        [BrandiShot] = "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider_reload",
        [BrandiZap] = "event:/sfx/characters/defect/defect_lightning_evoke",
        [BrandiEmber] = FmodSfx.fire,
        [BrandiSpit] = "event:/sfx/enemy/enemy_attacks/workbug_goop/workbug_goop_spit",
    };

    /// <summary>The same cue won't replay within this many seconds (Kids, back-to-back Choruses, several Davids).</summary>
    private const double MinGapSeconds = 0.4;
    private static readonly Dictionary<string, ulong> LastPlayed = new();
    private static readonly Dictionary<string, string?> Files = new();

    public static void Play(string cue)
    {
        try
        {
            ulong now = Time.GetTicksMsec();
            if (LastPlayed.TryGetValue(cue, out ulong last) && now - last < MinGapSeconds * 1000) return;
            LastPlayed[cue] = now;

            var file = FindFile(cue);
            if (file != null) new ModSound(file).Play();
            else if (Fallbacks.TryGetValue(cue, out var sfx)) SfxCmd.Play(sfx);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Sound {cue} failed: {e}");
        }
    }

    private static string? FindFile(string cue)
    {
        if (Files.TryGetValue(cue, out var found)) return found;
        found = new[] { ".ogg", ".wav" }.Select(ext => $"{MainFile.ResPath}/audio/{cue}{ext}").FirstOrDefault(path => ResourceLoader.Exists(path));
        return Files[cue] = found;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is StratagemOrbitalBarrage) Play(DanielBarrage);
        if (cardPlay.Card is IGroupHealCard) Play(JoshuaHeal);
        if (cardPlay.Card is Cards.John.CartelSpecial or Cards.John.CartelMasterwork) Play(JohnStimm);
        return Task.CompletedTask;
    }

    // David's gold in combat (his cards, Royalties, Piggy Bank...). Out of combat the game already plays its own.
    public override Task AfterGoldGained(Player player)
    {
        if (player.Character is Character.David && player.Creature?.CombatState != null) Play(DavidGold);
        return Task.CompletedTask;
    }
}
