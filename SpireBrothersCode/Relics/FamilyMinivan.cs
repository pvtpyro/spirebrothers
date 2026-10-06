using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Tim's starter. Start each combat with 2 Kids.</summary>
public class FamilyMinivan : TimRelic
{
    public const int StartingKids = 2;
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Fifteen-Passenger Van.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<FifteenPassengerVan>().ToMutable();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        await Kids.Gain(choiceContext, player, StartingKids, null);
    }
}
