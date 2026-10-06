using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Shows Bleed on the health bar the way the game shows Poison: the HP that Bleed will take at the end of the
/// enemy's turn is drawn in its own color (dark crimson) at the end of the bar, or the whole bar if it's lethal.
/// It sits just left of any Poison segment, since both hit at the end of the turn. Display only.
/// </summary>
[HarmonyPatch]
public static class BleedHealthBar
{
    /// <summary>Bleed's color on the bar. Plain HP is bright red (F1373E), Poison is green, Doom is purple.</summary>
    private static readonly Color BleedColor = new("8E0E26");

    private const string NodeName = "SpireBrothersBleedForeground";

    private static readonly AccessTools.FieldRef<NHealthBar, Creature> CreatureRef =
        AccessTools.FieldRefAccess<NHealthBar, Creature>("_creature");

    private static readonly AccessTools.FieldRef<NHealthBar, Control> HpForeground =
        AccessTools.FieldRefAccess<NHealthBar, Control>("_hpForeground");

    private static readonly Func<NHealthBar, float> MaxFgWidth =
        AccessTools.MethodDelegate<Func<NHealthBar, float>>(AccessTools.PropertyGetter(typeof(NHealthBar), "MaxFgWidth"));

    [HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
    [HarmonyPostfix]
    private static void AfterRefreshForeground(NHealthBar __instance)
    {
        try
        {
            var creature = CreatureRef(__instance);
            var hp = HpForeground(__instance);
            if (creature == null || hp == null) return;

            int bleed = creature.GetPowerAmount<BleedPower>();
            var segment = hp.GetParent()?.GetNodeOrNull<Control>(NodeName);
            if (bleed <= 0 || !hp.Visible || creature.CurrentHp <= 0)
            {
                if (segment != null) segment.Visible = false;
                return;
            }

            segment ??= MakeSegment(hp);
            if (segment == null) return;

            // The plain HP fill currently ends where Poison (if any) begins. Bleed takes the next chunk to its left.
            float max = MaxFgWidth(__instance);
            float right = hp.OffsetRight;                       // offsets are measured from the bar's right edge
            float hpWidth = right + max;                        // width of the plain fill
            float bleedWidth = creature.MaxHp > 0 ? (float)bleed / creature.MaxHp * max : 0;
            float leftWidth = hpWidth - bleedWidth;

            segment.SelfModulate = BleedColor;
            segment.Visible = true;
            segment.OffsetRight = right;
            if (leftWidth <= 12f)
            {
                // Lethal (or close to it): the whole remaining bar is Bleed.
                segment.OffsetLeft = 0;
                hp.Visible = false;
            }
            else
            {
                int margin = segment is NinePatchRect patch ? patch.PatchMarginLeft : 0;
                segment.OffsetLeft = Math.Max(0f, leftWidth - margin);
                hp.OffsetRight = leftWidth - max;
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Bleed health bar failed: {e}");
        }
    }

    // A copy of the plain HP fill, so it has the same texture and shape, just recolored.
    private static Control? MakeSegment(Control hp)
    {
        if (hp.Duplicate() is not Control copy) return null;
        copy.Name = NodeName;
        copy.UniqueNameInOwner = false;
        foreach (var child in copy.GetChildren()) child.QueueFree();
        hp.AddSibling(copy);
        return copy;
    }
}
