using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using SpireBrothers.SpireBrothersCode.Powers;

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
        var seq = Sequence.Get(state);
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
        RefreshTracker(cardPlay.Card.Owner);
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState;
        if (state == null) return;

        var seq = Seq(state);
        MixedLastTurn.Set(state, seq.Contains(BrotherKeywords.Logic) && seq.Contains(BrotherKeywords.Hands));
        Sequence.Set(state, new List<CardKeyword>());
        WiredFiredThisTurn.Set(state, false);

        // Daniel gets the Train of Thought tracker icon on his first turn of each combat.
        if (player.Character is SpireBrothers.SpireBrothersCode.Character.Daniel
            && player.Creature.GetPower<TrainOfThoughtPower>() == null)
        {
            await PowerCmd.Apply<TrainOfThoughtPower>(choiceContext, player.Creature, 1, player.Creature, null, silent: true);
        }
        RefreshTracker(player);
    }

    private static void RefreshTracker(Player? player) =>
        player?.Creature?.GetPower<TrainOfThoughtPower>()?.Refresh();

    /// <summary>The keyword's display name ("Logic" / "Hands"), from card_keywords.json.</summary>
    public static string KeywordName(CardKeyword kw)
    {
        var id = kw == BrotherKeywords.Logic ? "SPIREBROTHERS-LOGIC" : "SPIREBROTHERS-HANDS";
        return new LocString("card_keywords", id + ".title").GetFormattedText();
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

    /// <summary>How many steps of the combo have been entered, in order (not necessarily back to back), this turn.</summary>
    public static int ComboProgress(Player? player, IReadOnlyList<CardKeyword> combo)
    {
        var state = player?.PlayerCombatState;
        if (state == null) return 0;
        int i = 0;
        foreach (var kw in Seq(state))
        {
            if (i < combo.Count && kw == combo[i]) i++;
        }
        return i;
    }

    public static bool MatchesCombo(Player? player, IReadOnlyList<CardKeyword> combo) =>
        ComboProgress(player, combo) >= combo.Count;
}
