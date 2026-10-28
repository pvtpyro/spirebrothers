using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Enshrouded. Whenever John leaves the Shroud, he gains Amount Energy and draws 2 cards per stack.</summary>
public class ElixirWellPower : BrothersPower
{
    public const int CardsPerStack = 2;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnLeftShroud(PlayerChoiceContext ctx)
    {
        if (Owner.Player is not { } player) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount * 2, player);
        await MegaCrit.Sts2.Core.Commands.CardPileCmd.Draw(ctx, Amount * CardsPerStack, player);
    }
}
