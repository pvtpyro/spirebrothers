using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Tim's Script queue. Amount is how many cards are queued. At the start of his next turn, each queued card's
/// effect happens again: a temporary duplicate is auto-played (vanilla History Course does the same), then
/// Lua Add-On reacts. Duplicates vanish after playing and never queue again.
/// </summary>
public class ScriptPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Each Script power gets its own queue. The game makes powers by copying a template field by field, so without
    // DeepCloneFields every copy shared one list: cards left queued when a fight ended (the Kids landing the last hit
    // at turn end) replayed in the next fight, and two Tims in co-op shared a queue.
    private List<CardModel> _queued = new();

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _queued = new List<CardModel>();
    }

    // Duplicates made by Script replays, so Ringing Ears can ignore them (see RingingScriptFix).
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<CardModel, object> Replays = new();

    public static bool IsReplay(CardModel card) => Replays.TryGetValue(card, out _);

    public void Queue(CardModel card) => _queued.Add(card);

    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        var cards = _queued.ToList();
        _queued.Clear();
        await PowerCmd.Remove(this);
        // Repeat Last Command: each queued card runs Amount extra times.
        int runs = 1 + Owner.GetPowerAmount<RepeatLastCommandPower>();
        foreach (var card in cards)
        {
            for (int run = 0; run < runs; run++)
            {
                if (!Owner.IsAlive || CombatState == null || !CombatState.HittableEnemies.Any()) return;
                try
                {
                    var dupe = card.CreateDupe();
                    Replays.AddOrUpdate(dupe, true);
                    await CardCmd.AutoPlay(choiceContext, dupe, null);
                }
                catch (Exception e)
                {
                    MainFile.Logger.Error($"Script re-run failed for {card.GetType().Name}: {e}");
                    break;
                }
                await AfterScriptRan(choiceContext, player);
            }
        }
    }

    /// <summary>Lua Add-On: whenever a Script effect happens, deal damage to a random enemy.</summary>
    public static async Task AfterScriptRan(PlayerChoiceContext ctx, Player player)
    {
        foreach (var addOn in player.Creature.Powers.OfType<LuaAddOnPower>().ToList())
            await addOn.Trigger(ctx);
        RunStats.Add(RunStats.Scripts, player);
        await FamilyBonds.AfterScriptRan(player);
    }
}
