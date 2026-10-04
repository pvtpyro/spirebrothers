using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Starter: if you played both a Logic and a Hands card last turn, draw 1 at the start of your turn.</summary>
public class AndYouKnowWhat : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.PlayerCombatState == null) return;
        if (!TurnTracker.MixedLastTurn.Get(player.PlayerCombatState)) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, 1, player);
    }
}
