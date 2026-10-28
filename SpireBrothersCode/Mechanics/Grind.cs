using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// John's Grind: a card's number grows permanently, for the rest of the run, the first time it's played each combat
/// (like vanilla Genetic Algorithm). The growth is saved on the deck copy of the card; combat copies read it from there.
/// The card side (applying it to the number, once per combat) lives in JohnCard.
/// </summary>
public static class Grind
{
    /// <summary>How much this card has ground up over the run.</summary>
    public static readonly SavedSpireField<CardModel, int> Ground = new(() => 0, "SpireBrothersGround");

    /// <summary>How many times John's cards have Ground this run (Completionist, Level Cap).</summary>
    public static readonly SavedSpireField<Player, int> Times = new(() => 0, "SpireBrothersGrinds");

    /// <summary>The run-long growth for a card: kept on its deck copy, or on itself if it was made in combat.</summary>
    public static int Of(CardModel card) => Ground.Get(card.DeckVersion ?? card);

    public static void Add(CardModel card, int amount)
    {
        Ground.Set(card, Ground.Get(card) + amount);
        if (card.DeckVersion is { } deck && deck != card) Ground.Set(deck, Ground.Get(deck) + amount);
    }

    public static int TimesThisRun(Player? player) => player == null ? 0 : Times.Get(player);
}
