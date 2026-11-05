using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Atlas's passive (Mechanics/Chassis): while his Titan stands, his attacks deal Amount more damage. Given on
/// Titanfall, removed on eject. Type None so nothing strips it.
/// </summary>
public class TargetingLinkPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || cardSource?.Owner?.Creature != Owner || !props.IsPoweredAttack() || !Titan.IsIn(Owner.Player)) return 0m;
        return Amount;
    }
}
