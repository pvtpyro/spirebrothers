using MegaCrit.Sts2.Core.Events;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>Mom and Dad's lake. Each player picks: go fishing with Dad (a gamble), wakeboard, or lounge on the dock.</summary>
public class DayAtTheLake : BrotherEvent
{
    // Fishing: 40% potion, 30% Gold, 20% relic, 10% an old boot.
    public const int PotionOdds = 40, GoldOdds = 30, RelicOdds = 20, FishingGold = 60;
    public const int WakeboardHp = 8, WakeboardMaxHp = 5, DockHealPercent = 25;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(GoFishingWithDad),
        Option(Wakeboarding).ThatDoesDamage(WakeboardHp),
        Option(LoungeOnTheDock),
    ];

    private async Task GoFishingWithDad()
    {
        int roll = Rng.NextInt(100);
        if (roll < PotionOdds)
        {
            await RandomPotion();
            Finish("CAUGHT_POTION");
        }
        else if (roll < PotionOdds + GoldOdds)
        {
            await GainGold(FishingGold);
            Finish("CAUGHT_GOLD");
        }
        else if (roll < PotionOdds + GoldOdds + RelicOdds)
        {
            await RandomRelic();
            Finish("CAUGHT_RELIC");
        }
        else
        {
            Finish("CAUGHT_BOOT");
        }
    }

    private async Task Wakeboarding()
    {
        await LoseHp(WakeboardHp);
        await GainMaxHp(WakeboardMaxHp);
        Finish("WAKEBOARDING");
    }

    private async Task LoungeOnTheDock()
    {
        await HealPercent(DockHealPercent);
        Finish("LOUNGE_ON_THE_DOCK");
    }
}
