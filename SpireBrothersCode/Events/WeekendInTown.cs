using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Runs;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// David flies out to see John and Huffman. Each player picks how the weekend goes: a raid night at John's, dinner at
/// Huffman's (he isn't going anywhere, so he fries everything), or David plans it on a budget. Shows up when David,
/// John, Huffman, or Brandi is in the party.
/// </summary>
public class WeekendInTown : BrotherEvent
{
    public const int RaidNightHp = 6, RaidNightUpgrades = 2, FryerMaxHp = 7, BudgetGold = 60;

    public override bool IsAllowed(IRunState runState) =>
        runState.Players.Any(p => p.Character is Character.David or Character.John or Character.Huffman or Character.Brandi);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(RaidNight).ThatDoesDamage(RaidNightHp),
        Option(DinnerAtHuffmans),
        Option(LetDavidPlanIt),
    ];

    private async Task RaidNight()
    {
        await LoseHp(RaidNightHp);
        await UpgradeChosenCards(RaidNightUpgrades);
        Finish("RAID_NIGHT");
    }

    private async Task DinnerAtHuffmans()
    {
        await GainMaxHp(FryerMaxHp);
        Finish("DINNER_AT_HUFFMANS");
    }

    private async Task LetDavidPlanIt()
    {
        await GainGold(BudgetGold);
        Finish("LET_DAVID_PLAN_IT");
    }
}
