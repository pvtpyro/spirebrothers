using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On a Kull Warrior (AnubisBoss): Attacks deal half damage to it. Anything that isn't an Attack (Poison, Bleed, Ember,
/// Toxin, thorns) gets through in full. The Anti-Kull Disruptor token strips it for good.
/// </summary>
public class KullArmorPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack()) return 1m;
        return 0.5m;
    }
}
