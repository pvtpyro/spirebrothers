using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Runs;
using SpireBrothers.SpireBrothersCode.Cards.Brandi;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// Brandi's event: a day at home with her pets. Each player picks one to spend the afternoon with: Peppi (shreds the
/// toilet paper, says "beep boop"), Felicity (gray fur on everything), Lily Bean (the pug, wants a walk), or Yertle (the
/// mean 22-year-old turtle). Shows up when Brandi is in the party.
/// </summary>
public class DayAtHome : BrotherEvent
{
    public const int FelicityMaxHp = 5, LilyBeanUpgrades = 2, YertleBite = 8;

    public override bool IsAllowed(IRunState runState) => runState.Players.Any(p => p.Character is Character.Brandi);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        ShowingCard<Peppi>(Option(PlayWithPeppi)),
        Option(BrushFelicity),
        Option(WalkLilyBean),
        Option(FeedYertle).ThatDoesDamage(YertleBite),
    ];

    private async Task PlayWithPeppi()
    {
        await AddToDeck<Peppi>();
        Finish("PLAY_WITH_PEPPI");
    }

    private async Task BrushFelicity()
    {
        await GainMaxHp(FelicityMaxHp);
        Finish("BRUSH_FELICITY");
    }

    private async Task WalkLilyBean()
    {
        await UpgradeRandomCards(LilyBeanUpgrades);
        Finish("WALK_LILY_BEAN");
    }

    private async Task FeedYertle()
    {
        await LoseHp(YertleBite);
        await RandomRelic();
        Finish("FEED_YERTLE");
    }
}
