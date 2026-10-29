using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// WoW: she plays everything. At the start of Brandi's turn she picks a spec: Feral (gain Amount Strength this turn) or
/// Discipline (ALL players gain 2 x Amount + 1 Block). The two choices are token cards on a choose-a-card screen.
/// </summary>
public class DualSpecPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int Block => Amount * 2 + 1;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || Owner.CombatState is not { } combat) return;
        var feral = combat.CreateCard<Cards.Brandi.DualSpecFeral>(player);
        var disc = combat.CreateCard<Cards.Brandi.DualSpecDisc>(player);
        feral.DynamicVars["Strength"].BaseValue = Amount;
        disc.DynamicVars.Block.BaseValue = Block;
        Flash();
        var picked = await CardSelectCmd.FromChooseACardScreen(choiceContext, [feral, disc], player);
        if (picked == disc)
        {
            foreach (var p in combat.Players.Where(p => p.Creature.IsAlive))
                await CreatureCmd.GainBlock(p.Creature, Block, ValueProp.Unpowered, null);
        }
        else await PowerCmd.Apply<FlexPotionPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
