using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Whenever you trigger Exact, gain Amount Gold.</summary>
public class ReturnOnInvestmentPower : BrothersPower, IExactListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnExact(PlayerChoiceContext choiceContext, Player player)
    {
        Flash();
        await PlayerCmd.GainGold(Amount, player);
    }
}
