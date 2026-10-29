using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Brandi's MI work. Intel (IntelPower) on an enemy makes Attacks from any player deal more damage to it until the end
/// of the round. Every Intel she applies goes through Apply, so Comms Up can react.
/// </summary>
public static class Intel
{
    public static async Task Apply(PlayerChoiceContext ctx, IEnumerable<Creature> targets, int amount, Player by, CardModel? card)
    {
        var alive = targets.Where(t => t.IsAlive).ToList();
        if (alive.Count == 0 || amount <= 0) return;
        await PowerCmd.Apply<IntelPower>(ctx, alive, amount, by.Creature, card);
        if (by.Creature.GetPower<CommsUpPower>() is { } comms) await comms.Applied();
    }

    public static Task Apply(PlayerChoiceContext ctx, Creature target, int amount, Player by, CardModel? card) =>
        Apply(ctx, [target], amount, by, card);
}

/// <summary>Counts of what a player has played this turn, from the game's combat history (like vanilla Finisher).</summary>
public static class PlayedThisTurn
{
    public static int Count(Player? player, ICombatState? combat, Func<CardModel, bool> which) =>
        player == null || combat == null ? 0 : CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(combat) && e.CardPlay.Card.Owner == player && which(e.CardPlay.Card));

    public static int Attacks(Player? player, ICombatState? combat) => Count(player, combat, c => c.Type == CardType.Attack);
    public static int Skills(Player? player, ICombatState? combat) => Count(player, combat, c => c.Type == CardType.Skill);
    public static int Cards(Player? player, ICombatState? combat) => Count(player, combat, _ => true);
}

/// <summary>Picks the living player with the lowest HP (Disc Priest cards), the owner in solo.</summary>
public static class LowestHp
{
    public static Creature Player(Player owner)
    {
        var players = owner.Creature.CombatState?.Players.Select(p => p.Creature).Where(c => c.IsAlive).ToList() ?? [];
        return players.OrderBy(c => c.CurrentHp).ThenBy(c => c == owner.Creature ? 1 : 0).FirstOrDefault() ?? owner.Creature;
    }

    public static Task Block(Player owner, int amount) =>
        amount <= 0 ? Task.CompletedTask : CreatureCmd.GainBlock(Player(owner), amount, ValueProp.Unpowered, null);
}
