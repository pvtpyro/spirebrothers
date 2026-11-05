using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Ogre's passive (Mechanics/Chassis): while his Titan stands, enemy attacks on his teammates deal Amount less
/// damage per hit (the big Titan covers the team). Not on Danny himself: his hits go to the Titan, which has its own
/// armor. Given on Titanfall, removed on eject. Type None so nothing strips it.
/// </summary>
public class BulwarkPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == null || target == Owner || !target.IsPlayer || target.Side != Owner.Side) return 0m;
        if (dealer == null || dealer.Side == Owner.Side || !props.IsPoweredAttack() || !Titan.IsIn(Owner.Player)) return 0m;
        return -Math.Min(Amount, Math.Max(0, amount));
    }
}
