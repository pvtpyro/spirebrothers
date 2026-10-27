using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using SpireBrothers.SpireBrothersCode.Powers;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Joshua's D&amp;D dice. Roll a d20 on the run RNG (so co-op stays in sync) and show it over his head.
/// 1 is a Fumble (half effect), 20 is a Natural 20 (double effect). Advantage rolls twice and keeps the higher,
/// Lucky rerolls the first 1 each turn, Lucky d20 makes the first Roll each combat a 20, Dungeon Master adds +4.
/// </summary>
public static class Dice
{
    public const int Sides = 20;

    public static async Task<int> Roll(PlayerChoiceContext ctx, Player player)
    {
        var rng = player.RunState.Rng.CombatTargets;
        int Throw() => rng.NextInt(Sides) + 1;

        int roll = Throw();
        if (player.Creature.GetPower<AdvantagePower>() is { } advantage)
        {
            roll = Math.Max(roll, Throw());
            if (advantage.Amount <= 1) await PowerCmd.Remove(advantage);
            else await PowerCmd.ModifyAmount(ctx, advantage, -1, null, null);
        }
        if (roll == 1 && (player.Creature.GetPower<LuckyPower>()?.TryUse() ?? false)) roll = Throw();
        if (player.GetRelic<LuckyD20>()?.TryUse() ?? false) roll = Sides;
        roll = Math.Min(Sides, roll + (player.Creature.GetPowerAmount<DungeonMasterPower>() * DungeonMasterPower.Bonus));

        var key = roll >= Sides ? "nat20" : roll <= 1 ? "fumble" : "roll";
        var text = new LocString("powers", $"SPIREBROTHERS-DICE.{key}");
        text.Add("Roll", roll);
        Banter.Bubble(player.Creature, text.GetFormattedText());
        return roll;
    }

    /// <summary>How much a Roll scales a card's effect: half on a 1, double on a 20.</summary>
    public static decimal Multiplier(int roll) => roll <= 1 ? 0.5m : roll >= Sides ? 2m : 1m;
}
