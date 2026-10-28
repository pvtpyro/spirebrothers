using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Total Annihilation. Huffman's Metal builds up during combat (MetalPower on him). Build X cards spend X Metal for
/// their bonus if he has it; without it they still work, just without the bonus.
/// </summary>
public static class Metal
{
    public static int Get(Player? player) => Math.Max(0, player?.Creature?.GetPowerAmount<MetalPower>() ?? 0);

    public static async Task Gain(PlayerChoiceContext ctx, Player player, int amount, CardModel? card)
    {
        if (amount <= 0) return;
        await PowerCmd.Apply<MetalPower>(ctx, player.Creature, amount, player.Creature, card);
        // Server Room: every bit of Metal also comes with Block.
        if (player.Creature.GetPower<ServerRoomPower>() is { } room) await room.OnMetal(amount);
    }

    /// <summary>Spends the Metal if he has enough and says whether he did.</summary>
    public static async Task<bool> Spend(PlayerChoiceContext ctx, Player player, int amount, CardModel? card)
    {
        if (amount <= 0) return true;
        if (player.Creature.GetPower<MetalPower>() is not { } metal || metal.Amount < amount) return false;
        await PowerCmd.ModifyAmount(ctx, metal, -amount, player.Creature, card);
        RunStats.Add(RunStats.MetalSpent, player, amount);
        return true;
    }

    /// <summary>
    /// A Build card's bonus: spends X Metal if he has it. Build Queue makes the next Build this turn free.
    /// </summary>
    public static async Task<bool> Build(PlayerChoiceContext ctx, Player player, int cost, CardModel card)
    {
        if (player.Creature.GetPower<BuildQueuePower>() is { } queue)
        {
            await PowerCmd.Remove(queue);
            return true;
        }
        return await Spend(ctx, player, cost, card);
    }

    public static bool CanBuild(Player? player, int cost) =>
        player?.Creature?.GetPower<BuildQueuePower>() != null || Get(player) >= cost;
}
