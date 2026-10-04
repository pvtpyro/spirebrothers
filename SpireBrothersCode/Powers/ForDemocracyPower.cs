using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Helldivers 2. Whenever you play a Stratagem, gain Amount energy and draw Amount cards.</summary>
public class ForDemocracyPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player?.Creature != Owner || !cardPlay.Card.Keywords.Contains(BrotherKeywords.Stratagem)) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount, player);
        await CardPileCmd.Draw(choiceContext, Amount, player);
    }
}
