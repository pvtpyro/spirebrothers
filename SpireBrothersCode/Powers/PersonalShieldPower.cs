using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On Apophis: a Goa'uld personal shield stops anything fast, so no single hit deals more than Amount damage to him.
/// Many small hits beat one big one. Vanilla's damage cap hook (like Intangible, which caps at 1); it also covers Block
/// loss and the damage preview. A Jaffa turning on him (Shol'va) is standing inside the shield, so it's not capped.
/// </summary>
public class PersonalShieldPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer?.Monster is Jaffa) return decimal.MaxValue;
        return Amount;
    }

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }
}
