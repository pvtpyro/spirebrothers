using MegaCrit.Sts2.Core.Events;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>Shared (the party votes). The family hikes a lot: summit, easy loop, or turn back for ice cream.</summary>
public class FamilyHike : BrotherEvent
{
    public const int SummitHp = 10, SummitUpgrades = 2, LoopHealPercent = 15, LoopMaxHp = 4, IceCreamHealPercent = 10;

    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(GoForTheSummit).ThatDoesDamage(SummitHp),
        Option(TakeTheEasyLoop),
        Option(TurnBackForIceCream),
    ];

    private async Task GoForTheSummit()
    {
        await LoseHp(SummitHp);
        await UpgradeChosenCards(SummitUpgrades);
        Finish("GO_FOR_THE_SUMMIT");
    }

    private async Task TakeTheEasyLoop()
    {
        await HealPercent(LoopHealPercent);
        await GainMaxHp(LoopMaxHp);
        Finish("TAKE_THE_EASY_LOOP");
    }

    private async Task TurnBackForIceCream()
    {
        await HealPercent(IceCreamHealPercent);
        await RandomPotion();
        Finish("TURN_BACK_FOR_ICE_CREAM");
    }
}
