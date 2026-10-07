using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// One of Tim's Kids, standing on the ground by his feet like the Necrobinder's Osty (it's a pet).
/// Decorative and untargetable, like the vanilla Byrdpip pet: huge HP, no health bar, no moves.
/// The end-of-turn hits live in Mechanics/Kids.cs. They bounce gently while idle and hop forward when they hit.
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
        int number = KidNumber();
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(KidArt.Placeholder(number));
        if (visuals.GetChildren().OfType<Sprite2D>().FirstOrDefault() is { } sprite)
            sprite.TreeEntered += () => KidMotion.Bounce(sprite, number);
        return visuals;
    }

    // Which Kid this is (1 = first one he gained), so they always come in the same order as Tim's real kids.
    private int KidNumber()
    {
        var kids = Creature?.PetOwner?.PlayerCombatState?.Pets.Where(p => p.Monster is Kid).ToList();
        if (kids == null) return 1;
        int i = Creature == null ? -1 : kids.IndexOf(Creature);
        return i >= 0 ? i + 1 : kids.Count + 1;
    }
}

/// <summary>The Kids' little animations: an idle bounce and a hop toward the enemies when they hit. Display only.</summary>
public static class KidMotion
{
    /// <summary>How high the idle bounce goes and how far the attack hop goes, in game units.</summary>
    private const float BounceHeight = 3f, HopForward = 22f, HopUp = 10f;

    public static void Bounce(Sprite2D sprite, int number)
    {
        float baseY = sprite.Position.Y;
        // Slightly different speeds so they don't all bob in step.
        double half = 0.42 + (number % 4) * 0.05;
        var tween = sprite.CreateTween().SetLoops();
        tween.TweenProperty(sprite, "position:y", baseY - BounceHeight, half).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(sprite, "position:y", baseY, half).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
    }

    /// <summary>A quick hop forward and back, so you can see which Kid is hitting.</summary>
    public static void Hop(Creature kid)
    {
        try
        {
            if (NCombatRoom.Instance?.GetCreatureNode(kid)?.Visuals is not { } visuals) return;
            // Remember where it stands, so a hop that starts mid-hop still lands back in place.
            if (!visuals.HasMeta("kid_home")) visuals.SetMeta("kid_home", visuals.Position);
            var home = visuals.GetMeta("kid_home").AsVector2();
            float x = home.X, y = home.Y;
            var tween = visuals.CreateTween();
            tween.TweenProperty(visuals, "position", new Vector2(x + HopForward, y - HopUp), 0.09).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(visuals, "position", new Vector2(x, y), 0.14).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Kid hop failed: {e}");
        }
    }
}

/// <summary>
/// Placeholder kid pictures drawn pixel by pixel until there's real art. They come in the order of Tim's real kids
/// (boy, girl, boy, girl, girl, boy, girl, boy): boys in shades of blue, girls in shades of pink with longer hair and a
/// bow, everyone smiling. To use real art later, return a loaded Texture2D from Placeholder() instead.
/// </summary>
public static class KidArt
{
    private const int W = 56, H = 88;

    private static readonly bool[] IsGirl = [false, true, false, true, true, false, true, false];

    private static readonly Color[] Hair =
    [
        new("4a2c17"), new("c9a227"), new("8b3a1a"), new("2b1d14"),
        new("e0c070"), new("6b4226"), new("a0522d"), new("3d2b1f")
    ];

    // Each kid's own shade: blues for the boys, pinks for the girls (in the order above).
    private static readonly Color[] Shirts =
    [
        new("3a7bd5"), new("f06292"), new("5aa9e6"), new("f8a5c2"),
        new("d94f8a"), new("2f5fa8"), new("ffb3d1"), new("7cc0f0")
    ];

    private static readonly Dictionary<int, Texture2D> Cache = new();

    public static Texture2D Placeholder(int number)
    {
        int i = (number - 1) % IsGirl.Length;
        if (Cache.TryGetValue(i, out var cached)) return cached;

        var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
        img.Fill(new Color(0, 0, 0, 0));
        var skin = new Color("f2c9a0");
        var dark = new Color("2a2a2a");
        var shirt = Shirts[i];
        bool girl = IsGirl[i];

        if (girl)
        {
            Rect(img, 20, 72, 6, 16, skin);                    // legs
            Rect(img, 30, 72, 6, 16, skin);
            Rect(img, 19, 84, 8, 4, dark);                     // shoes
            Rect(img, 29, 84, 8, 4, dark);
            Rect(img, 14, 40, 28, 18, shirt);                  // dress, flaring out at the bottom
            Rect(img, 12, 58, 32, 8, shirt);
            Rect(img, 10, 66, 36, 7, shirt.Darkened(0.12f));
        }
        else
        {
            Rect(img, 18, 70, 8, 18, new Color("34495e"));     // legs
            Rect(img, 30, 70, 8, 18, new Color("34495e"));
            Rect(img, 14, 40, 28, 32, shirt);                  // shirt
        }
        Rect(img, 8, 42, 6, 20, skin);                         // arms
        Rect(img, 42, 42, 6, 20, skin);

        Disc(img, 28, 20, 16, Hair[i]);                        // hair
        if (girl)
        {
            Rect(img, 11, 18, 7, 24, Hair[i]);                 // long hair down to the shoulders
            Rect(img, 38, 18, 7, 24, Hair[i]);
        }
        Disc(img, 28, 23, 13, skin);                           // face
        if (girl)
        {
            var bow = shirt.Darkened(0.25f);                   // a bow on top
            Rect(img, 34, 3, 5, 5, bow); Rect(img, 41, 3, 5, 5, bow); Rect(img, 39, 4, 2, 3, bow.Darkened(0.2f));
        }

        Rect(img, 22, 19, 3, 4, dark);                         // eyes, with a little shine
        Rect(img, 31, 19, 3, 4, dark);
        img.SetPixel(22, 19, Colors.White);
        img.SetPixel(31, 19, Colors.White);
        var cheeks = new Color("f49a9a");                      // rosy cheeks
        Rect(img, 18, 25, 3, 2, cheeks);
        Rect(img, 35, 25, 3, 2, cheeks);
        var mouth = new Color("8a3b2a");                       // a big smile, corners up
        Rect(img, 22, 26, 2, 2, mouth);
        Rect(img, 32, 26, 2, 2, mouth);
        Rect(img, 24, 28, 8, 2, mouth);

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
