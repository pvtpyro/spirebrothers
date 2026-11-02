using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Danny (Titanfall: the battery you rip out of an enemy Titan). Gain 20 Titan Meter, or Repair 20 if your Titan stands.</summary>
public class TitanBattery : DannyPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Meter", 20)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        int amount = DynamicVars["Meter"].IntValue;
        if (Titan.IsIn(Owner)) await Titan.Repair(Owner, amount);
        else await Titan.GainMeter(Owner, amount);
    }
}
