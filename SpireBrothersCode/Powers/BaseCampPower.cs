using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>David's Base Camp. At the start of your turn, ALL players gain Amount Block.</summary>
public class BaseCampPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null) return;
        Flash();
        foreach (var p in CombatState.Players.Where(p => p.Creature.IsAlive).ToList())
            await CreatureCmd.GainBlock(p.Creature, Amount, ValueProp.Unpowered, null);
    }
}
