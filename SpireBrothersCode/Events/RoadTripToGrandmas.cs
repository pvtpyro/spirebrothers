using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Runs;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// Shared (the party votes). The Idaho crew drives south to Grandma's, stopping at Daniel's on the way.
/// Page 1 is the drive, page 2 is Grandma's house.
/// </summary>
public class RoadTripToGrandmas : BrotherEvent
{
    public const int RestAreaHealPercent = 20, StraightThroughHp = 5, StraightThroughGold = 50, TuneUpCards = 2;
    public const int CookingMaxHp = 6, GrandmaGold = 75, StoryChoices = 3;
    private const string Grandmas = "GRANDMAS";

    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(StopAtEveryRestArea),
        Option(DriveStraightThrough).ThatDoesDamage(StraightThroughHp),
        Option(SwingByDaniels),
    ];

    private async Task StopAtEveryRestArea()
    {
        await HealPercent(RestAreaHealPercent);
        ArriveAtGrandmas("STOP_AT_EVERY_REST_AREA");
    }

    private async Task DriveStraightThrough()
    {
        await LoseHp(StraightThroughHp);
        await GainGold(StraightThroughGold);
        ArriveAtGrandmas("DRIVE_STRAIGHT_THROUGH");
    }

    private async Task SwingByDaniels()
    {
        await UpgradeRandomCards(TuneUpCards);
        ArriveAtGrandmas("SWING_BY_DANIELS");
    }

    private void ArriveAtGrandmas(string routePage) =>
        SetEventState(PageDescription(routePage),
        [
            Option(GrandmasCooking, Grandmas),
            Option(GrandmaSlipsYouSomeCash, Grandmas),
            Option(ListenToGrandmasStories, Grandmas),
        ]);

    private async Task GrandmasCooking()
    {
        await GainMaxHp(CookingMaxHp);
        Finish("GRANDMAS_COOKING");
    }

    private async Task GrandmaSlipsYouSomeCash()
    {
        await GainGold(GrandmaGold);
        Finish("GRANDMA_SLIPS_YOU_SOME_CASH");
    }

    // Pick 1 of 3 cards from your own pool, already upgraded.
    private async Task ListenToGrandmasStories()
    {
        var owner = Owner!;
        var options = CardCreationOptions.ForNonCombatWithDefaultOdds([owner.Character.CardPool], c => DuoCards.Allowed(c, owner));
        var cards = CardFactory.CreateForReward(owner, StoryChoices, options).ToList();
        foreach (var result in cards.Where(r => r.Card.IsUpgradable)) CardCmd.Upgrade(result.Card);
        var prefs = new CardSelectorPrefs(L10NLookup($"{Id.Entry}.pages.{Grandmas}.selectionScreenPrompt"), 1) { Cancelable = false };
        var card = (await CardSelectCmd.FromSimpleGridForRewards(new BlockingPlayerChoiceContext(), cards, owner, prefs)).FirstOrDefault();
        if (card != null) CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        Finish("LISTEN_TO_GRANDMAS_STORIES");
    }
}
