using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Joshua's upgraded starter (from Touch of Orobas, replacing Well-Worn Guitar): at the start of each turn, gain 2 Verses.</summary>
public class SignatureGuitar : JoshuaRelic
{
    public const int VersesPerTurn = 2;
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (player.Creature.CombatState == null) return;
        Flash();
        await Verses.Gain(choiceContext, player, VersesPerTurn, null);
    }
}
