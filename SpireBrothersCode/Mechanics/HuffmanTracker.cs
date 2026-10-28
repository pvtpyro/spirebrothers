using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Huffman;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Huffman's in-combat rules: his dog shows up on turn 1 and bites at the end of each of his turns.</summary>
public class HuffmanTracker() : CustomSingletonModel(HookType.Combat)
{
    private static bool IsHuffman(Player? player) => player?.Character is SpireBrothers.SpireBrothersCode.Character.Huffman;

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (!IsHuffman(player) || player.Creature.CombatState is not { RoundNumber: 1 }) return;
        await Mastiff.Spawn(choiceContext, player);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var creature in participants.ToList())
            if (creature.Player is { } player && IsHuffman(player)) await Mastiff.EndOfTurn(choiceContext, player);
    }

    // I'll Wait Till I Get Home: each turn he holds onto it, it gets bigger.
    public override Task AfterFlush(PlayerChoiceContext choiceContext, Player player, IReadOnlyCollection<CardModel> flushedCards, IReadOnlyCollection<CardModel> retainedCards)
    {
        foreach (var card in retainedCards.OfType<IllWaitTillIGetHome>()) card.HeldAnotherTurn();
        return Task.CompletedTask;
    }
}
