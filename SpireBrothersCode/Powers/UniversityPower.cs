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

/// <summary>Tim's University (Age of Empires). At the start of your turn, Upgrade Amount random cards in your draw pile for this combat.</summary>
public class UniversityPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        var upgradable = PileType.Draw.GetPile(player).Cards.Where(c => c.IsUpgradable).ToList();
        if (upgradable.Count == 0) return Task.CompletedTask;
        Flash();
        var rng = player.RunState.Rng.CombatCardSelection;
        for (int i = 0; i < Amount && upgradable.Count > 0; i++)
        {
            var card = rng.NextItem(upgradable)!;
            upgradable.Remove(card);
            CardCmd.Upgrade(card);
        }
        return Task.CompletedTask;
    }
}
