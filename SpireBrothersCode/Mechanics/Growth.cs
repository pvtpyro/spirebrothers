using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Growing numbers show the printed value struck through and the current value in green: "[s]6[/s] [green]9[/green]".
/// This adds {CalcGrown} and {CalcStart} for cards using a CalculatedDamage or CalculatedBlock var.
/// </summary>
public static class Growth
{
    public static void AddCalcArgs(CardModel card, LocString description)
    {
        foreach (var key in new[] { "CalculatedDamage", "CalculatedBlock" })
        {
            if (!card.DynamicVars.TryGetValue(key, out var v) || v is not CalculatedVar calc) continue;
            decimal start = card.DynamicVars["CalculationBase"].BaseValue;
            decimal now = start;
            try { now = calc.Calculate(null); }
            catch (Exception e) { MainFile.Logger.Error($"Growth preview failed on {card.GetType().Name}: {e}"); }
            description.Add("CalcGrown", now > start);
            description.Add("CalcStart", start);
            return;
        }
    }
}
