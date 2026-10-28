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
/// Huffman's cyber-mastiff, standing in front of him like the Necrobinder's Osty (it's a pet). Untargetable like Tim's
/// Kids: huge HP, no health bar, no moves of its own. Its bites live in Mechanics/Mastiff.cs. Bounces while idle and
/// hops forward when it bites (reusing the Kids' motion). Art: images/monsters/mastiff/mastiff.png (pixelgen).
/// </summary>
public class MastiffDog : CustomMonsterModel
{
    public static readonly string ArtPath = $"{MainFile.ResPath}/images/monsters/mastiff/mastiff.png";

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
        // Pixel art drawn small, shown big and crisp. Falls back to a Kid picture if the art is missing.
        Texture2D texture = ResourceLoader.Exists(ArtPath) ? GD.Load<Texture2D>(ArtPath) : KidArt.Placeholder(1);
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(texture);
        if (visuals.GetChildren().OfType<Sprite2D>().FirstOrDefault() is { } sprite)
        {
            sprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            if (ResourceLoader.Exists(ArtPath)) sprite.Scale = Vector2.One * Scale;
            sprite.TreeEntered += () => KidMotion.Bounce(sprite, 2);
        }
        return visuals;
    }

    /// <summary>The art is 40 x 28; this makes it about the size of a big dog next to him.</summary>
    private const float Scale = 3.2f;
}

/// <summary>The game spreads pets across their owner's body width; this puts the dog just in front of Huffman instead.</summary>
[HarmonyPatch]
public static class MastiffLayout
{
    private const float Forward = 95f, Down = 12f;

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
    [HarmonyPostfix]
    private static void AfterAddCreature(NCombatRoom __instance, Creature creature)
    {
        if (creature.Monster is not MastiffDog || creature.PetOwner == null) return;
        Place(__instance.GetCreatureNode(creature.PetOwner.Creature), __instance.GetCreatureNode(creature));
    }

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.PositionPlayersAndPets))]
    [HarmonyPostfix]
    private static void AfterPositionPlayersAndPets(List<NCreature> creatureNodes)
    {
        foreach (var dog in creatureNodes.Where(n => n.Entity.Monster is MastiffDog).ToList())
            Place(creatureNodes.FirstOrDefault(n => n.Entity == dog.Entity.PetOwner?.Creature), dog);
    }

    private static void Place(NCreature? owner, NCreature? dog)
    {
        try
        {
            if (owner == null || dog == null) return;
            dog.Position = new Vector2(owner.Position.X + Forward, owner.Position.Y + Down);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Mastiff layout failed: {e}");
        }
    }
}
