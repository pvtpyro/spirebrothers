using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Realm First! (Ancient). Read in JohnCard.GrindAfterPlay: the once-per-combat rule for Grind is gone.</summary>
public class RealmFirstPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
