using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Huffman's cyber-mastiff (Darktide Arbitrator). One dog stands at his feet all combat (a pet, untargetable like Tim's
/// Kids). Its Bite is BitePower on Huffman. At the end of his turn it bites the Marked enemy (a random one if none is
/// Marked), done in HuffmanTracker. Random targets use the run RNG so co-op stays in sync.
/// </summary>
public static class Mastiff
{
    public const int StartingBite = 3;

    public static int Bite(Player? player) => Math.Max(0, player?.Creature?.GetPowerAmount<BitePower>() ?? 0);

    public static Creature? Dog(Player? player) =>
        player?.PlayerCombatState?.Pets.FirstOrDefault(p => p.Monster is MastiffDog && p.IsAlive);

    public static async Task Spawn(PlayerChoiceContext ctx, Player player)
    {
        if (Dog(player) != null || player.PlayerCombatState == null || !player.Creature.IsAlive) return;
        await PlayerCmd.AddPet<MastiffDog>(player);
        // Chew Toy: the dog shows up a little more worked up.
        int bite = StartingBite;
        if (player.GetRelic<Relics.ChewToy>() is { } toy) { toy.OnSpawn(); bite += Relics.ChewToy.ExtraBite; }
        await PowerCmd.Apply<BitePower>(ctx, player.Creature, bite, player.Creature, null);
    }

    public static async Task GainBite(PlayerChoiceContext ctx, Player player, int amount, CardModel? card)
    {
        if (amount <= 0) return;
        await PowerCmd.Apply<BitePower>(ctx, player.Creature, amount, player.Creature, card);
    }

    public static Creature? Marked(ICombatState? combat) =>
        combat?.HittableEnemies.FirstOrDefault(e => e.IsAlive && e.GetPower<MarkedPower>() != null);

    /// <summary>Marks an enemy for the dog. Only one enemy is Marked at a time, so the mark moves.</summary>
    public static async Task Mark(PlayerChoiceContext ctx, Player player, Creature target, CardModel? card)
    {
        if (!target.IsAlive || target.CombatState is not { } combat) return;
        foreach (var other in combat.HittableEnemies.Where(e => e != target).ToList())
            if (other.GetPower<MarkedPower>() is { } old) await PowerCmd.Remove(old);
        if (target.GetPower<MarkedPower>() == null) await PowerCmd.Apply<MarkedPower>(ctx, target, 1, player.Creature, card);
    }

    /// <summary>The dog bites once: the given target, else the Marked enemy, else a random one.</summary>
    public static async Task Attack(PlayerChoiceContext ctx, Player player, Creature? target = null)
    {
        var combat = player.Creature.CombatState;
        if (combat == null || !player.Creature.IsAlive || Dog(player) is not { } dog) return;
        // Lone Wolf: he sent the dog home for this fight.
        if (player.Creature.GetPower<LoneWolfPower>() != null) return;
        target ??= Marked(combat);
        if (target is not { IsAlive: true })
        {
            var enemies = combat.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            target = player.RunState.Rng.CombatTargets.NextItem(enemies)!;
        }
        int bite = Bite(player);
        if (bite <= 0) return;
        KidMotion.Hop(dog);
        VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_slash");
        await CreatureCmd.Damage(ctx, target, bite, ValueProp.Unpowered, player.Creature);
        Sounds.Play(Sounds.HuffmanBark);
        RunStats.Add(RunStats.Bites, player);
        await FamilyBonds.AfterBite(ctx, player, target);
        if (player.Creature.GetPower<LoyalCompanionPower>() is { } loyal) await loyal.AfterBite();
        if (player.Creature.GetPower<HomeSweetHomePower>() != null) await Metal.Gain(ctx, player, HomeSweetHomePower.MetalPerBite, null);
    }

    /// <summary>End of Huffman's turn: one bite, plus one per Best in Show.</summary>
    public static async Task EndOfTurn(PlayerChoiceContext ctx, Player player)
    {
        int bites = 1 + player.Creature.GetPowerAmount<BestInShowPower>();
        for (int i = 0; i < bites; i++) await Attack(ctx, player);
    }
}
