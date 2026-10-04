using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Monkey Island pirate drink. Deal 12 damage to ALL enemies and apply 2 Insulted to ALL enemies.</summary>
public class Grog : DanielPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AllEnemies;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Unpowered), new PowerVar<InsultedPower>(2)];
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var player = Owner.Creature;
        if (player.CombatState == null) return;
        var damage = DynamicVars.Damage;
        await CreatureCmd.Damage(choiceContext, player.CombatState.HittableEnemies, damage.BaseValue, damage.Props, player, null);
        await PowerCmd.Apply<InsultedPower>(choiceContext, player.CombatState.HittableEnemies, DynamicVars["InsultedPower"].BaseValue, player, null);
    }
}
