using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// One of Tim's Kids, standing on the ground by his feet like the Necrobinder's Osty (it's a pet).
/// Decorative and untargetable, like the vanilla Byrdpip pet: huge HP, no health bar, no moves.
/// The end-of-turn hits live in Mechanics/Kids.cs.
/// </summary>
public class Kid : CustomMonsterModel
{
    public override int MinInitialHp => 9999;
    public override int MaxInitialHp => 9999;
    public override bool IsHealthBarVisible => false;

    // Visuals are generated in code, so there's no scene file to preload.
    public override IEnumerable<string> AssetPaths => [];

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var nothing = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        nothing.FollowUpState = nothing;
        return new MonsterMoveStateMachine([nothing], nothing);
    }

    public override NCreatureVisuals CreateCustomVisuals()
    {
        int number = Math.Max(1, Kids.Count(Creature?.PetOwner));
        return NodeFactory<NCreatureVisuals>.CreateFromResource(KidArt.Placeholder(number));
    }
}

/// <summary>
/// Placeholder kid pictures drawn pixel by pixel until there's real art. Each of the eight gets a different
/// hair and shirt color. To use real art later, return a loaded Texture2D from Placeholder() instead.
/// </summary>
public static class KidArt
{
    private const int W = 56, H = 88;

    private static readonly Color[] Hair =
    [
        new("4a2c17"), new("c9a227"), new("8b3a1a"), new("2b1d14"),
        new("e0c070"), new("6b4226"), new("a0522d"), new("3d2b1f")
    ];

    private static readonly Color[] Shirts =
    [
        new("d9534f"), new("5bc0de"), new("5cb85c"), new("f0ad4e"),
        new("9b59b6"), new("e67e22"), new("3498db"), new("e84393")
    ];

    private static readonly Dictionary<int, Texture2D> Cache = new();

    public static Texture2D Placeholder(int number)
    {
        int i = (number - 1) % Hair.Length;
        if (Cache.TryGetValue(i, out var cached)) return cached;

        var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
        img.Fill(new Color(0, 0, 0, 0));
        var skin = new Color("f2c9a0");
        var dark = new Color("2a2a2a");

        Rect(img, 18, 70, 8, 18, new Color("34495e"));        // legs
        Rect(img, 30, 70, 8, 18, new Color("34495e"));
        Rect(img, 14, 40, 28, 32, Shirts[i]);                 // shirt
        Rect(img, 8, 42, 6, 20, skin);                         // arms
        Rect(img, 42, 42, 6, 20, skin);
        Disc(img, 28, 20, 16, Hair[i]);                        // hair
        Disc(img, 28, 23, 13, skin);                           // face
        Rect(img, 22, 20, 3, 3, dark);                         // eyes
        Rect(img, 31, 20, 3, 3, dark);
        Rect(img, 23, 29, 10, 2, new Color("8a3b2a"));        // smile

        var tex = ImageTexture.CreateFromImage(img);
        Cache[i] = tex;
        return tex;
    }

    private static void Rect(Image img, int x, int y, int w, int h, Color c)
    {
        for (int px = x; px < x + w; px++)
            for (int py = y; py < y + h; py++)
                if (px >= 0 && py >= 0 && px < W && py < H) img.SetPixel(px, py, c);
    }

    private static void Disc(Image img, int cx, int cy, int r, Color c)
    {
        for (int px = cx - r; px <= cx + r; px++)
            for (int py = cy - r; py <= cy + r; py++)
                if ((px - cx) * (px - cx) + (py - cy) * (py - cy) <= r * r && px >= 0 && py >= 0 && px < W && py < H)
                    img.SetPixel(px, py, c);
    }
}
