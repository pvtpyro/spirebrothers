using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Harmony. Whenever you play a Chorus, apply Amount Heartstrings to ALL enemies. (Healed ALL players until 2026-10-26.)</summary>
public class HarmonyPower : BrothersPower, IChorusListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnChorus(PlayerChoiceContext choiceContext, Player player, int verses)
    {
        var enemies = CombatState?.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies == null || enemies.Count == 0) return;
        Flash();
        await PowerCmd.Apply<HeartstringsPower>(choiceContext, enemies, Amount, Owner, null);
    }
}
