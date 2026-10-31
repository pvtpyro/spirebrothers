using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On Anubis: his mothership's superweapon. Amount is how many of his turns until it fires (1 = next turn); Anubis sets
/// it after each move. Only a countdown for the party to plan around. Type None so nothing strips it.
/// </summary>
public class SuperweaponPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;
}
