using MegaCrit.Sts2.Core.Events;
using SpireBrothers.SpireBrothersCode.Cards.Shared;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// Each player picks their own. Dad deep-fries the turkey, Mom does all the other cooking (and always the dishes), and
/// Dad's giant shop is where the boys end up. Turkey, seconds on pie (plus Food Coma), dishes with Mom, or Dad's shop.
/// </summary>
public class ThanksgivingWithMomAndDad : BrotherEvent
{
    public const int TurkeyMaxHp = 7, PieHealPercent = 30;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(DadsDeepFriedTurkey),
        Option(SecondsOnMomsPie),
        Option(HelpMomWithTheDishes),
        Option(OutInDadsShop),
    ];

    private async Task DadsDeepFriedTurkey()
    {
        await GainMaxHp(TurkeyMaxHp);
        Finish("DADS_DEEP_FRIED_TURKEY");
    }

    private async Task SecondsOnMomsPie()
    {
        await HealPercent(PieHealPercent);
        await AddToDeck<FoodComa>();
        Finish("SECONDS_ON_MOMS_PIE");
    }

    private async Task HelpMomWithTheDishes()
    {
        await RemoveCards(1);
        Finish("HELP_MOM_WITH_THE_DISHES");
    }

    private async Task OutInDadsShop()
    {
        await UpgradeChosenCards(1);
        Finish("OUT_IN_DADS_SHOP");
    }
}
