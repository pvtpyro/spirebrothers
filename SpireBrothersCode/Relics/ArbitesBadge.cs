using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Huffman's starter. The first Power you play each combat costs 0 (his Laws are Powers). Works like vanilla Brilliant Scarf.</summary>
public class ArbitesBadge : HuffmanRelic
{
    public virtual int FreePowers => 1;
    private int _powersPlayed;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => false;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into the Judge's Badge.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<JudgesBadge>().ToMutable();

    private bool Free(CardModel card) => card.Owner == Owner && card.Type == CardType.Power && _powersPlayed < FreePowers;

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!Free(card)) return false;
        modifiedCost = 0;
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Power && !cardPlay.IsAutoPlay && _powersPlayed < FreePowers)
        {
            _powersPlayed++;
            Flash();
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _powersPlayed = 0;
        return Task.CompletedTask;
    }
}

/// <summary>Touch of Orobas upgrade of the Arbites Badge: the first 2 Powers each combat cost 0.</summary>
public class JudgesBadge : ArbitesBadge
{
    public override int FreePowers => 2;
    public override RelicModel? GetUpgradeReplacement() => null;
}
