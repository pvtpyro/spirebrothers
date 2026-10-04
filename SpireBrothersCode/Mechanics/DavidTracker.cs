using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Counts David's Rant cards played this turn and this combat, per player.</summary>
public class DavidTracker() : CustomSingletonModel(HookType.Combat)
{
    public static readonly SpireField<PlayerCombatState, int> RantsThisTurnField = new(() => 0);
    public static readonly SpireField<PlayerCombatState, int> RantsThisCombatField = new(() => 0);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var state = cardPlay.Card.Owner?.PlayerCombatState;
        if (state == null || !cardPlay.Card.Keywords.Contains(BrotherKeywords.Rant)) return Task.CompletedTask;
        RantsThisTurnField.Set(state, RantsThisTurnField.Get(state) + 1);
        RantsThisCombatField.Set(state, RantsThisCombatField.Get(state) + 1);
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState;
        if (state != null) RantsThisTurnField.Set(state, 0);
        return Task.CompletedTask;
    }

    /// <summary>Rants already played this turn. Read inside OnPlay, it doesn't count the card being played.</summary>
    public static int RantsThisTurn(Player? player)
    {
        var state = player?.PlayerCombatState;
        return state == null ? 0 : RantsThisTurnField.Get(state);
    }

    public static int RantsThisCombat(Player? player)
    {
        var state = player?.PlayerCombatState;
        return state == null ? 0 : RantsThisCombatField.Get(state);
    }
}
