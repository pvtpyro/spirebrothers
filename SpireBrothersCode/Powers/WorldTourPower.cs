using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>World Tour (Ancient). Playing an Attack adds a Verse, and new Verses start Held (read in Verses.Gain).</summary>
public class WorldTourPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = Owner.Player;
        if (player == null || cardPlay.Card.Owner != player || cardPlay.Card.Type != CardType.Attack) return;
        Flash();
        await Verses.Gain(choiceContext, player, 1, cardPlay.Card);
    }
}
