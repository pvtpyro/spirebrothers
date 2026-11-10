using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On the Ender Dragon while she's in the air: Attacks deal half damage to her (like Kull Armor). Swapped for PerchedPower
/// when she lands (EnderDragon.Land) and back when she takes off. Type None so nothing strips it.
/// </summary>
public class CirclingPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack()) return 1m;
        return 0.5m;
    }
}
