using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Encore. The first Chorus you play each turn (one per stack) spends only half your Verses (rounded down).</summary>
public class EncorePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _usedThisTurn;

    /// <summary>Called by Verses.SpendForChorus. True if this Chorus spends only half its Verses.</summary>
    public bool TryUse()
    {
        if (_usedThisTurn >= Amount) return false;
        _usedThisTurn++;
        Flash();
        return true;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner) _usedThisTurn = 0;
        return Task.CompletedTask;
    }
}
