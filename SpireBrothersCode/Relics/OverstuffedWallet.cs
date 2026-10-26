using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// David's upgraded starter (from Touch of Orobas, replacing Old Wallet): at the start of each combat, ALL players gain 1 Block per
/// 15 Gold (max 20).
/// </summary>
public class OverstuffedWallet : DavidRelic
{
    public const int GoldPerBlock = 15;
    public const int MaxBlock = 20;
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FamilyBondsPower>()];

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
