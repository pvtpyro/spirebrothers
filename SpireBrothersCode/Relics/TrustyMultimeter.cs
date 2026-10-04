using MegaCrit.Sts2.Core.Entities.Relics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>The first time each turn a Wired bonus triggers, gain 3 Block. Logic lives in Mechanics/Wired.cs.</summary>
public class TrustyMultimeter : DanielRelic
{
    public const int BlockAmount = 3;
    public override RelicRarity Rarity => RelicRarity.Common;
}
