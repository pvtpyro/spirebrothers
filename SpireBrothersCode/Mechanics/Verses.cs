using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Powers that react whenever their owner plays a Chorus (Harmony, Junimos).</summary>
public interface IChorusListener
{
    Task OnChorus(PlayerChoiceContext choiceContext, Player player, int verses);
}

/// <summary>
/// Joshua's Verse / Chorus rules. Song cards add Verses (JoshuaTracker), Chorus cards spend them all.
/// Choir adds to what a Chorus counts, Encore and Bridge let a Chorus keep its Verses.
/// </summary>
public static class Verses
{
    /// <summary>Verses the player is holding right now.</summary>
    public static int Count(Player? player) => player?.Creature?.GetPowerAmount<VersePower>() ?? 0;

    /// <summary>How many Verses a Chorus would count if played now (Choir adds to it).</summary>
    public static int ForChorus(Player? player) =>
        Count(player) + (player?.Creature?.GetPowerAmount<ChoirPower>() ?? 0);

    /// <summary>Verses a Song gives: 1, plus 1 per Perfect Pitch.</summary>
    public static int PerSong(Player? player) => 1 + (player?.Creature?.GetPowerAmount<PerfectPitchPower>() ?? 0);

    public static async Task Gain(PlayerChoiceContext ctx, Player player, decimal amount, CardModel? source)
    {
        if (amount <= 0 || !player.Creature.IsAlive) return;
        await PowerCmd.Apply<VersePower>(ctx, player.Creature, amount, player.Creature, source);
    }

    /// <summary>
    /// Call at the start of a Chorus card's OnPlay. Returns the Verses it counts, then spends them
    /// (unless Encore or Bridge saves them) and runs the owner's Chorus listeners.
    /// </summary>
    public static async Task<int> SpendForChorus(PlayerChoiceContext ctx, CardModel card)
    {
        var owner = card.Owner;
        if (owner?.Creature == null) return 0;
        int verses = ForChorus(owner);

        // Encore first (it comes back every turn), then Bridge (one use each).
        bool keep = owner.Creature.GetPower<EncorePower>()?.TryUse() ?? false;
        if (!keep)
        {
            var bridge = owner.Creature.GetPower<BridgePower>();
            if (bridge != null)
            {
                keep = true;
                await bridge.Use();
            }
        }
        if (!keep)
        {
            var held = owner.Creature.GetPower<VersePower>();
            if (held != null) await PowerCmd.Remove(held);
        }

        foreach (var listener in owner.Creature.Powers.OfType<IChorusListener>().ToList())
            await listener.OnChorus(ctx, owner, verses);
        return verses;
    }

    /// <summary>Adds random Song cards from Joshua's pool to the player's hand, optionally free this turn.</summary>
    public static async Task AddRandomSongs(Player player, int count, bool freeThisTurn)
    {
        if (count <= 0 || player.Creature.CombatState == null) return;
        var songs = ModelDb.CardPool<JoshuaCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanonicalKeywords.Contains(BrotherKeywords.Song))
            .ToList();
        var picked = CardFactory.GetDistinctForCombat(player, songs, count, player.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in picked)
        {
            if (freeThisTurn) card.EnergyCost.SetThisTurn(0);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
    }
}
