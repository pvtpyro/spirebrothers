using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// The shop and rest sites only take a Spine scene per character, so the brothers borrow their vanilla character's
/// (PlaceholderCharacterModel). These patches keep that scene, so everything it does (thought bubbles, selection,
/// flipping) still works, but hide its Spine body and put the brother's own animation in its place: idle in the shop,
/// sitting on a log ("rest": Daniel tinkers, David flips a coin, Joshua strums, Tim toasts a marshmallow) at rest sites.
/// </summary>
[HarmonyPatch]
public static class StandIns
{
    private const string NodeName = "BrotherStandIn";

    /// <summary>Height in the shop. Combat bodies are 400; tweak if he looks too big or small next to the merchant.</summary>
    private const float ShopHeight = 400f;

    /// <summary>
    /// Rest-site height, the same for all four. (It used to come from the borrowed vanilla character's hitbox, which
    /// made them slightly different sizes.)
    /// </summary>
    private const float RestSiteHeight = 620f;

    /// <summary>
    /// Where his feet go at the campfire, as a fraction of his height from his seat. The scene has two arched logs, and
    /// each player's spot (Character_1..4: front-left, front-right, back-left, back-right) is the point on a log where
    /// vanilla characters sit. In the rest frames the bottom of his seat is about 6px behind and 10px above the frame's
    /// bottom-center (of 80), so the feet go that far forward and down. Tweak these if they float or sink.
    /// The old version offset from the borrowed vanilla body and pushed him toward the fire, which put the back-row
    /// seats in 4-player co-op out on bare ground.
    /// </summary>
    private static readonly Vector2 RestSeat = new(0.075f, 0.125f);

    private static readonly AccessTools.FieldRef<NMerchantRoom, List<Player>> Players =
        AccessTools.FieldRefAccess<NMerchantRoom, List<Player>>("_players");

    private static readonly AccessTools.FieldRef<NMerchantRoom, List<NMerchantCharacter>> PlayerVisuals =
        AccessTools.FieldRefAccess<NMerchantRoom, List<NMerchantCharacter>>("_playerVisuals");

    // The room builds one character per player, in the same order as its player list.
    [HarmonyPatch(typeof(NMerchantRoom), nameof(NMerchantRoom._Ready))]
    [HarmonyPostfix]
    private static void AfterMerchantRoomReady(NMerchantRoom __instance)
    {
        try
        {
            var players = Players(__instance);
            var visuals = PlayerVisuals(__instance);
            for (int i = 0; i < Math.Min(players.Count, visuals.Count); i++)
                if (players[i].Character is BrotherCharacter brother) StandIn(visuals[i], brother, ShopHeight, "idle", null);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Shop stand-in failed: {e}");
        }
    }

    [HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter._Ready))]
    [HarmonyPostfix]
    private static void AfterRestSiteCharacterReady(NRestSiteCharacter __instance)
    {
        try
        {
            if (__instance.Player?.Character is not BrotherCharacter brother) return;
            // The node itself sits on the player's spot (the room puts it at its container's origin).
            StandIn(__instance, brother, RestSiteHeight, "rest", RestSeat * RestSiteHeight);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Rest site stand-in failed: {e}");
        }
    }

    // Characters on the far side of the campfire are mirrored; mirror ours with them.
    [HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter.FlipX))]
    [HarmonyPostfix]
    private static void AfterRestSiteFlip(NRestSiteCharacter __instance)
    {
        if (__instance.GetNodeOrNull<Node2D>(NodeName) is not { } sprite) return;
        sprite.Scale = new Vector2(-sprite.Scale.X, sprite.Scale.Y);
        sprite.Position = new Vector2(-sprite.Position.X, sprite.Position.Y);
    }

    /// <param name="feet">Where his feet go, relative to the host node (unmirrored); null puts them where the vanilla body stood.</param>
    private static void StandIn(Node host, BrotherCharacter brother, float height, string anim, Vector2? feet)
    {
        var spines = host.GetChildren().OfType<Node2D>().Where(n => n.GetClass() == "SpineSprite").ToList();
        if (spines.Count == 0 || host.GetNodeOrNull(NodeName) != null) return;
        if (brother.CreateIdleSprite(height, anim) is not { } sprite) return;

        sprite.Name = NodeName;
        bool mirrored = spines[0].Scale.X < 0;
        sprite.Position = feet is { } f ? new Vector2(mirrored ? -f.X : f.X, f.Y) : spines[0].Position;
        if (mirrored) sprite.Scale = new Vector2(-sprite.Scale.X, sprite.Scale.Y);
        foreach (var spine in spines) spine.Visible = false;
        host.AddChild(sprite);
        MainFile.Logger.Info($"Stand-in for {brother.Id.Entry} in {host.GetType().Name}: at {sprite.Position}, {height} tall");
    }
}
