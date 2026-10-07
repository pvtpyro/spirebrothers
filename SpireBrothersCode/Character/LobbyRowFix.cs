using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Co-op character select: when a player picks a character, the game shakes their row in the player list, saving the
/// row's position first and putting it back there when the shake ends. If that happens right after the player joins,
/// before the list has laid itself out, the saved position is stale and the row lands on top of another one (seen on
/// the host's screen). After a shake finishes, this clears it and asks the list to lay itself out again.
/// </summary>
[HarmonyPatch(typeof(NRemoteLobbyPlayer), nameof(NRemoteLobbyPlayer._Process))]
public static class LobbyRowFix
{
    private static readonly AccessTools.FieldRef<NRemoteLobbyPlayer, ScreenPunchInstance?> Shake =
        AccessTools.FieldRefAccess<NRemoteLobbyPlayer, ScreenPunchInstance?>("_shake");

    [HarmonyPostfix]
    private static void AfterProcess(NRemoteLobbyPlayer __instance)
    {
        if (Shake(__instance) is not { IsDone: true }) return;
        __instance.CancelShake();   // clears the shake and the saved position
        (__instance.GetParent() as Container)?.QueueSort();
    }
}
