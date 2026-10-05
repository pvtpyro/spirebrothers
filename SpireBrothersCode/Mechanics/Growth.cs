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
    // TEMP (2026-10-04): playtest diagnostics for the faded-number display. Logs a card's values once each time they
    // change, so godot.log shows whether "grown" was detected. Remove once the display is confirmed working.
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<CardModel, string> LastLogged = new();

    private static void LogOnChange(CardModel card, decimal start, decimal now, decimal preview)
    {
        if (card.CombatState == null) return;
        var state = $"start={start} now={now} preview={preview} grown={now > start}";
        if (LastLogged.TryGetValue(card, out var last) && last == state) return;
        LastLogged.AddOrUpdate(card, state);
        MainFile.Logger.Info($"[Growth] {card.GetType().Name}: {state}");
    }

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
            LogOnChange(card, start, now, v.PreviewValue);
            return;
        }
    }
}
