using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>The road trip. At the start of your turn, gain Amount energy and 2 Block per turn so far (times Amount).</summary>
public class IdahoToJerseyPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || player.PlayerCombatState == null) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount, player);
        await CreatureCmd.GainBlock(Owner, 2 * player.PlayerCombatState.TurnNumber * Amount, ValueProp.Unpowered, null);
    }
}
