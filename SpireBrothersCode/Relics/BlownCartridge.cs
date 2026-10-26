using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Shared (all brothers). At the start of your 2nd turn each combat, draw 2 cards. It works on the second try.</summary>
public class BlownCartridge : DanielRelic
{
    public const int Cards = 2;
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 2) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, Cards, player);
    }
}
