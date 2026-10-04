using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>while true do (the infinite loop). At the start of each turn, gain 3 Block and draw 1 card per stack. Counts as a Script effect.</summary>
public class WhileTrueDoPower : BrothersPower
{
    public const int BlockPerLoop = 3;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        for (int i = 0; i < Amount; i++)
        {
            await CreatureCmd.GainBlock(Owner, BlockPerLoop, ValueProp.Unpowered, null);
            await CardPileCmd.Draw(choiceContext, 1, player);
            await ScriptPower.AfterScriptRan(choiceContext, player);
        }
    }
}
