using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Relics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Orobas can offer Sea Glass for a random other character, titled by key "SEA_GLASS.&lt;character id&gt;.title".
/// The brothers have theirs in relics.json; this covers any modded character that doesn't (a missing key throws,
/// which could leave the event with no options), falling back to the plain "Sea Glass" title.
/// </summary>
[HarmonyPatch(typeof(SeaGlass), nameof(SeaGlass.Title), MethodType.Getter)]
public static class SeaGlassTitleFallback
{
    [HarmonyPostfix]
    private static void UsePlainTitleIfMissing(SeaGlass __instance, ref LocString __result)
    {
        try
        {
            if (!__result.Exists()) __result = new LocString("relics", __instance.Id.Entry + ".title");
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Sea Glass title fallback failed: {e}");
        }
    }
}
