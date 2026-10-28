using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>John (Enshrouded). The Shroud no longer costs you HP (read in Mechanics/Shroud.HpLoss).</summary>
public class SurvivorsLantern : JohnRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}
