using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Tim's upgraded starter (from Touch of Orobas, replacing Family Minivan): start each combat with 4 Kids.</summary>
public class FifteenPassengerVan : TimRelic
{
    public const int StartingKids = 4;
    public const int PocketMoney = 15;
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FamilyBondsPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        await Kids.Gain(choiceContext, player, StartingKids, null);
    }

    // Gold burns a hole in his pocket, so there's always a little more to spend.
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner == null) return;
        Flash();
        await PlayerCmd.GainGold(PocketMoney, Owner);
    }
}
