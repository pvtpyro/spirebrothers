using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>David's Mattress Money. Your Attacks deal Amount extra damage per 100 Gold you have.</summary>
public class MattressMoneyPower : BrothersPower
{
    public const int GoldPerBonus = 100;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource?.Type != CardType.Attack) return 0m;
        return Amount * ((Owner.Player?.Gold ?? 0) / GoldPerBonus);
    }
}
