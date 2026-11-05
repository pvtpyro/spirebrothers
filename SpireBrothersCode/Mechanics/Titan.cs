using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards;
using SpireBrothers.SpireBrothersCode.Cards.Danny;
using SpireBrothers.SpireBrothersCode.Monsters;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Danny's Titan (Titanfall). Damage he deals fills the Titan Meter (one per combat, starts empty). When it's full he
/// gets a Stand By for Titanfall card; playing it drops his Titan in and he climbs in: a pet (Monsters/DannyTitan) with its own HP
/// (30 on the Atlas, and small hits do nothing to it; Mechanics/Chassis) that stands where he does, hides his body, and
/// takes the damage meant for him. While it stands, TitanPower is on him, the meter is hidden, his cards show their
/// Titan side (DannyCard), and his own cards can't give him Block (teammates' still can). When the Titan is destroyed he
/// ejects: the meter comes back, empty, and fills again. The hooks live in DannyTracker.
/// </summary>
public static class Titan
{
    // The meter size, the hull (before Titan Kit), and the Titan Armor come from his chassis (Mechanics/Chassis).
    /// <summary>Damage it takes to fill the meter.</summary>
    public static int MeterSize(Player? player) => TitanChassis.Of(player).MeterSize;
    /// <summary>Titan Armor: hits on the Titan of this much or less do nothing.</summary>
    public static int ArmorThreshold(Player? player) => TitanChassis.Of(player).Armor;

    private static readonly SpireField<PlayerCombatState, int> Meter = new(() => 0);

    public static bool IsDanny(Player? player) => player?.Character is Character.Danny;

    public static Creature? Unit(Player? player) =>
        player?.PlayerCombatState?.Pets.FirstOrDefault(p => p.Monster is DannyTitan && p.IsAlive);

    /// <summary>True while his Titan stands.</summary>
    public static bool IsIn(Player? player) => Unit(player) != null;

    public static int MeterOf(Player? player) => player?.PlayerCombatState is { } state ? Meter.Get(state) : 0;

    public static bool IsReady(Player? player) => !IsIn(player) && MeterOf(player) >= MeterSize(player);

    public static int MaxHull(Player player) => TitanChassis.Of(player).Hull + player.Creature.GetPowerAmount<TitanKitPower>();

    /// <summary>Fills the meter (Titan Hunter: 50% more per stack). Nothing while the Titan stands.</summary>
    public static async Task GainMeter(Player player, int amount)
    {
        if (amount <= 0 || !IsDanny(player) || IsIn(player) || player.PlayerCombatState is not { } state) return;
        int hunter = player.Creature.GetPowerAmount<TitanHunterPower>();
        if (hunter > 0) amount += amount * hunter / 100;
        int before = Meter.Get(state);
        int size = MeterSize(player);
        if (before >= size) return;
        Meter.Set(state, Math.Min(size, before + amount));
        TitanMeterDisplay.Update(player);
        if (Meter.Get(state) >= size) await HandOverToken(player);
    }

    /// <summary>The meter is full: put Stand By for Titanfall in his hand, unless one is already there.</summary>
    public static async Task HandOverToken(Player player)
    {
        if (!IsReady(player) || player.Creature.CombatState is not { } combat || !player.Creature.IsAlive) return;
        if (PileType.Hand.GetPile(player).Cards.Any(c => c is StandByForTitanfall)) return;
        var token = combat.CreateCard<StandByForTitanfall>(player);
        await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, player);
    }

    /// <summary>Stand by for Titanfall: drops the Titan in at full HP, empties the meter, and flips his hand.</summary>
    public static async Task Call(PlayerChoiceContext ctx, Player player)
    {
        if (IsIn(player) || !player.Creature.IsAlive || player.PlayerCombatState is not { } state) return;
        Meter.Set(state, 0);
        Sounds.Play(Sounds.DannyTitanfall);
        var titan = await PlayerCmd.AddPet<DannyTitan>(player);
        await CreatureCmd.SetMaxAndCurrentHp(titan, MaxHull(player));
        TitanLayout.ShowPilot(player.Creature, false);
        TitanHullDisplay.Update(player);
        await PowerCmd.Apply<TitanArmorPower>(ctx, titan, ArmorThreshold(player), player.Creature, null);
        await PowerCmd.Apply<TitanPower>(ctx, player.Creature, 1, player.Creature, null);
        RunStats.Add(RunStats.Titanfalls, player);
        TitanMeterDisplay.Update(player);
        DannyCard.FlipHand(player);
        await TitanChassis.AfterTitanfall(ctx, player);
        if (player.Creature.GetPower<WarpfallPower>() is { } warpfall) await warpfall.AfterTitanfall(ctx);
    }

    /// <summary>Called by DannyTracker once the Titan is destroyed (or he ejects on purpose).</summary>
    public static async Task AfterDestroyed(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature.GetPower<TitanPower>() is { } inside) await PowerCmd.Remove(inside);
        await TitanChassis.AfterEject(player);
        Sounds.Play(Sounds.DannyEject);
        TitanLayout.ShowPilot(player.Creature, true);
        TitanHullDisplay.Update(player);
        if (player.Creature.GetPower<NuclearEjectionPower>() is { } nuke) await nuke.Detonate(ctx);
        TitanMeterDisplay.Update(player);
        DannyCard.FlipHand(player);
    }

    /// <summary>Leave the Titan on purpose: it's destroyed, the same as if the enemies had done it.</summary>
    public static async Task Eject(Player player)
    {
        if (Unit(player) is { } titan) await CreatureCmd.Kill(titan);
    }

    /// <summary>Repair: the Titan gets back HP (never above its max). Nothing if it isn't standing.</summary>
    public static async Task Repair(Player player, decimal amount)
    {
        if (amount <= 0 || Unit(player) is not { } titan) return;
        await CreatureCmd.Heal(titan, amount);
    }

    /// <summary>Damage to a player with a Titan up goes to the Titan first (the rest spills onto him).</summary>
    public static Creature? Redirect(Creature target, decimal amount, Creature? dealer)
    {
        if (amount <= 0 || !target.IsPlayer || dealer == target) return null;
        return Unit(target.Player);
    }

    /// <summary>His own cards can't give him Block in the Titan.</summary>
    public static bool BlocksBlock(Creature target, CardModel? source) =>
        target.Player is { } player && source?.Owner == player && IsIn(player);

    public static void Reset(Player player)
    {
        if (player.PlayerCombatState is { } state) Meter.Set(state, 0);
    }

    /// <summary>Unpowered damage to ALL enemies (Nuclear Ejection, Satchel Charge).</summary>
    public static Task HitAll(PlayerChoiceContext ctx, Player player, decimal amount)
    {
        var enemies = player.Creature.CombatState?.HittableEnemies.Where(e => e.IsAlive).ToList() ?? [];
        return enemies.Count == 0 || amount <= 0
            ? Task.CompletedTask
            : CreatureCmd.Damage(ctx, enemies, amount, ValueProp.Unpowered, player.Creature, null);
    }
}
