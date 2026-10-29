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
/// Al and Pacino, Brandi's alpaca hunter pets from WoW, standing in front of her (pets, untargetable like Tim's Kids:
/// huge HP, no health bar, no moves of their own). What they do lives in Mechanics/Alpacas.cs. They bounce while idle and
/// hop when they act (the Kids' motion). Art: images/monsters/alpaca/&lt;al|pacino&gt;.png if present, else drawn here.
/// </summary>
public abstract class Alpaca : CustomMonsterModel, Mechanics.IPhotoArt
{
    protected abstract string ArtName { get; }
    /// <summary>Wool, a darker shade for the far legs and the topknot, and which Kid bounce timing to borrow.</summary>
    protected abstract Color Wool { get; }
    protected abstract int BounceNumber { get; }

    private string ArtPath => $"{MainFile.ResPath}/images/monsters/alpaca/{ArtName}.png";

    // Family Album photos (IPhotoArt): about as tall as they stand in combat.
    string Mechanics.IPhotoArt.PhotoArt => ArtPath;
    float Mechanics.IPhotoArt.PhotoHeight => AlpacaArt.H * Scale;

    public override int MinInitialHp => 9999;
    public override int MaxInitialHp => 9999;
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
        Texture2D texture = ResourceLoader.Exists(ArtPath) ? GD.Load<Texture2D>(ArtPath) : AlpacaArt.Draw(Wool);
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(texture);
        if (visuals.GetChildren().OfType<Sprite2D>().FirstOrDefault() is { } sprite)
        {
            sprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            sprite.Scale = Vector2.One * Scale;
            sprite.TreeEntered += () => KidMotion.Bounce(sprite, BounceNumber);
        }
        return visuals;
    }

    /// <summary>The art is 32 x 40; this makes them about chest-high next to her.</summary>
    private const float Scale = 3.4f;
}

/// <summary>Al: a cream alpaca. He spits.</summary>
public class AlpacaAl : Alpaca
{
    protected override string ArtName => "al";
    protected override Color Wool => new("efe3c8");
    protected override int BounceNumber => 3;
}

/// <summary>Pacino: a brown alpaca. He guards her.</summary>
public class AlpacaPacino : Alpaca
{
    protected override string ArtName => "pacino";
    protected override Color Wool => new("8a5a36");
    protected override int BounceNumber => 6;
}

/// <summary>Placeholder pixel alpacas, drawn at load like the Kids, facing right toward the enemies.</summary>
public static class AlpacaArt
{
    public const int W = 32, H = 40;
    private static readonly Dictionary<Color, Texture2D> Cache = new();

    public static Texture2D Draw(Color wool)
    {
        if (Cache.TryGetValue(wool, out var cached)) return cached;
        var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
        img.Fill(new Color(0, 0, 0, 0));
        var shade = wool.Darkened(0.18f);
        var dark = new Color("2a2320");
        var outline = wool.Darkened(0.55f);

        // far legs (shaded), then the near ones in front
        Rect(img, 9, 30, 3, 9, shade); Rect(img, 20, 30, 3, 9, shade);
        Rect(img, 6, 30, 3, 10, wool); Rect(img, 17, 30, 3, 10, wool);
        Rect(img, 6, 38, 3, 2, dark); Rect(img, 17, 38, 3, 2, dark);   // hooves
        Rect(img, 9, 38, 3, 2, dark); Rect(img, 20, 38, 3, 2, dark);
        // fluffy body
        Blob(img, 14, 26, 10, 6, wool);
        Blob(img, 5, 24, 3, 3, wool);                                 // tail tuft
        // long neck and head
        Rect(img, 20, 9, 6, 17, wool);
        Blob(img, 24, 8, 5, 4, wool);                                 // head
        Rect(img, 27, 8, 4, 3, wool);                                 // snout
        Blob(img, 22, 4, 4, 2, shade);                                // topknot
        Rect(img, 21, 1, 2, 4, wool); Rect(img, 25, 1, 2, 4, wool);   // ears
        img.SetPixel(21, 1, outline); img.SetPixel(25, 1, outline);
        // face: eye, nose, a little smile
        Rect(img, 26, 7, 2, 2, dark);
        img.SetPixel(26, 7, Colors.White);
        img.SetPixel(30, 9, dark);
        img.SetPixel(28, 11, outline); img.SetPixel(29, 11, outline);
        // fleece texture
        for (int y = 21; y < 31; y += 3)
            for (int x = 7 + (y % 2); x < 23; x += 4) img.SetPixel(x, y, shade);
        Outline(img, outline);

        var tex = ImageTexture.CreateFromImage(img);
        Cache[wool] = tex;
        return tex;
    }

    private static void Rect(Image img, int x, int y, int w, int h, Color c)
    {
        for (int px = x; px < x + w; px++)
            for (int py = y; py < y + h; py++)
                if (px >= 0 && py >= 0 && px < W && py < H) img.SetPixel(px, py, c);
    }

    private static void Blob(Image img, int cx, int cy, int rx, int ry, Color c)
    {
        for (int px = cx - rx; px <= cx + rx; px++)
            for (int py = cy - ry; py <= cy + ry; py++)
            {
                float dx = (px - cx) / (float)rx, dy = (py - cy) / (float)ry;
                if (dx * dx + dy * dy <= 1.05f && px >= 0 && py >= 0 && px < W && py < H) img.SetPixel(px, py, c);
            }
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

/// <summary>The game spreads pets across their owner's body width; this stands Pacino just in front of Brandi and Al beside him.</summary>
[HarmonyPatch]
public static class AlpacaLayout
{
    private const float PacinoForward = 95f, AlForward = 175f, Down = 10f;

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
    [HarmonyPostfix]
    private static void AfterAddCreature(NCombatRoom __instance, Creature creature)
    {
        if (creature.Monster is not Alpaca || creature.PetOwner == null) return;
        Place(__instance.GetCreatureNode(creature.PetOwner.Creature), __instance.GetCreatureNode(creature));
    }

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.PositionPlayersAndPets))]
    [HarmonyPostfix]
    private static void AfterPositionPlayersAndPets(List<NCreature> creatureNodes)
    {
        foreach (var alpaca in creatureNodes.Where(n => n.Entity.Monster is Alpaca).ToList())
            Place(creatureNodes.FirstOrDefault(n => n.Entity == alpaca.Entity.PetOwner?.Creature), alpaca);
    }

    private static void Place(NCreature? owner, NCreature? alpaca)
    {
        try
        {
            if (owner == null || alpaca == null) return;
            float forward = alpaca.Entity.Monster is AlpacaAl ? AlForward : PacinoForward;
            alpaca.Position = new Vector2(owner.Position.X + forward, owner.Position.Y + Down);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Alpaca layout failed: {e}");
        }
    }
}
