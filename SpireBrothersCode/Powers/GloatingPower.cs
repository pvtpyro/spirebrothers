using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Captain Blockbeard gloating after nobody answered his insult: Amount extra Strength until his next attack, then it
/// goes away. InsultDuelPower applies it at the start of the players' turn, so his attack intent already shows the
/// bigger number while they plan their Block. His attack moves call <see cref="Spend"/> afterwards.
/// </summary>
public class GloatingPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Gives the Strength now and records it here so it can be taken back.</summary>
    public static async Task Gloat(PlayerChoiceContext ctx, Creature pirate, int amount)
    {
        await PowerCmd.Apply<StrengthPower>(ctx, pirate, amount, pirate, null);
        await PowerCmd.Apply<GloatingPower>(ctx, pirate, amount, pirate, null);
    }

    /// <summary>After one of his attacks: the gloating is used up, so take the Strength back.</summary>
    public static async Task Spend(Creature pirate)
    {
        if (pirate.GetPower<GloatingPower>() is not { } gloating) return;
        decimal amount = gloating.Amount;
        await PowerCmd.Remove(gloating);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), pirate, -amount, pirate, null);
    }
}
