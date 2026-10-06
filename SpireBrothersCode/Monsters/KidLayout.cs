using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The game spreads pets evenly across their owner's whole body width, so two Kids end up at opposite edges (one
/// nearly under the next player). After the game places pets, this lines Tim's Kids up side by side at his feet.
/// Other pets are left where the game put them.
/// </summary>
[HarmonyPatch]
public static class KidLayout
{
    /// <summary>Distance between neighbouring Kids. The placeholder art is 56 wide, so they overlap a little.</summary>
    private const float Spacing = 42f;

    /// <summary>How far in front of the owner's centre the group stands.</summary>
    private const float Forward = 40f;

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
    [HarmonyPostfix]
    private static void AfterAddCreature(NCombatRoom __instance, Creature creature)
    {
        if (creature.Monster is not Kid || creature.PetOwner == null) return;
        Arrange(__instance.GetCreatureNode(creature.PetOwner.Creature), __instance.CreatureNodes);
    }

    [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.PositionPlayersAndPets))]
    [HarmonyPostfix]
    private static void AfterPositionPlayersAndPets(List<NCreature> creatureNodes)
    {
        foreach (var player in creatureNodes.Where(n => n.Entity.IsPlayer).ToList())
            Arrange(player, creatureNodes);
    }

    private static void Arrange(NCreature? owner, IEnumerable<NCreature> nodes)
    {
        try
        {
            if (owner?.Entity.Player == null) return;
            var kids = nodes.Where(n => n.Entity.Monster is Kid && n.Entity.PetOwner == owner.Entity.Player).ToList();
            for (int i = 0; i < kids.Count; i++)
            {
                float x = owner.Position.X + Forward + (i - (kids.Count - 1) / 2f) * Spacing;
                kids[i].Position = new Vector2(x, owner.Position.Y + 10f);
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Kid layout failed: {e}");
        }
    }
}
