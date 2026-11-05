using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.John;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>John's in-combat rules: his Stimm Lab talents that act at the start of combat or on his Cartel Special.</summary>
public class JohnTracker() : CustomSingletonModel(HookType.Combat)
{
    // Whether John has Ground yet this combat (the Efficient Grind talent doubles the first one).
    private static readonly SpireField<PlayerCombatState, bool> GroundYet = new(() => false);
    // HP John has lost this combat, from any source (Payback).
    private static readonly SpireField<PlayerCombatState, int> HpLost = new(() => 0);

    public static int HpLostThisCombat(Player? player) =>
        player?.PlayerCombatState is { } state ? HpLost.Get(state) : 0;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target.Player is { Character: Character.John, PlayerCombatState: { } state } && result.UnblockedDamage > 0)
            HpLost.Set(state, HpLost.Get(state) + result.UnblockedDamage);
        return Task.CompletedTask;
    }

    /// <summary>True the first time it's asked each combat, false after.</summary>
    public static bool TakeFirstGrind(Player player)
    {
        var state = player.PlayerCombatState;
        if (state == null || GroundYet.Get(state)) return false;
        GroundYet.Set(state, true);
        return true;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Character is not Character.John) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        if (StimmLab.TotalRanks(player) > 0 && player.Creature.GetPower<TalentsPower>() == null)
            await PowerCmd.Apply<TalentsPower>(choiceContext, player.Creature, 1, player.Creature, null, silent: true);
        // Pack Mule: start each combat with some Block.
        int mule = StimmLab.Rank(player, StimmLab.Talent.PackMule);
        if (mule > 0) await CreatureCmd.GainBlock(player.Creature, StimmLab.PackMuleBlock * mule, ValueProp.Unpowered, null);
        // Toxic Payload: every enemy starts the fight with Toxin.
        int payload = StimmLab.Rank(player, StimmLab.Talent.ToxicPayload);
        if (payload > 0) await Toxin.Apply(choiceContext, combat.HittableEnemies, StimmLab.ToxicPayloadToxin * payload, player, null);
    }

    // Potency: his Cartel Special gives more Block per rank.
    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, MegaCrit.Sts2.Core.Entities.Cards.CardPlay? cardPlay)
    {
        if (cardSource is not (CartelSpecial or CartelMasterwork) || cardSource.Owner == null) return 0m;
        return StimmLab.PotencyBlock * StimmLab.Rank(cardSource.Owner, StimmLab.Talent.Potency);
    }
}
