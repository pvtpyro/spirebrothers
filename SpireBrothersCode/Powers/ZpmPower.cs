using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Stargate Atlantis. At the start of your turn, gain 2 energy. Amount is the turns of charge left; it runs down to 0.</summary>
public class ZpmPower : BrothersPower
{
    public const int EnergyPerTurn = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PlayerCmd.GainEnergy(EnergyPerTurn, player);
        await PowerCmd.Decrement(this);
    }
}
