using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>At the end of each combat, gain 5% interest on your Gold (max 20). He was never going to spend it anyway.</summary>
public class SavingsAccount : DavidRelic
{
    public const int InterestPercent = 5;
    public const int MaxInterest = 20;
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        int interest = Math.Min(MaxInterest, Owner.Gold * InterestPercent / 100);
        if (interest <= 0) return;
        Flash();
        await PlayerCmd.GainGold(interest, Owner);
    }
}
