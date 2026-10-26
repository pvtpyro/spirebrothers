using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Age of Empires: a relic garrisoned in a monastery trickles in gold. At the end of each combat, gain 12 Gold.</summary>
public class GarrisonedRelic : TimRelic
{
    public const int GoldAmount = 12;
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        Flash();
        await PlayerCmd.GainGold(GoldAmount, Owner);
    }
}
