using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Lucky (the D&amp;D feat). The first 1 you roll each turn is rolled again (read by Dice.Roll).</summary>
public class LuckyPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _usedRound = -1;

    public bool TryUse()
    {
        int round = CombatState?.RoundNumber ?? 0;
        if (_usedRound == round) return false;
        _usedRound = round;
        Flash();
        return true;
    }
}
