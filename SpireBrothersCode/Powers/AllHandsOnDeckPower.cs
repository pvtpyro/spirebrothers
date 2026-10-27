using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>All Hands on Deck (Ancient). Gain a Kid at the start of each turn; Kids deal +1 damage per Age (read in Kids.DamagePerKid).</summary>
public class AllHandsOnDeckPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || Kids.Count(player) >= Kids.Max) return;
        Flash();
        await Kids.Gain(choiceContext, player, 1, null);
    }
}
