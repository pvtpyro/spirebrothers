using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Our bosses' map icons (Apophis, Ba'al) are full-color pixel art. The map tints placeholder boss icons with the act's
/// ink color (vanilla's are drawn as silhouettes for that), which turned a dark-clothed boss into a black blob, so for
/// one of our bosses the tint is undone. The outline ring keeps the map's background color, as a halo.
/// RefreshColorInstantly runs from _Ready and whenever the point's state changes, so this covers both. The boss is
/// looked up the same way NBossMapPoint._Ready does.
/// </summary>
[HarmonyPatch(typeof(NBossMapPoint), "RefreshColorInstantly")]
public static class BossMapIconPatch
{
    [HarmonyPostfix]
    private static void Postfix(NBossMapPoint __instance)
    {
        var node = Traverse.Create(__instance);
        if (node.Field("_runState").GetValue<IRunState>() is not { } run) return;
        var boss = __instance.Point == run.Map.SecondBossMapPoint ? run.Act.SecondBossEncounter : run.Act.BossEncounter;
        if (boss is not BibleElite) return;
        if (node.Field("_placeholderImage").GetValue<TextureRect>() is { } image) image.SelfModulate = Colors.White;
    }
}
