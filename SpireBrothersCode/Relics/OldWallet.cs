using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>David's starter (he never saw a reason to replace it). At the start of each combat, gain 1 Block per 20 Gold (max 15).</summary>
public class OldWallet : DavidRelic
{
    public const int GoldPerBlock = 20;
    public const int MaxBlock = 15;
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        int block = Math.Min(MaxBlock, player.Gold / GoldPerBlock);
        if (block <= 0) return;
        Flash();
        await CreatureCmd.GainBlock(player.Creature, block, ValueProp.Unpowered, null);
    }
}
