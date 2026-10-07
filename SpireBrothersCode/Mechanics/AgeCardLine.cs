using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Once Tim has Aged Up, every card of his that deals damage or gives Block gets a gold line at the bottom of its text
/// saying what his Age adds ("Castle Age: +2 damage."), the same way enchantments add their purple line. It shows
/// everywhere, deck view included; in combat the damage/Block numbers also already include the bonus.
/// Normal upgrades are untouched and still show the usual green title and "+".
/// </summary>
[HarmonyPatch]
public static class AgeCardLine
{
    // CardModel.GetDescriptionForPile(PileType, DescriptionPreviewType, Creature) is private, as is its enum.
    private static MethodBase TargetMethod() =>
        AccessTools.GetDeclaredMethods(typeof(CardModel)).First(m => m.Name == "GetDescriptionForPile" && m.GetParameters().Length == 3);

    [HarmonyPostfix]
    private static void AddAgeLine(CardModel __instance, ref string __result)
    {
        try
        {
            if (!__instance.IsMutable || __instance.Owner is not { } owner) return;
            int age = Ages.Get(owner);
            if (age <= Ages.Dark) return;

            bool damage = Has(__instance, "Damage") || Has(__instance, "CalculatedDamage");
            bool block = __instance.GainsBlock || Has(__instance, "Block") || Has(__instance, "CalculatedBlock");
            if (!damage && !block) return;

            var line = new LocString("powers", $"SPIREBROTHERS-AGES.{(damage && block ? "cardBoth" : damage ? "cardDamage" : "cardBlock")}");
            line.Add("Age", Ages.Name(age));
            line.Add("Bonus", Ages.Bonus(owner));
            __result = string.IsNullOrEmpty(__result) ? line.GetFormattedText() : __result + "\n" + line.GetFormattedText();
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Age card line failed on {__instance.GetType().Name}: {e}");
        }
    }

    private static bool Has(CardModel card, string var) => card.DynamicVars.TryGetValue(var, out _);
}
