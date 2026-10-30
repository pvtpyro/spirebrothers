using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Shows Bleed (David) and Ember (Brandi) on the health bar the way the game shows Poison: the HP each will take at the
/// start of the creature's turn is drawn in its own color at the end of the bar, or the whole bar if it's lethal.
/// They sit just left of any Poison segment (Bleed, then Ember), since all of them hit at the start of the turn.
/// Display only.
/// </summary>
[HarmonyPatch]
public static class BleedHealthBar
{
    /// <summary>Colors on the bar. Plain HP is bright red (F1373E), Poison is green, Doom is purple.</summary>
    private static readonly Color BleedColor = new("8E0E26"), EmberColor = new("F08A1C");

    private const string BleedNode = "SpireBrothersBleedForeground", EmberNode = "SpireBrothersEmberForeground";

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

            float max = MaxFgWidth(__instance);
            Segment(creature, hp, max, BleedNode, BleedColor, creature.GetPowerAmount<BleedPower>());
            Segment(creature, hp, max, EmberNode, EmberColor, creature.GetPowerAmount<EmberPower>());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Bleed/Ember health bar failed: {e}");
        }
    }

    /// <summary>Draws one damage-over-time chunk just left of whatever already ends the plain HP fill.</summary>
    private static void Segment(Creature creature, Control hp, float max, string nodeName, Color color, int amount)
    {
        var segment = hp.GetParent()?.GetNodeOrNull<Control>(nodeName);
        if (amount <= 0 || !hp.Visible || creature.CurrentHp <= 0)
        {
            if (segment != null) segment.Visible = false;
            return;
        }

        segment ??= MakeSegment(hp, nodeName);
        if (segment == null) return;

        // The plain HP fill currently ends where Poison (or the last segment) begins. This takes the next chunk.
        float right = hp.OffsetRight;                       // offsets are measured from the bar's right edge
        float hpWidth = right + max;                        // width of the plain fill
        float width = creature.MaxHp > 0 ? (float)amount / creature.MaxHp * max : 0;
        float leftWidth = hpWidth - width;

        segment.SelfModulate = color;
        segment.Visible = true;
        segment.OffsetRight = right;
        if (leftWidth <= 12f)
        {
            // Lethal (or close to it): the whole remaining bar is this segment.
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

    // A copy of the plain HP fill, so it has the same texture and shape, just recolored.
    private static Control? MakeSegment(Control hp, string nodeName)
    {
        if (hp.Duplicate() is not Control copy) return null;
        copy.Name = nodeName;
        copy.UniqueNameInOwner = false;
        foreach (var child in copy.GetChildren()) child.QueueFree();
        hp.AddSibling(copy);
        return copy;
    }
}
