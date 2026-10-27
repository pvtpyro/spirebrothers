using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Catan. At the start of your turn, gain Amount Verses for every 2 cards you played last turn.</summary>
public class LongestRoadPower : BrothersPower
{
    public const int CardsPerVerse = 2;

    private int _playedThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner) _playedThisTurn++;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        int verses = _playedThisTurn / CardsPerVerse * Amount;
        _playedThisTurn = 0;
        if (verses <= 0) return;
        Flash();
        await Verses.Gain(choiceContext, player, verses, null);
    }
}
