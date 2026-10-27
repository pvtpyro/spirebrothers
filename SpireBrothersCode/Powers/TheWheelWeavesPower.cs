using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>The Wheel of Time. Whenever you shuffle your draw pile, gain Amount energy and draw 2 per Amount.</summary>
public class TheWheelWeavesPower : BrothersPower
{
    public const int CardsPerStack = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if (shuffler != Owner.Player) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount, shuffler);
        await CardPileCmd.Draw(choiceContext, Amount * CardsPerStack, shuffler);
    }
}
