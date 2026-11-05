using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Danny's Titan (Mechanics/Titan): a pet like the Necrobinder's Osty, but with real HP, since it
/// takes the hits meant for him. He climbs in: it stands where he does and his body is hidden (TitanLayout). It drops in
/// from the sky when called, and shrugs off small hits (TitanArmorPower). No moves of its own; his
/// cards fight through it. Its own health bar is switched off (like the Mastiff); Mechanics/TitanHullDisplay draws a
/// labelled one above Danny's. Art: images/monsters/titan/titan.png if present, else drawn here (TitanArt).
/// </summary>
public class DannyTitan : CustomMonsterModel, Mechanics.IPhotoArt
{
    public static readonly string ArtPath = $"{MainFile.ResPath}/images/monsters/titan/titan.png";

    // Family Album photos (IPhotoArt): about as tall as it stands in combat.
    string Mechanics.IPhotoArt.PhotoArt => ArtPath;
    float Mechanics.IPhotoArt.PhotoHeight => TitanArt.H * Scale;

    // Set to the real hull by Titan.Call right after it's added.
    public override int MinInitialHp => Mechanics.TitanChassis.Atlas.Hull;
    public override int MaxInitialHp => Mechanics.TitanChassis.Atlas.Hull;

    // The game's bar for it never showed up in testing; TitanHullDisplay draws one instead. This also lets the mouse
    // through it to Danny.
    public override bool IsHealthBarVisible => false;

    public override IEnumerable<string> AssetPaths => [];

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var nothing = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        nothing.FollowUpState = nothing;
        return new MonsterMoveStateMachine([nothing], nothing);
    }

    public override NCreatureVisuals CreateCustomVisuals()
    {
        Texture2D texture = ArtFor(Mechanics.ChassisKind.Atlas);
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(texture);
        // BaseLib lays the picture out 1 pixel = 1 unit with the feet at the origin; scale it like the pixel bodies.
        Character.PixelBody.ScaleToHeight(visuals, texture.GetSize().Y, TitanArt.H * Scale);
        if (visuals.GetChildren().OfType<Sprite2D>().FirstOrDefault() is { } sprite)
        {
            sprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            sprite.TreeEntered += () => DropIn(sprite);
        }
        return visuals;
    }

    /// <summary>
    /// The chassis's picture: images/monsters/titan/titan.png (Atlas) or titan_stryder.png / titan_ogre.png if present,
    /// else the drawn placeholder (TitanArt).
    /// </summary>
    public static Texture2D ArtFor(Mechanics.ChassisKind kind)
    {
        string path = kind == Mechanics.ChassisKind.Atlas ? ArtPath : $"{MainFile.ResPath}/images/monsters/titan/titan_{kind.ToString().ToLowerInvariant()}.png";
        return ResourceLoader.Exists(path) ? GD.Load<Texture2D>(path) : TitanArt.Draw(kind);
    }

    /// <summary>
    /// The visuals are built before the Titan knows whose it is, always as the Atlas; once it's placed (TitanLayout),
    /// this swaps in the owner's chassis, keeping the same height on screen.
    /// </summary>
    public static void ShowChassis(NCreature node, Mechanics.ChassisKind kind)
    {
        if (kind == Mechanics.ChassisKind.Atlas || node.Visuals?.GetChildren().OfType<Sprite2D>().FirstOrDefault() is not { } sprite) return;
        var texture = ArtFor(kind);
        float oldHeight = sprite.Texture?.GetSize().Y ?? texture.GetSize().Y;
        sprite.Texture = texture;
        sprite.Scale *= oldHeight / texture.GetSize().Y;
    }

    /// <summary>The art is 46 x 60; this makes it a head taller than him.</summary>
    private const float Scale = 7.6f;

    // Standby for Titanfall: it falls out of the sky, lands hard, and settles into a slow idle sway.
    private static void DropIn(Sprite2D sprite)
    {
        try
        {
            float homeY = sprite.Position.Y;
            sprite.Position = new Vector2(sprite.Position.X, homeY - 1400);
            var tween = sprite.CreateTween();
            tween.TweenProperty(sprite, "position:y", homeY, 0.32).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
            tween.TweenProperty(sprite, "scale:y", sprite.Scale.Y * 0.9f, 0.07).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(sprite, "scale:y", sprite.Scale.Y, 0.16).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
            tween.TweenCallback(Callable.From(() => KidMotion.Bounce(sprite, 9)));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titanfall drop failed: {e}");
        }
    }
}

/// <summary>
/// Placeholder pixel Titans, drawn at load like the Kids, facing right, one per chassis (Mechanics/Chassis): the Atlas is a
/// Vanguard-style mech in steel and blue, the Stryder a lean one on long legs, the Ogre a wide armored block.
/// All three share the 46 x 60 canvas so they scale the same.
/// </summary>
public static class TitanArt
{
    public const int W = 46, H = 60;
    private static readonly Dictionary<Mechanics.ChassisKind, Texture2D> Cached = new();

    public static Texture2D Draw(Mechanics.ChassisKind kind = Mechanics.ChassisKind.Atlas)
    {
        if (Cached.TryGetValue(kind, out var cached)) return cached;
        var texture = ImageTexture.CreateFromImage(kind switch
        {
            Mechanics.ChassisKind.Stryder => Stryder(),
            Mechanics.ChassisKind.Ogre => Ogre(),
            _ => Atlas()
        });
        Cached[kind] = texture;
        return texture;
    }

    private static Image Atlas()
    {
        var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
        img.Fill(new Color(0, 0, 0, 0));
        var steel = new Color("8c96a3");
        var shade = new Color("5f6875");
        var dark = new Color("2b3038");
        var blue = new Color("3d74d6");
        var glow = new Color("8fd0ff");
        var orange = new Color("e0782b");

        // far arm and leg (shaded), behind the body
        Rect(img, 3, 22, 6, 13, shade); Rect(img, 2, 34, 7, 4, dark);         // far arm and fist
        Rect(img, 13, 40, 7, 16, shade); Rect(img, 11, 55, 10, 4, dark);      // far leg and foot
        // near leg
        Rect(img, 25, 40, 8, 16, steel); Rect(img, 24, 55, 12, 4, dark);
        Rect(img, 25, 46, 8, 2, shade);                                      // knee joint
        Rect(img, 13, 46, 7, 2, dark);
        // hips
        Rect(img, 10, 36, 25, 6, shade);
        // torso: a chunky box, the cockpit hatch facing the enemies
        Rect(img, 8, 15, 28, 22, steel);
        Rect(img, 8, 15, 28, 2, shade);
        Rect(img, 24, 18, 12, 13, shade);                                    // hatch
        Rect(img, 27, 21, 8, 2, glow);                                       // visor slit
        Rect(img, 10, 26, 12, 3, blue);                                      // blue stripe
        Rect(img, 10, 31, 3, 3, orange);                                     // warning light
        // head sensor with its one blue eye
        Rect(img, 18, 9, 9, 7, steel);
        Rect(img, 24, 11, 2, 2, glow);
        Rect(img, 18, 9, 9, 1, shade);
        // shoulders
        Rect(img, 4, 14, 8, 8, shade);
        Rect(img, 31, 15, 10, 8, steel); Rect(img, 31, 15, 10, 2, blue);
        // near arm holding the XO-16
        Rect(img, 34, 23, 6, 10, steel);
        Rect(img, 32, 31, 12, 5, dark);                                      // gun body
        Rect(img, 43, 32, 3, 2, dark);                                       // barrel
        Rect(img, 35, 32, 3, 1, orange);
        Outline(img, dark);
        return img;
    }

    // Lean and tall: thin limbs, a small cockpit high up, a long rifle.
    private static Image Stryder()
    {
        var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
        img.Fill(new Color(0, 0, 0, 0));
        var steel = new Color("a3adb8");
        var shade = new Color("6c7682");
        var dark = new Color("2b3038");
        var blue = new Color("3d74d6");
        var glow = new Color("8fd0ff");
        var yellow = new Color("e8c34a");

        // far arm and leg
        Rect(img, 8, 19, 4, 14, shade); Rect(img, 7, 32, 5, 3, dark);
        Rect(img, 16, 34, 4, 22, shade); Rect(img, 14, 55, 8, 4, dark);
        // near leg, long, with a backward knee
        Rect(img, 25, 34, 5, 11, steel); Rect(img, 27, 44, 5, 12, steel);
        Rect(img, 26, 44, 6, 2, shade); Rect(img, 25, 55, 10, 4, dark);
        // hips
        Rect(img, 14, 31, 18, 4, shade);
        // a narrow torso, the cockpit up front
        Rect(img, 12, 14, 19, 18, steel);
        Rect(img, 12, 14, 19, 2, shade);
        Rect(img, 23, 17, 8, 9, shade);
        Rect(img, 25, 19, 6, 2, glow);
        Rect(img, 13, 24, 9, 2, blue);
        Rect(img, 13, 28, 9, 2, yellow);                                      // racing stripe
        // a small head with a wide visor
        Rect(img, 17, 8, 8, 6, steel); Rect(img, 20, 10, 5, 2, glow);
        // thrusters on the back
        Rect(img, 7, 15, 5, 10, dark); Rect(img, 7, 25, 5, 2, yellow);
        // shoulders
        Rect(img, 9, 13, 6, 5, shade);
        Rect(img, 28, 13, 7, 5, steel); Rect(img, 28, 13, 7, 1, blue);
        // near arm and a long rifle
        Rect(img, 31, 18, 4, 11, steel);
        Rect(img, 29, 28, 15, 3, dark);
        Rect(img, 44, 29, 2, 1, dark);
        Outline(img, dark);
        return img;
    }

    // Wide and low: thick plated legs, a huge box of a body, big shoulder plates, a heavy cannon.
    private static Image Ogre()
    {
        var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
        img.Fill(new Color(0, 0, 0, 0));
        var steel = new Color("727c88");
        var shade = new Color("4c5460");
        var dark = new Color("23272e");
        var blue = new Color("3d74d6");
        var glow = new Color("8fd0ff");
        var red = new Color("c8463a");

        // far arm and leg
        Rect(img, 1, 24, 7, 14, shade); Rect(img, 0, 37, 9, 5, dark);
        Rect(img, 9, 44, 10, 12, shade); Rect(img, 7, 55, 14, 4, dark);
        // near leg, short and thick
        Rect(img, 24, 44, 11, 12, steel); Rect(img, 22, 55, 16, 4, dark);
        Rect(img, 24, 48, 11, 2, shade);
        // hips
        Rect(img, 7, 40, 30, 6, shade);
        // a big armored box, the hatch set deep
        Rect(img, 5, 17, 34, 25, steel);
        Rect(img, 5, 17, 34, 3, shade);
        Rect(img, 25, 21, 12, 13, shade);
        Rect(img, 28, 25, 8, 2, glow);
        Rect(img, 7, 30, 15, 3, blue);
        Rect(img, 7, 35, 15, 2, red);                                         // hazard plate
        Rect(img, 9, 35, 2, 2, dark); Rect(img, 14, 35, 2, 2, dark); Rect(img, 19, 35, 2, 2, dark);
        // a low head, sunk between the shoulders
        Rect(img, 17, 12, 11, 6, steel); Rect(img, 24, 14, 3, 2, glow);
        // shoulder plates
        Rect(img, 0, 15, 11, 10, shade); Rect(img, 0, 15, 11, 2, dark);
        Rect(img, 31, 15, 14, 10, steel); Rect(img, 31, 15, 14, 3, blue);
        // near arm and a heavy cannon
        Rect(img, 35, 25, 8, 9, steel);
        Rect(img, 31, 33, 15, 7, dark);
        Rect(img, 35, 35, 4, 2, red);
        Outline(img, dark);
        return img;
    }

    private static void Rect(Image img, int x, int y, int w, int h, Color c)
    {
        for (int px = x; px < x + w; px++)
            for (int py = y; py < y + h; py++)
                if (px >= 0 && py >= 0 && px < W && py < H) img.SetPixel(px, py, c);
    }

    // A one-pixel dark edge around the figure, so it reads against any background.
    private static void Outline(Image img, Color c)
    {
        var edge = new List<(int, int)>();
        for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
            {
                if (img.GetPixel(x, y).A > 0) continue;
                bool touches = (x > 0 && img.GetPixel(x - 1, y).A > 0) || (x < W - 1 && img.GetPixel(x + 1, y).A > 0)
                               || (y > 0 && img.GetPixel(x, y - 1).A > 0) || (y < H - 1 && img.GetPixel(x, y + 1).A > 0);
                if (touches) edge.Add((x, y));
            }
        foreach (var (x, y) in edge) img.SetPixel(x, y, c);
    }
}

/// <summary>
/// He climbs in: the Titan stands exactly where Danny does, and his own body is hidden while it stands (Show/Hide,
/// called from Mechanics/Titan). Its health bar is off, so hovering or targeting it reaches Danny.
/// </summary>
[HarmonyPatch]
public static class TitanLayout
{
    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
    [HarmonyPostfix]
    private static void AfterAddCreature(NCombatRoom __instance, Creature creature)
    {
        if (creature.Monster is not DannyTitan || creature.PetOwner == null) return;
        var node = __instance.GetCreatureNode(creature);
        try
        {
            if (node != null) DannyTitan.ShowChassis(node, Mechanics.TitanChassis.Of(creature.PetOwner).Kind);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titan chassis art failed: {e}");
        }
        Place(__instance.GetCreatureNode(creature.PetOwner.Creature), node);
    }

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.PositionPlayersAndPets))]
    [HarmonyPostfix]
    private static void AfterPositionPlayersAndPets(List<NCreature> creatureNodes)
    {
        foreach (var titan in creatureNodes.Where(n => n.Entity.Monster is DannyTitan).ToList())
            Place(creatureNodes.FirstOrDefault(n => n.Entity == titan.Entity.PetOwner?.Creature), titan);
    }

    private static void Place(NCreature? owner, NCreature? titan)
    {
        try
        {
            if (owner == null || titan == null) return;
            titan.Position = owner.Position;
            if (owner.Visuals != null) owner.Visuals.Visible = !titan.Entity.IsAlive;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titan layout failed: {e}");
        }
    }

    /// <summary>Shows (ejected) or hides (in the Titan) Danny's own body.</summary>
    public static void ShowPilot(Creature pilot, bool show)
    {
        try
        {
            if (NCombatRoom.Instance?.GetCreatureNode(pilot)?.Visuals is { } visuals) visuals.Visible = show;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Showing the pilot failed: {e}");
        }
    }
}
