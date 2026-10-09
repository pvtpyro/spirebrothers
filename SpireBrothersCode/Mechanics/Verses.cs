using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Orbs;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Powers that react whenever their owner plays a Chorus (Harmony, Junimos).</summary>
public interface IChorusListener
{
    Task OnChorus(PlayerChoiceContext choiceContext, Player player, int verses);
}

/// <summary>
/// Joshua's Verse / Chorus rules. Song cards add Verses (JoshuaTracker), Chorus cards spend them all.
/// Choir adds to what a Chorus counts, Encore lets a Chorus keep half its Verses, and Bridge keeps them all.
/// </summary>
public static class Verses
{
    /// <summary>The most Verses you can hold: one per orb slot, and the game allows 10.</summary>
    public const int Max = OrbQueue.maxCapacity;

    /// <summary>Verses the player is holding right now (VerseOrbs floating above them).</summary>
    public static int Count(Player? player) =>
        player?.PlayerCombatState?.OrbQueue.Orbs.Count(o => o is VerseOrb) ?? 0;

    /// <summary>How many Verses a Chorus would count if played now (Choir adds to it).</summary>
    public static int ForChorus(Player? player) =>
        Count(player) + (player?.Creature?.GetPowerAmount<ChoirPower>() ?? 0);

    /// <summary>Verses a Song gives: 1, plus 1 per Perfect Pitch.</summary>
    public static int PerSong(Player? player) => 1 + (player?.Creature?.GetPowerAmount<PerfectPitchPower>() ?? 0);

    /// <summary>Adds Verse orbs (up to 10), opening a new orb slot for each so they never push other orbs out.</summary>
    public static async Task Gain(PlayerChoiceContext ctx, Player player, decimal amount, CardModel? source)
    {
        var state = player.PlayerCombatState;
        if (amount <= 0 || state == null || !player.Creature.IsAlive) return;
        for (int i = 0; i < (int)amount && Count(player) < Max; i++)
        {
            if (state.OrbQueue.Orbs.Count >= state.OrbQueue.Capacity)
            {
                if (state.OrbQueue.Capacity >= OrbQueue.maxCapacity) return;
                await OrbCmd.AddSlots(player, 1);
            }
            await OrbCmd.Channel(ctx, ModelDb.Orb<VerseOrb>().ToMutable(), player);
        }
    }

    /// <summary>Removes the newest <paramref name="count"/> Verse orbs without evoking them, and closes the slots they used.</summary>
    private static void Spend(Player player, int count)
    {
        var state = player.PlayerCombatState;
        if (state == null || count <= 0) return;
        var verses = state.OrbQueue.Orbs.OfType<VerseOrb>().TakeLast(count).ToList();
        var orbManager = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.OrbManager;
        foreach (var verse in verses)
        {
            if (!state.OrbQueue.Remove(verse)) continue;
            orbManager?.EvokeOrbAnim(verse);
            verse.RemoveInternal();
        }
        int emptySlots = state.OrbQueue.Capacity - state.OrbQueue.Orbs.Count;
        if (emptySlots > 0) OrbCmd.RemoveSlots(player, Math.Min(emptySlots, verses.Count));
    }

    /// <summary>
    /// Call at the start of a Chorus card's OnPlay. Returns the Verses it counts, then spends them
    /// (Encore saves half, Bridge saves all) and runs the owner's Chorus listeners.
    /// </summary>
    public static async Task<int> SpendForChorus(PlayerChoiceContext ctx, CardModel card)
    {
        var owner = card.Owner;
        if (owner?.Creature == null) return 0;
        int verses = ForChorus(owner);

        // Encore first (it comes back every turn, and spends half, rounded down), then Bridge (one use each, spends none).
        int spend = Count(owner);
        if (owner.Creature.GetPower<EncorePower>()?.TryUse() ?? false)
        {
            spend /= 2;
        }
        else
        {
            var bridge = owner.Creature.GetPower<BridgePower>();
            if (bridge != null)
            {
                spend = 0;
                await bridge.Use();
            }
        }
        Spend(owner, spend);

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
