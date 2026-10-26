using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>A quick Lua patch, pushed straight to production. Your next 3 cards this turn have Script (uses Snap to Grid's power).</summary>
public class Hotfix : TimPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Script)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<SnapToGridPower>(choiceContext, Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature, null);
    }
}
