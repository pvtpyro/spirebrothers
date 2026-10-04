using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Monkey Island (and a nod to Daniel the mechanic). Comebacks get +2 per Insulted. Read by Mechanics/Comeback.cs.</summary>
public class MonkeyWrench : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [MonkeySpeak.Translation("relics", Id.Entry), HoverTipFactory.FromPower<InsultedPower>()];
}
