using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>John (always grinding for loot). Whenever one of your cards Grinds, gain 2 Gold (paid in JohnCard.GrindTimes).</summary>
public class LootChest : JohnRelic
{
    public const int GoldPerGrind = 2;
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public async Task OnGrind(int times)
    {
        if (Owner == null) return;
        Flash();
        await MegaCrit.Sts2.Core.Commands.PlayerCmd.GainGold(GoldPerGrind * times, Owner);
    }
}
