using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// Shared (the party votes on every answer). Captain Blockbeard challenges you to an insult duel: he throws an insult,
/// you pick the comeback that actually answers it (1 right, 2 from other insults). Best 2 of 3. Win: a relic. Lose:
/// pay him off, or draw swords and fight him as an elite.
/// Pairs are SPIREBROTHERS-INSULT_DUEL.pairs.N.insult / .comeback in events.json; any number works (at least 3).
/// </summary>
public class InsultDuel : BrotherEvent
{
    public const int WinsNeeded = 2, Choices = 3, PayUpGold = 125;
    private const string Round = "ROUND";

    public override bool IsShared => true;

    // Act 3 only, like his elite fight: by then the brothers have Insult and Comeback cards to answer him with.
    public override bool IsAllowed(MegaCrit.Sts2.Core.Runs.IRunState runState) => base.IsAllowed(runState) && runState.CurrentActIndex == 2;

    private List<int> _pairOrder = [];
    private int _round, _wins, _losses;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(AcceptTheDuel),
        Option(WalkAway),
    ];

    private static int PairCount()
    {
        int n = 0;
        while (LocString.Exists("events", $"SPIREBROTHERS-INSULT_DUEL.pairs.{n}.insult")) n++;
        return n;
    }

    private static LocString Pair(int pair, string part) => new("events", $"SPIREBROTHERS-INSULT_DUEL.pairs.{pair}.{part}");

    private Task AcceptTheDuel()
    {
        _pairOrder = Enumerable.Range(0, PairCount()).ToList().StableShuffle(Rng);
        _round = _wins = _losses = 0;
        ShowRound(null);
        return Task.CompletedTask;
    }

    private Task WalkAway()
    {
        Finish("WALK_AWAY");
        return Task.CompletedTask;
    }

    /// <summary>Blockbeard's next insult and three comebacks. <paramref name="result"/> is how the last round went.</summary>
    private void ShowRound(string? result)
    {
        int pair = _pairOrder[_round % _pairOrder.Count];
        var wrong = _pairOrder.Where(p => p != pair).ToList().StableShuffle(Rng).Take(Choices - 1);
        var answers = wrong.Select(p => (Pair: p, Right: false)).Append((Pair: pair, Right: true)).ToList().StableShuffle(Rng);

        var page = PageDescription(Round);
        page.Add("Result", result == null ? "" : L10NLookup($"{Id.Entry}.pages.{result}.description").GetFormattedText() + "\n\n");
        page.Add("Insult", Pair(pair, "insult").GetFormattedText());
        page.Add("Wins", _wins.ToString());
        page.Add("Losses", _losses.ToString());

        var retort = L10NLookup($"{Id.Entry}.pages.{Round}.options.RETORT.description");
        SetEventState(page, answers.Select(a => Option(a.Right ? RightComeback : WrongComeback, Pair(a.Pair, "comeback"), retort)).ToList());
    }

    private Task RightComeback() { _wins++; return NextRound("TOUCHE"); }

    private Task WrongComeback() { _losses++; return NextRound("MISS"); }

    private Task NextRound(string result)
    {
        _round++;
        if (_wins >= WinsNeeded) return Victory();
        if (_losses >= WinsNeeded)
        {
            SetEventState(PageDescription("DEFEAT"), [Option(PayUp, "DEFEAT"), Option(DrawSwords, "DEFEAT")]);
            return Task.CompletedTask;
        }
        ShowRound(result);
        return Task.CompletedTask;
    }

    private async Task Victory()
    {
        await RandomRelic();
        Finish("VICTORY");
    }

    private async Task PayUp()
    {
        int gold = Math.Min(PayUpGold, Owner!.Gold);
        if (gold > 0) await PlayerCmd.LoseGold(gold, Owner, GoldLossType.Stolen);
        Finish("PAY_UP");
    }

    // Lost the duel and won't pay: fight him for real (normal elite rewards).
    private Task DrawSwords()
    {
        EnterCombatWithoutExitingEvent(ModelDb.Encounter<BlockbeardElite>().ToMutable(), [], shouldResumeAfterCombat: false);
        return Task.CompletedTask;
    }
}
