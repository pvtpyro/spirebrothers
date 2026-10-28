using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Huffman (his dog). The Mastiff starts each combat with 2 more Bite (added in Mechanics/Mastiff.Spawn).</summary>
public class ChewToy : HuffmanRelic
{
    public const int ExtraBite = 2;
    public override RelicRarity Rarity => RelicRarity.Common;

    public void OnSpawn() => Flash();
}
