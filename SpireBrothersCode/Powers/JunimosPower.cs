using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Junimos (Stardew Valley). Whenever you play a Chorus, add Amount random Song cards to your hand.</summary>
public class JunimosPower : BrothersPower, IChorusListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnChorus(PlayerChoiceContext choiceContext, Player player, int verses)
    {
        Flash();
        await Verses.AddRandomSongs(player, Amount, freeThisTurn: false);
    }
}
