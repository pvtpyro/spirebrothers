using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Dungeon Master. Your Rolls get +4 per stack, up to 20 (read by Dice.Roll).</summary>
public class DungeonMasterPower : BrothersPower
{
    public const int Bonus = 4;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
