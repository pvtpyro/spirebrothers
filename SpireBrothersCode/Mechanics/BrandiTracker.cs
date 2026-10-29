using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Cards whose cost drops with what's going on in the fight (Nobody Tells Me No). Read by BrandiTracker.</summary>
public interface ICostReducer
{
    int CostReduction { get; }
}

/// <summary>
/// Brandi's in-combat rules: Al and Pacino show up on turn 1 and act at the end of each of her turns, Ember jumps to
/// another enemy when its owner dies, and cards that get cheaper (ICostReducer) are priced here.
/// </summary>
public class BrandiTracker() : CustomSingletonModel(HookType.Combat)
{
    public static bool IsBrandi(Player? player) => player?.Character is Character.Brandi;

    // Ember on enemies that are dying, picked up again in AfterDeath once they're gone.
    private readonly Dictionary<Creature, (int Ember, ICombatState Combat)> _dyingEmber = new();

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (!IsBrandi(player) || player.Creature.CombatState is not { RoundNumber: 1 }) return;
        await Alpacas.Spawn(player);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var creature in participants.ToList())
            if (creature.Player is { } player && IsBrandi(player)) await Alpacas.Act(choiceContext, player);
    }

    public override Task BeforeDeath(Creature creature)
    {
        if (creature.IsMonster && creature.CombatState is { } combat && Ember.On(creature) is var ember and > 0) _dyingEmber[creature] = (ember, combat);
        return Task.CompletedTask;
    }

    // Fires spread: the Ember on a fallen enemy jumps to another random one (run RNG, so co-op stays in sync). Applied
    // straight, not through Ember.Apply, so the Zippo and Private Pyro don't add to it again.
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!_dyingEmber.Remove(creature, out var dying) || wasRemovalPrevented) return;
        var (ember, combat) = dying;
        var others = Ember.Enemies(combat).Where(e => e != creature).ToList();
        var brandi = combat.Players.FirstOrDefault(IsBrandi) ?? combat.Players.FirstOrDefault();
        if (others.Count == 0 || brandi == null) return;
        var next = brandi.RunState.Rng.CombatTargets.NextItem(others)!;
        await PowerCmd.Apply<EmberPower>(choiceContext, next, ember, brandi.Creature, null);
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _dyingEmber.Clear();
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card is not ICostReducer reducer || reducer.CostReduction <= 0 || originalCost <= 0) return false;
        modifiedCost = Math.Max(0, originalCost - reducer.CostReduction);
        return true;
    }
}
