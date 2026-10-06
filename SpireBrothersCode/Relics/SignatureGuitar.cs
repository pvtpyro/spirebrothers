using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Joshua's upgraded starter (from Touch of Orobas, replacing Well-Worn Guitar): start each combat with 4 Verses.</summary>
public class SignatureGuitar : JoshuaRelic
{
    public const int StartingVerses = 4;
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        await Verses.Gain(choiceContext, player, StartingVerses, null);
    }
}
