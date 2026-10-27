using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>D&amp;D. Your first Roll each combat is a Natural 20 (read by Dice.Roll).</summary>
public class LuckyD20 : JoshuaRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private ICombatState? _usedIn;

    public bool TryUse()
    {
        var combat = Owner?.Creature?.CombatState;
        if (combat == null || _usedIn == combat) return false;
        _usedIn = combat;
        Flash();
        return true;
    }
}
