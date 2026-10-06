using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using SpireBrothers.SpireBrothersCode.Cards;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim's Age of Empires Ages. The Age lasts the whole run: he starts in the Dark Age (0) and his Age Up card advances
/// one Age at a time (Feudal 1, Castle 2, Imperial 3), costing gold. Each Age gives all his cards +1 damage and
/// +1 Block, like AoE's blacksmith upgrades (TimTracker applies it). In exchange he can't Smith at rest sites
/// (TimRunTracker).
/// </summary>
public static class Ages
{
    public const int Dark = 0, Feudal = 1, Castle = 2, Imperial = 3;

    /// <summary>Damage and Block every one of his cards gains per Age.</summary>
    public const int BonusPerAge = 1;

    /// <summary>
    /// Saved with the run, and sent along with the player when a co-op partner joins or loads. Registered with BaseLib
    /// when this class is first touched, so MainFile.Initialize touches it.
    /// </summary>
    public static readonly SavedSpireField<Player, int> RunAge = new(() => Dark, "SpireBrothersAge");

    public static int Get(Player? player) => player == null ? Dark : RunAge.Get(player);

    /// <summary>Damage / Block bonus on each of the player's cards right now.</summary>
    public static int Bonus(Player? player) => Get(player) * BonusPerAge;

    /// <summary>
    /// Gold it costs to advance from <paramref name="age"/> to the next Age: 50, 100, then 150, or 40, 80, then 120
    /// with an upgraded Age Up. The energy cost never drops, so it stays a big turn to play.
    /// </summary>
    public static int GoldCost(int age, bool upgraded = false) => (upgraded ? 40 : 50) * (age + 1);

    /// <summary>The next Age's cost for this player, using the upgraded price if their deck has an upgraded Age Up.</summary>
    public static int GoldCost(Player player) =>
        GoldCost(Get(player), player.Deck.Cards.Any(c => c is Cards.Tim.AgeUp && c.IsUpgraded));

    /// <summary>Advances one Age, never past Imperial.</summary>
    public static void AgeUp(Player player)
    {
        RunAge.Set(player, Math.Min(Imperial, Get(player) + 1));
        AgeDisplay.Update(player);
        TimCard.RefreshHand(player);
    }

    /// <summary>"Dark Age", "Feudal Age", ...</summary>
    public static string Name(int age) =>
        new LocString("powers", $"SPIREBROTHERS-AGES.name{Math.Clamp(age, Dark, Imperial)}").GetFormattedText();
}
