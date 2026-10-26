using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>David's hiking snack, shared with the whole trail. Heal 10 HP. ALL players gain 6 Block.</summary>
public class TrailMix : DavidPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(10), new BlockVar(6, ValueProp.Unpowered)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        var combat = Owner.Creature.CombatState;
        if (combat == null) return;
        foreach (var player in combat.Players.Where(p => p.Creature.IsAlive).ToList())
            await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block.BaseValue, ValueProp.Unpowered, null);
    }
}
