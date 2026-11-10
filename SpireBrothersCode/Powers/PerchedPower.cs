using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.Shared;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On the Ender Dragon while she's landed on the portal (one player turn in three): Attacks deal 50% more damage to her,
/// and a Bed deals triple. Type None so nothing strips it.
/// </summary>
public class PerchedPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return 1m;
        if (cardSource is Bed) return 3m;
        return props.IsPoweredAttack() ? 1.5m : 1m;
    }
}
