using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Song cards give their owner Verses after they're played. Verses kept into a new turn become Held.</summary>
public class JoshuaTracker() : CustomSingletonModel(HookType.Combat)
{
    // Early, so the guitar's Verse (given in AfterPlayerTurnStart) arrives fresh, not Held.
    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        Verses.HoldAll(player);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner?.PlayerCombatState == null || !cardPlay.Card.Keywords.Contains(BrotherKeywords.Song)) return;
        await Verses.Gain(choiceContext, owner, Verses.PerSong(owner), cardPlay.Card);
    }
}
