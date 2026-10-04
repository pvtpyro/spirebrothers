using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Monkey Island items show pure monkey-speak as their description. This builds the extra hover tip,
/// titled "Translation", from the item's "&lt;ID&gt;.translation" loc key.
/// </summary>
public static class MonkeySpeak
{
    public static IHoverTip Translation(string table, string id, DynamicVarSet? vars = null)
    {
        var text = new LocString(table, id + ".translation");
        vars?.AddTo(text);
        return new HoverTip(new LocString(table, "SPIREBROTHERS-TRANSLATION.title"), text);
    }
}
