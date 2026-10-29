using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// In memory of Pugsley, the fawn pug she got with her late husband. When you Rest at a rest site, heal 10 more HP: a
/// pug napping in your lap. In Brandi's pool, and added to Daniel's in MainFile.
/// </summary>
public class PugsleysCollar : BrandiRelic
{
    public const int ExtraHeal = 10;
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override decimal ModifyRestSiteHealAmount(Creature creature, decimal amount)
    {
        if (Owner == null || creature != Owner.Creature) return amount;
        return amount + ExtraHeal;
    }
}
