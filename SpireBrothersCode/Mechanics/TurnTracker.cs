using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Remembers, per player and per combat, which Logic/Hands cards were played this turn (in order),
/// whether last turn mixed both, and a few counters other cards care about.
/// </summary>
public class TurnTracker() : CustomSingletonModel(HookType.Combat)
{
    public static readonly SpireField<PlayerCombatState, List<CardKeyword>> Sequence = new(() => new List<CardKeyword>());
    public static readonly SpireField<PlayerCombatState, bool> MixedLastTurn = new(() => false);
    public static readonly SpireField<PlayerCombatState, bool> WiredFiredThisTurn = new(() => false);
    public static readonly SpireField<PlayerCombatState, int> GearsPlayed = new(() => 0);

    private static List<CardKeyword> Seq(PlayerCombatState state)
    {
        var seq = Seq(state);
        if (seq == null) { seq = new List<CardKeyword>(); Sequence.Set(state, seq); }
        return seq;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var state = cardPlay.Card.Owner?.PlayerCombatState;
        if (state == null) return Task.CompletedTask;

        var seq = Seq(state);
        if (cardPlay.Card.Keywords.Contains(BrotherKeywords.Logic)) seq.Add(BrotherKeywords.Logic);
        if (cardPlay.Card.Keywords.Contains(BrotherKeywords.Hands)) seq.Add(BrotherKeywords.Hands);
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState;
        if (state == null) return Task.CompletedTask;

        var seq = Seq(state);
        MixedLastTurn.Set(state, seq.Contains(BrotherKeywords.Logic) && seq.Contains(BrotherKeywords.Hands));
        Sequence.Set(state, new List<CardKeyword>());
        WiredFiredThisTurn.Set(state, false);
        return Task.CompletedTask;
    }

    public static bool PlayedThisTurn(Player? player, CardKeyword kw)
    {
        var state = player?.PlayerCombatState;
        return state != null && Seq(state).Contains(kw);
    }

    public static int CountThisTurn(Player? player, CardKeyword kw)
    {
        var state = player?.PlayerCombatState;
        return state == null ? 0 : Seq(state).Count(k => k == kw);
    }

    /// <summary>True if the required tags appear in order (not necessarily back to back) this turn.</summary>
    public static bool MatchesCombo(Player? player, IReadOnlyList<CardKeyword> combo)
    {
        var state = player?.PlayerCombatState;
        if (state == null) return false;
        int i = 0;
        foreach (var kw in Seq(state))
        {
            if (i < combo.Count && kw == combo[i]) i++;
        }
        return i >= combo.Count;
    }
}
