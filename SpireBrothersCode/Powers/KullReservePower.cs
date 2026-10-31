using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On Anubis: the Kull Warriors he can still call in with Kull, Rise! (2 for the whole fight; Anubis.KullRise spends it).
/// Gone at 0, and then that turn is Dark Shroud instead. Keeps the fight from refilling forever. Type None so nothing strips it.
/// </summary>
public class KullReservePower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;
}
