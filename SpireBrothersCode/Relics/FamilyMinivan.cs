using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Tim's starter. Start each combat with 2 Kids.</summary>
public class FamilyMinivan : TimRelic
{
    public const int StartingKids = 2;
    public const int PocketMoney = 10;
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FamilyBondsPower>()];

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Fifteen-Passenger Van.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<FifteenPassengerVan>().ToMutable();

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
