using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>David's starter (he never saw a reason to replace it). At the start of each combat, ALL players gain 1 Block per 25 Gold (max 12).</summary>
public class OldWallet : DavidRelic
{
    public const int GoldPerBlock = 25;
    public const int MaxBlock = 12;
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FamilyBondsPower>()];

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Overstuffed Wallet.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<OverstuffedWallet>().ToMutable();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        int block = Math.Min(MaxBlock, player.Gold / GoldPerBlock);
        if (block <= 0) return;
        Flash();
        // His savings protect the whole team.
        foreach (var p in combat.Players.Where(p => p.Creature.IsAlive).ToList())
            await CreatureCmd.GainBlock(p.Creature, block, ValueProp.Unpowered, null);
    }
}
