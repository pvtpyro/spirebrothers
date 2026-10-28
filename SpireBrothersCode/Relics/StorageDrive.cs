using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Huffman (AE2). Start each combat with 3 Metal.</summary>
public class StorageDrive : HuffmanRelic
{
    public const int StartingMetal = 3;
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.Creature.CombatState is not { RoundNumber: 1 }) return;
        Flash();
        await Metal.Gain(choiceContext, player, StartingMetal, null);
    }
}
