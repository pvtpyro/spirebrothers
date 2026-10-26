using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// David's Rainy Day Fund. At the start of your turn, the living player with the lowest HP gains 1 Block per 25 Gold
/// you have (max 10), times the stacks. Ties go to the first player in the list, so every client picks the same one.
/// </summary>
public class RainyDayFundPower : BrothersPower
{
    public const int GoldPerBlock = 25;
    public const int MaxPerStack = 10;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null) return;
        int block = Math.Min(player.Gold / GoldPerBlock, MaxPerStack) * Amount;
        var neediest = CombatState.Players.Where(p => p.Creature.IsAlive).OrderBy(p => p.Creature.CurrentHp).FirstOrDefault();
        if (block <= 0 || neediest == null) return;
        Flash();
        await CreatureCmd.GainBlock(neediest.Creature, block, ValueProp.Unpowered, null);
    }
}
