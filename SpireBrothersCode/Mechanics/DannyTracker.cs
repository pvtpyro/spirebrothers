using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Danny's in-combat rules (Mechanics/Titan): his damage fills the Titan Meter, damage meant for him hits his Titan
/// first, his own cards give him no Block while he's in it, and he ejects when it's destroyed.
/// </summary>
public class DannyTracker() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (Titan.IsDanny(player) && player.Creature.CombatState is { RoundNumber: 1 }) TitanMeterDisplay.Update(player);
        return Task.CompletedTask;
    }

    // A full meter whose card got discarded (or never fit in a full hand) hands over a new one.
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Titan.IsReady(player)) await Titan.HandOverToken(player);
    }

    // Everything he hits fills the meter, Block soaked or not. The Titan's hits don't count (the meter is hidden then).
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer?.Player is not { } player || !Titan.IsDanny(player) || target.Side == dealer.Side) return;
        if (Titan.IsIn(player))
        {
            RunStats.Add(RunStats.TitanDamage, player, result.TotalDamage);
            return;
        }
        await Titan.GainMeter(player, result.TotalDamage);
    }

    public override Creature ModifyUnblockedDamageTarget(Creature target, decimal amount, ValueProp props, Creature? dealer) =>
        Titan.Redirect(target, amount, dealer) ?? target;

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay) =>
        Titan.BlocksBlock(target, cardSource) ? 0 : 1;

    // The Titan's bar is drawn by the mod (TitanHullDisplay), so it follows every HP change here.
    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature.Monster is DannyTitan && creature.PetOwner is { } player) TitanHullDisplay.Update(player);
        return Task.CompletedTask;
    }

    // If he goes down while in the Titan, show his body again so he can be seen falling.
    public override Task BeforeDeath(Creature creature)
    {
        if (Titan.IsDanny(creature.Player)) TitanLayout.ShowPilot(creature, true);
        return Task.CompletedTask;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.Monster is not DannyTitan || creature.PetOwner is not { } player) return;
        await Titan.AfterDestroyed(choiceContext, player);
    }
}
