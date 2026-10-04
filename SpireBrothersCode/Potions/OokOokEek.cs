using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Monkey Island. Apply 3 Insulted to ALL enemies. Description is monkey-speak.</summary>
public class OokOokEek : DanielPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AllEnemies;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<InsultedPower>(3)];
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
        [MonkeySpeak.Translation("potions", Id.Entry, DynamicVars), HoverTipFactory.FromPower<InsultedPower>()];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var player = Owner.Creature;
        if (player.CombatState == null) return;
        await PowerCmd.Apply<InsultedPower>(choiceContext, player.CombatState.HittableEnemies, DynamicVars["InsultedPower"].BaseValue, player, null);
    }
}
