using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Song cards give their owner Verses after they're played.</summary>
public class JoshuaTracker() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner?.PlayerCombatState == null || !cardPlay.Card.Keywords.Contains(BrotherKeywords.Song)) return;
        await Verses.Gain(choiceContext, owner, Verses.PerSong(owner), cardPlay.Card);
    }
}
