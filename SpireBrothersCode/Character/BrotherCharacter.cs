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

    // Combat body: body.png (a single still image, feet at the bottom edge). Any resolution works: it's scaled to
    // BodyHeight here, so art can be exported large and stays sharp on big screens.
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        var body = ArtIfPresent("body.png");
        if (body == null) return null;
        try
        {
            var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(body);
            ScaleToHeight(visuals, body);
            return visuals;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build {ArtFolder}'s body from {body}: {e}");
            return null;
        }
    }

    // BaseLib lays the image out at 1 pixel = 1 unit with the feet at the origin; scaling every child about the
    // origin keeps the feet planted and moves the hitbox, intent and talk markers along with the picture.
    private void ScaleToHeight(NCreatureVisuals visuals, string texturePath)
    {
        var height = GD.Load<Texture2D>(texturePath)?.GetSize().Y ?? 0;
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
}
