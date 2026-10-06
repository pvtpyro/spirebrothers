using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Daniel's upgraded starter (from Touch of Orobas, replacing And You Know What?): if you played both a Logic and a
/// Hands card last turn, draw 2 at the start of your turn.
/// </summary>
public class WellTechnically : DanielRelic
{
    public const int Draw = 2;
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.PlayerCombatState == null) return;
        if (!TurnTracker.MixedLastTurn.Get(player.PlayerCombatState)) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, Draw, player);
    }
}
