using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Joshua's starter. At the start of each turn, gain 1 Verse.</summary>
public class WellWornGuitar : JoshuaRelic
{
    public const int VersesPerTurn = 1;
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Signature Guitar.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<SignatureGuitar>().ToMutable();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (player.Creature.CombatState == null) return;
        Flash();
        await Verses.Gain(choiceContext, player, VersesPerTurn, null);
    }
}
