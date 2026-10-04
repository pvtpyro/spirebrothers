using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim's Age of Empires Ages. Every combat starts in the Dark Age (0); Age Up advances to Feudal (1),
/// Castle (2), then Imperial (3). Stored per combat on the player's combat state by TimTracker.
/// </summary>
public static class Ages
{
    public const int Dark = 0, Feudal = 1, Castle = 2, Imperial = 3;

    public static int Get(Player? player)
    {
        var state = player?.PlayerCombatState;
        return state == null ? Dark : TimTracker.AgeField.Get(state);
    }

    /// <summary>Advances one Age (or straight to <paramref name="to"/>), never past Imperial or backwards.</summary>
    public static void AgeUp(Player player, int? to = null)
    {
        var state = player.PlayerCombatState;
        if (state == null) return;
        int now = TimTracker.AgeField.Get(state);
        int next = Math.Min(Imperial, Math.Max(now, to ?? now + 1));
        TimTracker.AgeField.Set(state, next);
        var tracker = player.Creature.GetPower<AgePower>();
        tracker?.Refresh();
    }

    /// <summary>"Dark Age", "Feudal Age", ... from powers.json.</summary>
    public static string Name(int age) =>
        new LocString("powers", "SPIREBROTHERS-AGE_POWER.age" + Math.Clamp(age, Dark, Imperial)).GetFormattedText();
}
