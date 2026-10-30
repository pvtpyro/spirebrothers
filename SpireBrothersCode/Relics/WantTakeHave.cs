using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Buffy (her favorite show): Faith's motto. Your first Attack each combat deals double damage.</summary>
public class WantTakeHave : BrandiRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    // Combat-only, so it isn't saved.
    private bool _used;

    private bool Applies(Creature? dealer, ValueProp props, CardModel? card) =>
        !_used && Owner != null && dealer == Owner.Creature && props.IsPoweredAttack() && card?.Type == CardType.Attack;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource) =>
        Applies(dealer, props, cardSource) ? 2m : 1m;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_used || Owner == null || cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack) return Task.CompletedTask;
        _used = true;
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _used = false;
        return Task.CompletedTask;
    }
}
