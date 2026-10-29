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

/// <summary>Feral Druid. At the start of Brandi's turn, add Amount Claws to her hand (upgraded ones from Cat Form+).</summary>
public class CatFormPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected virtual bool UpgradedClaws => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || Owner.CombatState is not { } combat) return;
        Flash();
        for (int i = 0; i < Amount; i++)
        {
            var claw = combat.CreateCard<Cards.Brandi.Claw>(player);
            if (UpgradedClaws) CardCmd.Upgrade(claw);
            await CardPileCmd.AddGeneratedCardToCombat(claw, PileType.Hand, player);
        }
    }
}
