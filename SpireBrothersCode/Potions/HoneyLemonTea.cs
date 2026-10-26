using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>A singer's throat remedy, made for everyone. ALL players heal 8 HP.</summary>
public class HoneyLemonTea : JoshuaPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(8)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var combat = Owner.Creature.CombatState;
        if (combat == null) return;
        Mechanics.Sounds.Play(Mechanics.Sounds.JoshuaHeal);
        foreach (var player in combat.Players.Where(p => p.Creature.IsAlive).ToList())
            await CreatureCmd.Heal(player.Creature, DynamicVars.Heal.BaseValue);
    }
}
