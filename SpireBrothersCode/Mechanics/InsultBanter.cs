using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Monkey Island insult cards (the ones that apply Insulted). Playing one makes the player shout an insult.</summary>
public interface IInsultCard { }

/// <summary>
/// When a player plays an insult card, a speech bubble with an insult pops up over them. Lines come from
/// SPIREBROTHERS-INSULT_LINES.0 .. Count-1 in powers.json (all original, never the game's lines), dealt like a shuffled
/// deck so none repeats until every one has been used. The shuffle is seeded from the run and advanced by card plays,
/// which every co-op client sees in the same order, so everyone sees the same line. Display only.
/// </summary>
public class InsultBanter() : CustomSingletonModel(HookType.Combat)
{
    /// <summary>How many SPIREBROTHERS-INSULT_LINES.N keys exist. Update this when adding lines.</summary>
    public const int Count = 40;

    private sealed class Counter { public int Next; }
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<IRunState, Counter> Dealt = new();

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        try
        {
            var card = cardPlay.Card;
            if (card is not IInsultCard || card.Owner?.Creature is not { IsAlive: true } speaker) return Task.CompletedTask;
            var run = card.Owner.RunState;
            int dealt = Dealt.GetOrCreateValue(run).Next++;
            int line = LineAt(run.Rng.CombatTargets.Seed, dealt);
            TalkCmd.Play(new LocString("powers", $"SPIREBROTHERS-INSULT_LINES.{line}"), speaker, VfxColor.White, VfxDuration.Standard);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Insult banter failed: {e}");
        }
        return Task.CompletedTask;
    }

    // The n-th line dealt: each pass through the list is its own shuffle, so lines only repeat after all 40.
    private static int LineAt(uint seed, int n)
    {
        int pass = n / Count;
        var order = Enumerable.Range(0, Count).ToArray();
        new Random(unchecked((int)seed + pass * 7919)).Shuffle(order);
        return order[n % Count];
    }
}
