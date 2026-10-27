using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>D&amp;D. Your next Amount Rolls roll twice and keep the higher (used up in Dice.Roll).</summary>
public class AdvantagePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
