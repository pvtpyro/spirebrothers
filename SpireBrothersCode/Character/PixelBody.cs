using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Animated pixel-art bodies from frames/&lt;anim&gt;_&lt;n&gt;.png (drawn by art_originals/pixelgen). Shared by the brothers
/// (BrotherCharacter) and Captain Blockbeard. The game cues idle (loops), attack, cast (skills and powers), hit and
/// dead; BaseLib plays the matching animation on the AnimatedSprite2D it finds in the body. Only idle is required; a
/// missing cue just keeps the current one.
/// </summary>
public static class PixelBody
{
    private static readonly (string Name, float Fps, bool Loop)[] Anims =
    [
        ("idle", 6, true), ("attack", 14, false), ("cast", 10, false), ("hit", 12, false), ("dead", 8, false),
        ("rest", 5, true)   // sitting by the campfire; only rest sites play it (see StandIns)
    ];

    /// <summary>
    /// Loads every animation found through <paramref name="framePath"/> ("frames/idle_0.png" -> full res path).
    /// Null if there's no idle animation.
    /// </summary>
    public static SpriteFrames? LoadFrames(Func<string, string> framePath, float restFps, out Texture2D? first)
    {
        first = null;
        var frames = new SpriteFrames();
        frames.RemoveAnimation("default");
        foreach (var (name, fps, loop) in Anims)
        {
            for (int i = 0; ResourceLoader.Exists(framePath($"frames/{name}_{i}.png")); i++)
            {
                var tex = GD.Load<Texture2D>(framePath($"frames/{name}_{i}.png"));
                if (!frames.HasAnimation(name))
                {
                    frames.AddAnimation(name);
                    frames.SetAnimationSpeed(name, name == "rest" ? restFps : fps);
                    frames.SetAnimationLoop(name, loop);
                }
                frames.AddFrame(name, tex);
                first ??= tex;
            }
        }
        if (!frames.HasAnimation("idle")) return null;

        // The files are dead_N.png, but BaseLib's death cue only looks for an animation named "Dead", "Die" or "die"
        // (case-sensitive), so a lowercase "dead" never played. Register it as "die".
        if (frames.HasAnimation("dead")) frames.RenameAnimation("dead", "die");

        // Revive (e.g. a Lizard Tail save) plays the death in reverse, so he gets back up.
        if (frames.HasAnimation("die"))
        {
            frames.AddAnimation("revive");
            frames.SetAnimationSpeed("revive", 10);
            frames.SetAnimationLoop("revive", false);
            for (int i = frames.GetFrameCount("die") - 1; i >= 0; i--) frames.AddFrame("revive", frames.GetFrameTexture("die", i));
        }
        return frames;
    }

    /// <summary>
    /// The combat body, <paramref name="bodyHeight"/> units tall. Frames are drawn facing right (like the brothers);
    /// <paramref name="faceLeft"/> mirrors them for enemies. <paramref name="maxIntentHeight"/> keeps a tall body's intent from
    /// floating up behind the top bar: the intent sits no higher than that many units above his feet.
    /// <paramref name="tipOver"/> false keeps the body upright on death (lions and walls lie down or crumble in their own frames).
    /// </summary>
    public static NCreatureVisuals AnimatedBody(SpriteFrames frames, Texture2D first, float bodyHeight, bool faceLeft = false,
        float? maxIntentHeight = null, bool tipOver = true)
    {
        // Let BaseLib lay out the hitbox and markers from the first frame, then hide that still picture and play
        // the animation in its place. The frames are pixel art, so scale them up without blurring.
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(first);
        var still = visuals.GetChildren().OfType<Sprite2D>().First();
        still.SelfModulate = Colors.Transparent;
        var anim = new AnimatedSprite2D
        {
            Name = "AnimatedBody",
            SpriteFrames = frames,
            Autoplay = "idle",
            FlipH = faceLeft,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        still.AddChild(anim);
        // One-shot cues (attack, cast, hit, revive) fall back to idle; die stays on its last frame.
        anim.AnimationFinished += () =>
        {
            if (anim.Animation != "die") anim.Play("idle");
        };

        // The die frames only slump, so he also tips over backwards onto the ground, and gets back up on revive.
        // Moving the pivot from the middle of the picture to his feet (without moving the picture) makes him fall
        // over where he stands.
        float half = first.GetSize().Y / 2;
        anim.Position += new Vector2(0, half);
        anim.Offset = new Vector2(0, -half);
        float lyingAngle = faceLeft ? Mathf.Pi / 2 : -Mathf.Pi / 2;
        anim.AnimationChanged += () =>
        {
            float lying = tipOver && anim.Animation == "die" ? lyingAngle : 0f;
            if (Mathf.IsEqualApprox(anim.Rotation, lying)) return;
            anim.CreateTween().TweenProperty(anim, "rotation", lying, anim.Animation == "die" ? 0.45 : 0.3)
                .SetTrans(Tween.TransitionType.Quad).SetEase(anim.Animation == "die" ? Tween.EaseType.In : Tween.EaseType.Out);
        };
        ScaleToHeight(visuals, first.GetSize().Y, bodyHeight);
        if (maxIntentHeight is { } cap && visuals.GetNodeOrNull<Marker2D>("IntentPos") is { } intent)
            intent.Position = intent.Position with { Y = Mathf.Max(intent.Position.Y, -cap) };
        return visuals;
    }

    // BaseLib lays the image out at 1 pixel = 1 unit with the feet at the origin; scaling every child about the
    // origin keeps the feet planted and moves the hitbox, intent and talk markers along with the picture.
    private const float IntentGap = 70f;

    public static void ScaleToHeight(NCreatureVisuals visuals, float height, float bodyHeight)
    {
        if (height <= 0) return;
        float s = bodyHeight / height;
        foreach (var child in visuals.GetChildren())
        {
            switch (child)
            {
                case Sprite2D sprite:
                    sprite.Position *= s;
                    sprite.Scale *= s;
                    break;
                // BaseLib puts IntentPos a fixed 70 units above the head, and the game hangs the orbs (Joshua's
                // Verses) off it. Keep that gap fixed instead of scaling it, or small pixel-art frames scaled 5x
                // push the orbs off the top of the screen.
                case Marker2D intent when intent.Name.ToString() == "IntentPos":
                    intent.Position = (intent.Position + new Vector2(0, IntentGap)) * s - new Vector2(0, IntentGap);
                    break;
                case Node2D node:
                    node.Position *= s;
                    break;
                case Control control:
                    control.Position *= s;
                    control.Size *= s;
                    break;
            }
        }
    }
}
