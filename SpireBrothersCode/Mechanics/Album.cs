using System.Text.Json;
using Godot;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// A body the Family Album can draw in a photo: its first idle frame, how tall it stands in combat (game units, so
/// Goliath still towers over David in the picture), and whether it tips over when beaten. The brothers, the friends,
/// every foe of ours, and the Mastiff have one; anything without it (vanilla characters and monsters) means the
/// photo is a real screenshot instead.
/// </summary>
public interface IPhotoArt
{
    string PhotoArt { get; }
    float PhotoHeight { get; }
    bool PhotoTipOver => true;
}

/// <summary>One body in a drawn photo.</summary>
public sealed class PhotoFigure
{
    public string Art { get; set; } = "";
    public float Height { get; set; }
    public bool Fallen { get; set; }
    public bool TipOver { get; set; } = true;
}

/// <summary>One polaroid: what happened, the caption written at the time, and either the bodies to draw or a screenshot.</summary>
public sealed class Snapshot
{
    /// <summary>ELITE, BOSS, VICTORY, or DEFEAT.</summary>
    public string Kind { get; set; } = "";
    /// <summary>When it was taken, in Unix seconds.</summary>
    public long Time { get; set; }
    public int Act { get; set; }
    public int Floor { get; set; }
    public string Caption { get; set; } = "";
    public List<PhotoFigure> Party { get; set; } = new();
    public List<PhotoFigure> Foes { get; set; } = new();
    /// <summary>Screenshot file name in the album's shots folder, or null for a drawn photo.</summary>
    public string? Shot { get; set; }
}

/// <summary>
/// The Family Album: snapshots from elite and boss wins, run victories, and defeats, kept across runs on this computer
/// (user://SpireBrothers/album/, next to the game's own saves; not the run save, so every co-op player keeps their own).
/// Taken by AlbumTracker, shown by AlbumScreen (the "Family Album" button on the main menu).
/// </summary>
public static class Album
{
    private const string Dir = "user://SpireBrothers/album";
    private const string IndexFile = Dir + "/album.json";
    internal const string ShotsDir = Dir + "/shots";

    /// <summary>Screenshots are saved this wide (the height follows the window's shape).</summary>
    private const int ShotWidth = 960;

    private static List<Snapshot>? _snapshots;

    /// <summary>Every snapshot, oldest first.</summary>
    public static IReadOnlyList<Snapshot> All => _snapshots ??= Load();

    private static List<Snapshot> Load()
    {
        try
        {
            if (!Godot.FileAccess.FileExists(IndexFile)) return new();
            using var file = Godot.FileAccess.Open(IndexFile, Godot.FileAccess.ModeFlags.Read);
            return JsonSerializer.Deserialize<List<Snapshot>>(file.GetAsText()) ?? new();
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't read the Family Album: {e}");
            return new();
        }
    }

    public static void Add(Snapshot snapshot)
    {
        try
        {
            var list = _snapshots ??= Load();
            list.Add(snapshot);
            DirAccess.MakeDirRecursiveAbsolute(Dir);
            using var file = Godot.FileAccess.Open(IndexFile, Godot.FileAccess.ModeFlags.Write);
            file.StoreString(JsonSerializer.Serialize(list));
            SnapshotToast.Show();
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't save a Family Album snapshot: {e}");
        }
    }

    /// <summary>Saves what's on screen right now (the last drawn frame) and returns its file name, or null.</summary>
    public static string? SaveScreenshot()
    {
        try
        {
            if (Engine.GetMainLoop() is not SceneTree tree) return null;
            var image = tree.Root.GetTexture().GetImage();
            if (image == null || image.IsEmpty()) return null;
            int height = (int)Math.Round(image.GetHeight() * (double)ShotWidth / image.GetWidth());
            image.Resize(ShotWidth, height, Image.Interpolation.Bilinear);
            DirAccess.MakeDirRecursiveAbsolute(ShotsDir);
            string name = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.jpg";
            return image.SaveJpg($"{ShotsDir}/{name}", 0.88f) == Error.Ok ? name : null;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't take a Family Album screenshot: {e}");
            return null;
        }
    }
}
