using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Cards.Tim;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim's per-combat state: his Age, cards played this turn (Mark and Execute, Automation Suite), and Script.
/// When a card with Script is played (or Snap to Grid / Automation Suite gives it Script), it's queued in
/// ScriptPower, which auto-plays a temporary duplicate of it at the start of the next turn.
/// </summary>
public class TimTracker() : CustomSingletonModel(HookType.Combat)
{
    public static readonly SpireField<PlayerCombatState, int> AgeField = new(() => Ages.Dark);
    public static readonly SpireField<PlayerCombatState, int> CardsThisTurnField = new(() => 0);

    public static int CardsThisTurn(Player? player)
    {
        var state = player?.PlayerCombatState;
        return state == null ? 0 : CardsThisTurnField.Get(state);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var owner = card.Owner;
        var state = owner?.PlayerCombatState;
        // Duplicates are Script re-runs (or other replays); they don't count and don't queue again.
        if (owner == null || state == null || card.IsDupe) return;

        int played = CardsThisTurnField.Get(state) + 1;
        CardsThisTurnField.Set(state, played);

        bool script = card.Keywords.Contains(BrotherKeywords.Script);
        if (!script && card is not SnapToGrid)
        {
            var snap = owner.Creature.GetPower<SnapToGridPower>();
            if (snap != null)
            {
                script = true;
                await PowerCmd.Decrement(snap);
            }
        }
        if (!script && played == 1 && owner.Creature.GetPower<AutomationSuitePower>() is { } suite)
        {
            script = true;
            suite.Trigger();
        }

        // Powers would re-apply themselves, so they never get Script.
        if (!script || card.Type == CardType.Power) return;
        await PowerCmd.Apply<ScriptPower>(choiceContext, owner.Creature, 1, owner.Creature, null, silent: true);
        owner.Creature.GetPower<ScriptPower>()?.Queue(card);
    }

    // Kids act at the end of their parent's turn (unless Date Night sent them to Grandma's).
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var creature in participants.Where(c => c.IsPlayer && c.IsAlive).ToList())
        {
            if (creature.Player == null || Kids.Count(creature.Player) == 0) continue;
            if (creature.GetPower<DateNightPower>() != null) continue;
            await Kids.Act(choiceContext, creature.Player);
            // Wife Aggro ("Yes, Dear"): they act a second time.
            if (creature.GetPower<YesDearPower>() != null) await Kids.Act(choiceContext, creature.Player);
        }
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState;
        if (state == null) return Task.CompletedTask;
        CardsThisTurnField.Set(state, 0);

        // Tim's Age shows above his head (redrawn each turn in case the creature node was rebuilt).
        if (player.Character is SpireBrothers.SpireBrothersCode.Character.Tim) AgeDisplay.Update(player);
        return Task.CompletedTask;
    }
}
