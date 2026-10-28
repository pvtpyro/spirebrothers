using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Base for the regular-fight foes: a pixel-art body from images/monsters/&lt;ArtFolder&gt;/frames/ (drawn by
/// art_originals/mobgen, pixelgen's Mobs.cs), mirrored to face the party. Each subclass sets its art, size, and moves.
/// </summary>
public abstract class PixelMonster : CustomMonsterModel
{
    protected abstract string ArtFolder { get; }
    /// <summary>On-screen height in game units (a brother is about 300).</summary>
    protected abstract float BodyHeight { get; }
    /// <summary>False for foes whose death frames do the falling themselves (smoke puffs, bone piles, flipped bugs).</summary>
    protected virtual bool TipOver => false;

    private string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/{ArtFolder}/{file}";

    // The first path is the placeholder Spine scene, which these bodies don't use.
    public override IEnumerable<string> AssetPaths => base.AssetPaths.Skip(1);

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        try
        {
            if (PixelBody.LoadFrames(ArtPath, 5, out var first) is { } frames && first != null)
                return PixelBody.AnimatedBody(frames, first, BodyHeight, faceLeft: true, tipOver: TipOver);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build the {ArtFolder}'s body: {e}");
        }
        return null;
    }
}
