using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Starter: if you played both a Logic and a Hands card last turn, draw 1 at the start of your turn.</summary>
public class AndYouKnowWhat : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FamilyBondsPower>()];

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Well, Technically.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<WellTechnically>().ToMutable();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.PlayerCombatState == null) return;
        if (!TurnTracker.MixedLastTurn.Get(player.PlayerCombatState)) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, 1, player);
    }
}
