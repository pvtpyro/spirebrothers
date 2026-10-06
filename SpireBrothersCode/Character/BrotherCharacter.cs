using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Shared base for the four brothers. Uses a brother's own art from images/characters/&lt;name&gt;/ when the file
/// exists, and falls back to the borrowed vanilla character (PlaceholderID) when it doesn't, so art can be
/// dropped in one piece at a time. File names are listed in assets.md.
/// </summary>
public abstract class BrotherCharacter : PlaceholderCharacterModel
{
    /// <summary>Folder name under images/characters/, e.g. "tim".</summary>
    protected abstract string ArtFolder { get; }

    private string ArtPath(string file) => $"{MainFile.ResPath}/images/characters/{ArtFolder}/{file}";

    private string? ArtIfPresent(string file)
    {
        var path = ArtPath(file);
        return ResourceLoader.Exists(path) ? path : null;
    }

    /// <summary>How tall a brother's body stands in combat, in game units. Tweak per brother if one looks off.</summary>
    protected virtual float BodyHeight => 400f;

    // Combat body. Either an animated body from frames/ (see below), or body.png (a single still image, feet at the
    // bottom edge). Any resolution works: it's scaled to BodyHeight here, so art can be exported large and stays
    // sharp on big screens.
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        try
        {
            if (LoadFrames(out var first) is { } frames) return AnimatedBody(frames, first!);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build {ArtFolder}'s animated body: {e}");
        }

        var body = ArtIfPresent("body.png");
        if (body == null) return null;
        try
        {
            var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(body);
            ScaleToHeight(visuals, GD.Load<Texture2D>(body)?.GetSize().Y ?? 0);
            return visuals;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build {ArtFolder}'s body from {body}: {e}");
            return null;
        }
    }

    // Animated body: frames/<anim>_<n>.png, numbered from 0, all the same size, feet at the bottom edge. The game
    // cues idle (loops), attack, cast (skills and powers), hit and dead; BaseLib plays the matching animation on
    // the AnimatedSprite2D it finds in the body. Only idle is required; a missing cue just keeps the current one.
    private static readonly (string Name, float Fps, bool Loop)[] Anims =
    [
        ("idle", 6, true), ("attack", 14, false), ("cast", 10, false), ("hit", 12, false), ("dead", 8, false),
        ("rest", 5, true)   // sitting by the campfire; only rest sites play it (see StandIns)
    ];

    private SpriteFrames? LoadFrames(out Texture2D? first)
    {
        first = null;
        var frames = new SpriteFrames();
        frames.RemoveAnimation("default");
        foreach (var (name, fps, loop) in Anims)
        {
            for (int i = 0; ResourceLoader.Exists(ArtPath($"frames/{name}_{i}.png")); i++)
            {
                var tex = GD.Load<Texture2D>(ArtPath($"frames/{name}_{i}.png"));
                if (!frames.HasAnimation(name))
                {
                    frames.AddAnimation(name);
                    frames.SetAnimationSpeed(name, fps);
                    frames.SetAnimationLoop(name, loop);
                }
                frames.AddFrame(name, tex);
                first ??= tex;
            }
        }
        if (!frames.HasAnimation("idle")) return null;

        // Revive (e.g. a Lizard Tail save) plays the death in reverse, so he gets back up.
        if (frames.HasAnimation("dead"))
        {
            frames.AddAnimation("revive");
            frames.SetAnimationSpeed("revive", 10);
            frames.SetAnimationLoop("revive", false);
            for (int i = frames.GetFrameCount("dead") - 1; i >= 0; i--) frames.AddFrame("revive", frames.GetFrameTexture("dead", i));
        }
        return frames;
    }

    /// <summary>
    /// A looping animation of this brother, feet at the origin, <paramref name="height"/> units tall, for screens
    /// outside combat (see StandIns): <paramref name="anim"/> if he has it (e.g. "rest" at the campfire), otherwise idle.
    /// Null if he has no frames/ yet.
    /// </summary>
    public AnimatedSprite2D? CreateIdleSprite(float height, string anim = "idle")
    {
        try
        {
            if (LoadFrames(out var first) is not { } frames || first == null) return null;
            if (!frames.HasAnimation(anim)) anim = "idle";
            float h = first.GetSize().Y;
            var sprite = new AnimatedSprite2D
            {
                SpriteFrames = frames,
                Autoplay = anim,
                Offset = new Vector2(0, -h / 2),
                Scale = Vector2.One * (height / h),
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest
            };
            sprite.Play(anim);
            return sprite;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build {ArtFolder}'s idle sprite: {e}");
            return null;
        }
    }

    private NCreatureVisuals AnimatedBody(SpriteFrames frames, Texture2D first)
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
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        still.AddChild(anim);
        // One-shot cues (attack, cast, hit, revive) fall back to idle; dead stays on its last frame.
        anim.AnimationFinished += () =>
        {
            if (anim.Animation != "dead") anim.Play("idle");
        };

        // The dead frames only slump, so he also tips over backwards onto the ground, and gets back up on revive.
        // Moving the pivot from the middle of the picture to his feet (without moving the picture) makes him fall
        // over where he stands.
        float half = first.GetSize().Y / 2;
        anim.Position += new Vector2(0, half);
        anim.Offset = new Vector2(0, -half);
        anim.AnimationChanged += () =>
        {
            float lying = anim.Animation == "dead" ? -Mathf.Pi / 2 : 0f;
            if (Mathf.IsEqualApprox(anim.Rotation, lying)) return;
            anim.CreateTween().TweenProperty(anim, "rotation", lying, anim.Animation == "dead" ? 0.45 : 0.3)
                .SetTrans(Tween.TransitionType.Quad).SetEase(anim.Animation == "dead" ? Tween.EaseType.In : Tween.EaseType.Out);
        };
        ScaleToHeight(visuals, first.GetSize().Y);
        return visuals;
    }

    // BaseLib lays the image out at 1 pixel = 1 unit with the feet at the origin; scaling every child about the
    // origin keeps the feet planted and moves the hitbox, intent and talk markers along with the picture.
    private const float IntentGap = 70f;

    private void ScaleToHeight(NCreatureVisuals visuals, float height)
    {
        if (height <= 0) return;
        float s = BodyHeight / height;
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

    // Character select button: select.png and select_locked.png (132 x 195).
    public override string? CustomCharacterSelectIconPath => ArtIfPresent("select.png") ?? base.CustomCharacterSelectIconPath;
    public override string? CustomCharacterSelectLockedIconPath => ArtIfPresent("select_locked.png") ?? base.CustomCharacterSelectLockedIconPath;

    // Map token: map_marker.png (128 x 128).
    public override string? CustomMapMarkerPath => ArtIfPresent("map_marker.png") ?? base.CustomMapMarkerPath;

    // Character icon: icon.png (128 x 128), falling back to the map marker. Co-op shows it on the map vote markers and
    // the player list, with icon_outline.png (the same shape in flat white, a few pixels bigger) behind it.
    private string? IconArt => ArtIfPresent("icon.png") ?? ArtIfPresent("map_marker.png");
    public override string? CustomIconTexturePath => IconArt ?? base.CustomIconTexturePath;
    public override string? CustomIconOutlineTexturePath => ArtIfPresent("icon_outline.png") ?? base.CustomIconOutlineTexturePath;

    // Top-left icon during a run. Reuses the borrowed character's icon scene so the layout matches, with our pictures
    // swapped in for its character_icon textures.
    public override Control? CustomIcon
    {
        get
        {
            if (IconArt == null) return base.CustomIcon;
            try
            {
                var icon = GD.Load<PackedScene>(CustomIconPath).Instantiate<Control>();
                var art = GD.Load<Texture2D>(IconArt);
                var outline = CustomIconOutlineTexturePath is { } o ? GD.Load<Texture2D>(o) : null;
                SwapIconTextures(icon, art, outline);
                return icon;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Couldn't build {ArtFolder}'s icon: {e}");
                return base.CustomIcon;
            }
        }
    }

    private static void SwapIconTextures(Node node, Texture2D art, Texture2D? outline)
    {
        switch (node)
        {
            case TextureRect rect when rect.Texture?.ResourcePath.Contains("character_icon_") == true:
                rect.Texture = rect.Texture.ResourcePath.Contains("_outline") ? outline ?? rect.Texture : art;
                break;
            case Sprite2D sprite when sprite.Texture?.ResourcePath.Contains("character_icon_") == true:
                sprite.Texture = sprite.Texture.ResourcePath.Contains("_outline") ? outline ?? sprite.Texture : art;
                break;
        }
        foreach (var child in node.GetChildren()) SwapIconTextures(child, art, outline);
    }
}
