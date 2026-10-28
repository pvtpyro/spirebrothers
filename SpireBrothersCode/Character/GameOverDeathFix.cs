using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Losing (or giving up) outside a fight: the Defeat screen builds a fresh body for each player and plays "die" through
/// Spine, which our pixel bodies don't use, so they stood there alive. After it gathers the bodies, any of ours still
/// standing plays its death frames and tips over, like in combat. In combat the death cue already played, and the
/// screen also holds the enemies (which must stay up), so this only runs on the map, at rest sites, and in shops.
/// </summary>
[HarmonyPatch(typeof(NGameOverScreen), "MoveCreaturesToDifferentLayerAndDisableUi")]
public static class GameOverDeathFix
{
    private static readonly AccessTools.FieldRef<NGameOverScreen, Control> Container =
        AccessTools.FieldRefAccess<NGameOverScreen, Control>("_creatureContainer");

    [HarmonyPostfix]
    private static void Postfix(NGameOverScreen __instance)
    {
        try
        {
            if (NCombatRoom.Instance != null) return;
            foreach (var node in Container(__instance).FindChildren("*", nameof(AnimatedSprite2D), recursive: true, owned: false))
            {
                if (node is not AnimatedSprite2D sprite || sprite.SpriteFrames?.HasAnimation("die") != true) continue;
                if (sprite.Animation == "die") continue;
                sprite.Play("die");
                // The combat body tips itself over when "die" starts; the shop stand-in (pivot already at its feet) needs a push.
                if (sprite.Name != "AnimatedBody")
                    sprite.CreateTween().TweenProperty(sprite, "rotation", -Mathf.Pi / 2, 0.45)
                        .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Game over death pose failed: {e}");
        }
    }
}
