using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Darv's Dusty Tome gives an upgraded Ancient card from your character's pool. The brothers have no Ancient cards,
/// so vanilla picked from an empty list, crashed, and left that player's Darv event with no options (a soft lock).
/// When the pool has no Ancient cards (and BaseLib's ITomeCard has none either), give a Rare from the pool instead.
/// Uses the player's Rewards RNG like vanilla, so co-op stays in sync.
/// </summary>
[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
public static class DustyTomeFallback
{
    [HarmonyPrefix]
    [HarmonyAfter("BaseLib")]
    private static bool UseRareIfNoAncients(DustyTome __instance, Player player, bool __runOriginal)
    {
        if (!__runOriginal) return false; // BaseLib already picked an ITomeCard.
        try
        {
            var cards = player.Character.CardPool
                .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).ToList();
            if (cards.Any(c => c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c)))
                return true;

            var rares = cards.Where(c => c.Rarity == CardRarity.Rare).ToList();
            if (rares.Count == 0) return true;
            __instance.AncientCard = player.PlayerRng.Rewards.NextItem(rares)!.Id;
            return false;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Dusty Tome fallback failed: {e}");
            return true;
        }
    }
}
