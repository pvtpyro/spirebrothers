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
    // Small icon used for the orb's flash effect and tooltip. Placeholder gold note; replace the file to restyle it.
    public override string CustomIconPath => $"{MainFile.ResPath}/images/orbs/verse_orb.png";

    // Borrowed vanilla sound: the Regent's guiding-star chime when a Verse appears.
    public override string CustomChannelSfx => "event:/sfx/characters/regent/regent_guiding_star";
    public override string CustomPassiveSfx => "event:/sfx/characters/regent/regent_guiding_star";
    public override string CustomEvokeSfx => "event:/sfx/characters/regent/regent_guiding_star";

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

/// <summary>
/// The game's orb display assumes every orb sprite is a Spine animation and errors (stopping the redraw halfway)
/// when it isn't. For Verses this does the same setup without the Spine step, and hides the number labels,
/// since a Verse isn't worth a number. Every other orb goes through the game's normal code.
/// </summary>
[HarmonyPatch(typeof(NOrb), nameof(NOrb.UpdateVisuals))]
internal static class VerseOrbVisuals
{
    private static bool Prefix(NOrb __instance)
    {
        if (__instance.Model is not VerseOrb verse) return true;
        if (!__instance.IsNodeReady() || !MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsInProgress) return false;

        var t = Traverse.Create(__instance);
        bool isLocal = t.Field("_isLocal").GetValue<bool>();
        if (t.Field("_sprite").GetValue() is not Node2D)
        {
            var sprite = verse.CreateSprite();
            t.Field("_visualContainer").GetValue<Control>()?.AddChild(sprite);
            sprite.Position = Vector2.Zero;
            t.Field("_sprite").SetValue(sprite);
            sprite.Scale = Vector2.Zero;
            __instance.CreateTween().TweenProperty(sprite, "scale", Vector2.One, 0.5)
                .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        }
        if (t.Field("_outline").GetValue() is CanvasItem outline) outline.Visible = false;
        if (t.Field("_flashParticle").GetValue() is CpuParticles2D flash)
        {
            flash.Visible = true;
            flash.Texture = verse.Icon;
        }
        if (t.Field("_labelContainer").GetValue() is CanvasItem labels) labels.Visible = isLocal;
        if (t.Field("_passiveLabel").GetValue() is CanvasItem passive) passive.Visible = false;
        if (t.Field("_evokeLabel").GetValue() is CanvasItem evoke) evoke.Visible = false;
        if (!isLocal) __instance.Modulate = verse.DarkenedColor;
        return false;
    }
}
