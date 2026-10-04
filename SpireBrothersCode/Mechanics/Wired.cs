using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>Wired: a Logic card is Wired if you already played a Hands card this turn, and vice versa.</summary>
public static class Wired
{
    public static bool IsActive(CardModel card)
    {
        var owner = card.Owner;
        if (owner?.Creature == null) return false;
        if (owner.Creature.Powers.Any(p => p is NerdsWrongPower)) return true;

        bool logic = card.Keywords.Contains(BrotherKeywords.Logic);
        bool hands = card.Keywords.Contains(BrotherKeywords.Hands);
        if (logic && TurnTracker.PlayedThisTurn(owner, BrotherKeywords.Hands)) return true;
        if (hands && TurnTracker.PlayedThisTurn(owner, BrotherKeywords.Logic)) return true;
        return false;
    }

    /// <summary>Call from OnPlay. Returns whether the Wired bonus should happen, and fires once-per-turn effects.</summary>
    public static async Task<bool> Check(CardModel card)
    {
        if (!IsActive(card)) return false;

        var state = card.Owner.PlayerCombatState;
        if (state != null && !TurnTracker.WiredFiredThisTurn.Get(state))
        {
            TurnTracker.WiredFiredThisTurn.Set(state, true);
            var multimeter = card.Owner.GetRelic<TrustyMultimeter>();
            if (multimeter != null)
            {
                multimeter.Flash();
                await CreatureCmd.GainBlock(card.Owner.Creature, TrustyMultimeter.BlockAmount, ValueProp.Unpowered, null);
            }
        }
        return true;
    }
}
