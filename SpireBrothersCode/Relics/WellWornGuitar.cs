using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Joshua's starter. Start each combat with 3 Verses.</summary>
public class WellWornGuitar : JoshuaRelic
{
    public const int StartingVerses = 3;
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Signature Guitar.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<SignatureGuitar>().ToMutable();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        await Verses.Gain(choiceContext, player, StartingVerses, null);
    }
}
