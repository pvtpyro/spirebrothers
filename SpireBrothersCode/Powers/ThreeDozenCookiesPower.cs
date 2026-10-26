using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Cards.David;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// David's cookie order. Amount is the cookies still to bake. At the start of your turn, add a batch of Cookies to
/// your hand and count them off; the order (and this power) is done when it reaches 0.
/// </summary>
public class ThreeDozenCookiesPower : BrothersPower
{
    public const int OrderSize = 36;
    public const int BatchSize = 6;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState == null || Amount <= 0) return;
        int batch = Math.Min(BatchSize, Amount);
        Flash();
        for (int i = 0; i < batch; i++)
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Cookie>(player), PileType.Hand, player);
        if (Amount - batch <= 0) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(choiceContext, this, -batch, null, null);
    }
}
