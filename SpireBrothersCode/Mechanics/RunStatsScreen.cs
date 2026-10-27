using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Runs;
using SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Shows RunStats on the game over screen's summary page, as a second column of score lines beside the vanilla ones
/// (floors climbed, gold gained, ...), each with the brother's icon. Every brother in the party gets his lines, on every
/// player's screen: 3 each with one or two brothers, 2 each with three or four, so the column stays at most 8 tall.
/// They slide in right after the vanilla lines.
/// </summary>
[HarmonyPatch(typeof(NGameOverScreen))]
public static class RunStatsScreen
{
    private const int LinesPerBrother = 3, LinesPerBrotherInBigParty = 2;
    private const int ColumnGap = 80;

    private static readonly AccessTools.FieldRef<NGameOverScreen, RunState> RunStateField =
        AccessTools.FieldRefAccess<NGameOverScreen, RunState>("_runState");
    private static readonly AccessTools.FieldRef<NGameOverScreen, GridContainer> ScoreLinesField =
        AccessTools.FieldRefAccess<NGameOverScreen, GridContainer>("_scoreLineContainer");

    private static readonly ConditionalWeakTable<NGameOverScreen, List<NScoreLine>> Columns = new();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NGameOverScreen._Ready))]
    private static void AfterReady(NGameOverScreen __instance)
    {
        try
        {
            var runState = RunStateField(__instance);
            var brothers = runState.Players.Where(p => p.Character is BrotherCharacter).ToList();
            int perBrother = brothers.Count <= 2 ? LinesPerBrother : LinesPerBrotherInBigParty;
            var lines = new List<NScoreLine>();
            foreach (var player in brothers)
            {
                var icon = Icon(player.Character as BrotherCharacter);
                foreach (var text in RunStats.Lines(runState, player).Take(perBrother))
                {
                    var line = NScoreLine.Create(text, "", icon);
                    if (icon == null) line.GetNode<TextureRect>("%Icon").Texture = null;   // not the debug placeholder
                    lines.Add(line);
                }
            }
            if (lines.Count == 0) return;

            // Put the vanilla score lines and ours side by side in a new row where the vanilla list was.
            var vanilla = ScoreLinesField(__instance);
            var parent = vanilla.GetParent();
            int index = vanilla.GetIndex();
            var row = new HBoxContainer { Name = "SpireBrothersStatsRow" };
            row.AddThemeConstantOverride("separation", ColumnGap);
            parent.RemoveChild(vanilla);
            row.AddChild(vanilla);
            parent.AddChild(row);
            parent.MoveChild(row, index);

            var column = new VBoxContainer { Name = "SpireBrothersStats" };
            column.AddThemeConstantOverride("separation", vanilla.GetThemeConstant("v_separation"));
            foreach (var line in lines) column.AddChild(line);   // the scene starts them transparent
            row.AddChild(column);
            Columns.AddOrUpdate(__instance, lines);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Run stats screen failed: {e}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("AnimateScoreLines")]
    private static void AfterAnimateScoreLines(NGameOverScreen __instance, ref Task __result) =>
        __result = AnimateOurs(__result, __instance);

    private static async Task AnimateOurs(Task vanilla, NGameOverScreen screen)
    {
        await vanilla;
        if (!Columns.TryGetValue(screen, out var lines)) return;
        foreach (var line in lines)
        {
            if (!GodotObject.IsInstanceValid(line)) return;
            await line.AnimateIn();
        }
    }

    private static Texture2D? Icon(BrotherCharacter? character)
    {
        var path = character?.CustomIconTexturePath;
        return path != null && ResourceLoader.Exists(path) ? GD.Load<Texture2D>(path) : null;
    }
}
