using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Ringing Ears (Ceremonial Beast) allows only 1 card play per turn and counts auto-plays too, so Tim's Script
/// replays at the start of his turn used up that one card. This is the vanilla check with Script replays left out:
/// replays still happen, and you still get your one real card.
/// </summary>
[HarmonyPatch(typeof(RingingPower), nameof(RingingPower.ShouldPlay))]
public static class RingingScriptFix
{
    [HarmonyPrefix]
    private static bool IgnoreScriptReplays(RingingPower __instance, CardModel card, ref bool __result)
    {
        try
        {
            var owner = __instance.Owner;
            if (card.Owner.Creature != owner || card.Affliction is not Ringing || ScriptPower.IsReplay(card))
            {
                __result = true;
                return false;
            }
            __result = !CombatManager.Instance.History.CardPlaysStarted.Any(e =>
                e.HappenedThisTurn(__instance.CombatState) && e.CardPlay.Card.Owner.Creature == owner &&
                !ScriptPower.IsReplay(e.CardPlay.Card));
            return false;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Ringing Script fix failed, using vanilla: {e}");
            return true;
        }
    }
}
