using System.Globalization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// On Tim's cards, a {Var:diff()} number that differs from its printed value (Age bonus, Kids, Strength, Weak...)
/// shows the printed number dimmed and the current one after it: "6 8". Green if higher, red if lower.
/// This covers every Tim card at once, so his loc text uses plain {Damage:diff()} / {CalculatedDamage:diff()}.
/// Upgrade previews and other brothers' cards keep the vanilla look.
/// </summary>
[HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.ToHighlightedString))]
public static class TimDiffText
{
    private static readonly AccessTools.FieldRef<DynamicVar, AbstractModel?> OwnerRef =
        AccessTools.FieldRefAccess<DynamicVar, AbstractModel?>("_owner");

    [HarmonyPostfix]
    private static void ShowPrintedValue(DynamicVar __instance, bool inverse, ref string __result)
    {
        try
        {
            if (inverse || __instance.WasJustUpgraded || OwnerRef(__instance) is not TimCard) return;
            int now = (int)__instance.PreviewValue;
            int printed = (int)__instance.EnchantedValue;
            // Printed 0 is a "per Kid" total like Chores' "(8 Block)"; a dimmed 0 there would just be noise.
            if (now == printed || printed == 0) return;
            string color = now > printed ? "green" : "red";
            __result = $"[color=#ffffff8c]{printed.ToString(CultureInfo.InvariantCulture)}[/color] " +
                       $"[{color}]{now.ToString(CultureInfo.InvariantCulture)}[/{color}]";
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Tim diff text failed: {e}");
        }
    }
}
