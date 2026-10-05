using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Orbs;

namespace SpireBrothers.SpireBrothersCode.Orbs;

/// <summary>
/// One of Joshua's Verses, floating above him in an orb slot like the Defect's orbs (max 10, the orb slot limit).
/// It does nothing on its own: Song cards add them, Chorus cards spend them (removed quietly, never evoked).
/// See Mechanics/Verses.cs.
/// </summary>
public class VerseOrb : CustomOrbModel
{
    public override Color DarkenedColor => new("7a6a3a");
    public override decimal PassiveVal => 0;
    public override decimal EvokeVal => 0;

    public override Task Passive(PlayerChoiceContext choiceContext, Creature? target) => Task.CompletedTask;
    public override Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext) =>
        Task.FromResult<IEnumerable<Creature>>([]);

    // Placeholder look until there's art: a gold eighth note.
    public override Node2D CreateCustomSprite()
    {
        var gold = new Color("f5d27a");
        var root = new Node2D { Name = "VerseSprite" };
        root.AddChild(new Polygon2D { Polygon = Ellipse(new Vector2(-6, 12), 10, 7), Color = gold });       // note head
        root.AddChild(new Line2D { Points = [new Vector2(3, 11), new Vector2(3, -18)], Width = 4, DefaultColor = gold }); // stem
        root.AddChild(new Line2D { Points = [new Vector2(3, -18), new Vector2(13, -9), new Vector2(11, 0)], Width = 4, DefaultColor = gold }); // flag
        return root;
    }

    private static Vector2[] Ellipse(Vector2 center, float rx, float ry)
    {
        var points = new Vector2[20];
        for (int i = 0; i < points.Length; i++)
        {
            float a = Mathf.Tau * i / points.Length;
            points[i] = center + new Vector2(Mathf.Cos(a) * rx, Mathf.Sin(a) * ry);
        }
        return points;
    }
}

/// <summary>Verses aren't worth a number each, so hide the orb's passive/evoke labels for them.</summary>
[HarmonyPatch(typeof(NOrb), nameof(NOrb.UpdateVisuals))]
internal static class HideVerseOrbLabels
{
    private static void Postfix(NOrb __instance)
    {
        if (__instance.Model is not VerseOrb) return;
        var traverse = Traverse.Create(__instance);
        if (traverse.Field("_passiveLabel").GetValue() is Control passive) passive.Visible = false;
        if (traverse.Field("_evokeLabel").GetValue() is Control evoke) evoke.Visible = false;
    }
}
