using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Al and Pacino, Brandi's WoW hunter pets: two alpacas standing in front of her all combat (pets like Huffman's
/// Mastiff, untargetable). At the end of her turn Al spits on a random enemy and Pacino guards her (BrandiTracker).
/// "They act" runs both once. Feed Pet makes both stronger (FedPetPower). Random targets use the run RNG for co-op.
/// </summary>
public static class Alpacas
{
    public const int SpitDamage = 4, GuardBlock = 3;

    public static Creature? Al(Player? player) =>
        player?.PlayerCombatState?.Pets.FirstOrDefault(p => p.Monster is AlpacaAl && p.IsAlive);

    public static Creature? Pacino(Player? player) =>
        player?.PlayerCombatState?.Pets.FirstOrDefault(p => p.Monster is AlpacaPacino && p.IsAlive);

    public static int Spit(Player? player) => SpitDamage + (player?.Creature?.GetPowerAmount<FedPetPower>() ?? 0);
    public static int Guard(Player? player) => GuardBlock + (player?.Creature?.GetPowerAmount<FedPetPower>() ?? 0);

    public static async Task Spawn(Player player)
    {
        if (player.PlayerCombatState == null || !player.Creature.IsAlive) return;
        if (Al(player) == null) await PlayerCmd.AddPet<AlpacaAl>(player);
        if (Pacino(player) == null) await PlayerCmd.AddPet<AlpacaPacino>(player);
    }

    /// <summary>Al spits and Pacino guards, <paramref name="times"/> times.</summary>
    public static async Task Act(PlayerChoiceContext ctx, Player player, int times = 1)
    {
        for (int i = 0; i < times; i++)
        {
            var combat = player.Creature.CombatState;
            if (combat == null || !player.Creature.IsAlive) return;
            if (Al(player) is { } al)
            {
                var enemies = combat.HittableEnemies.Where(e => e.IsAlive).ToList();
                if (enemies.Count > 0)
                {
                    var target = player.RunState.Rng.CombatTargets.NextItem(enemies)!;
                    KidMotion.Hop(al);
                    Sounds.Play(Sounds.BrandiSpit);
                    VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_blunt");
                    await CreatureCmd.Damage(ctx, target, Spit(player), ValueProp.Unpowered, player.Creature);
                    RunStats.Add(RunStats.Spits, player);
                }
            }
            if (Pacino(player) is { } pacino)
            {
                KidMotion.Hop(pacino);
                await CreatureCmd.GainBlock(player.Creature, Guard(player), ValueProp.Unpowered, null);
            }
        }
    }
}
