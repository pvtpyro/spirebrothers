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
public abstract class BrotherCharacter : PlaceholderCharacterModel, Mechanics.IPhotoArt
{
    /// <summary>Folder name under images/characters/, e.g. "tim".</summary>
    protected abstract string ArtFolder { get; }

    /// <summary>Frames per second for his campfire loop ("rest"); a brother with a slower story there can lower it.</summary>
    protected virtual float RestFps => 5;

    private string ArtPath(string file) => $"{MainFile.ResPath}/images/characters/{ArtFolder}/{file}";

    // Family Album photos (IPhotoArt).
    string Mechanics.IPhotoArt.PhotoArt => ArtPath("frames/idle_0.png");
    float Mechanics.IPhotoArt.PhotoHeight => BodyHeight;

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
            if (LoadFrames(out var first) is { } frames) return NarrowBounds(AnimatedBody(frames, first!));
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

    // Animated body: frames/<anim>_<n>.png, numbered from 0, all the same size, feet at the bottom edge (see PixelBody).
    private SpriteFrames? LoadFrames(out Texture2D? first) => PixelBody.LoadFrames(ArtPath, RestFps, out first);

    private NCreatureVisuals AnimatedBody(SpriteFrames frames, Texture2D first) => PixelBody.AnimatedBody(frames, first, BodyHeight);

    private void ScaleToHeight(NCreatureVisuals visuals, float height) => PixelBody.ScaleToHeight(visuals, height, BodyHeight);

    /// <summary>How much of the frame's width counts as his body. The figure fills well under the whole frame.</summary>
    protected virtual float BoundsWidth => 0.6f;

    // The game spaces players by their Bounds width, and with 4 players the back row sits further left. The full
    // 64-wide frame scaled up is 320 units, much wider than a vanilla character, which pushed the 4th player off
    // the left edge of the screen. Narrowing Bounds (kept centred) also trims the hitbox and health bar to the figure.
    private NCreatureVisuals NarrowBounds(NCreatureVisuals visuals)
    {
        var bounds = visuals.GetNodeOrNull<Control>("%Bounds")
                     ?? visuals.GetChildren().OfType<Control>().FirstOrDefault(c => c.Name.ToString() == "Bounds");
        if (bounds == null) return visuals;
        float cut = bounds.Size.X * (1 - BoundsWidth);
        bounds.Position += new Vector2(cut / 2, 0);
        bounds.Size -= new Vector2(cut, 0);
        return visuals;
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

    // Character select button: select.png and select_locked.png (132 x 195).
    public override string? CustomCharacterSelectIconPath => ArtIfPresent("select.png") ?? base.CustomCharacterSelectIconPath;
    public override string? CustomCharacterSelectLockedIconPath => ArtIfPresent("select_locked.png") ?? base.CustomCharacterSelectLockedIconPath;

    // Character select background: scenes/char_select/<name>_bg.tscn, a full-screen TextureRect showing
    // images/characters/<name>/select_bg.png (480 x 270 pixel art, scaled up crisp; drawn by art_originals/pixelgen).
    public override string? CustomCharacterSelectBg
    {
        get
        {
            var scene = $"{MainFile.ResPath}/scenes/char_select/{ArtFolder}_bg.tscn";
            return ResourceLoader.Exists(scene) ? scene : base.CustomCharacterSelectBg;
        }
    }

    // Map token: map_marker.png (128 x 128).
    public override string? CustomMapMarkerPath => ArtIfPresent("map_marker.png") ?? base.CustomMapMarkerPath;

    // Character icon: icon.png (128 x 128), falling back to the map marker. Co-op shows it on the map vote markers and
    // the player list, with icon_outline.png (the same shape in flat white, a few pixels bigger) behind it.
    private string? IconArt => ArtIfPresent("icon.png") ?? ArtIfPresent("map_marker.png");
    public override string? CustomIconTexturePath => IconArt ?? base.CustomIconTexturePath;
    public override string? CustomIconOutlineTexturePath => ArtIfPresent("icon_outline.png") ?? base.CustomIconOutlineTexturePath;

    // Co-op treasure room hands (pointing while picking, then rock / paper / scissors): hands/<move>.png, 422 x 1200
    // like vanilla, drawn by pixelgen. Without them the game borrows the placeholder character's arms.
    public override string? CustomArmPointingTexturePath => ArtIfPresent("hands/point.png") ?? base.CustomArmPointingTexturePath;
    public override string? CustomArmRockTexturePath => ArtIfPresent("hands/rock.png") ?? base.CustomArmRockTexturePath;
    public override string? CustomArmPaperTexturePath => ArtIfPresent("hands/paper.png") ?? base.CustomArmPaperTexturePath;
    public override string? CustomArmScissorsTexturePath => ArtIfPresent("hands/scissors.png") ?? base.CustomArmScissorsTexturePath;

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
