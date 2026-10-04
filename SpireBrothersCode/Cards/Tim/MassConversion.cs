using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Age of Empires (a monk army). ALL enemies lose 3 Strength. You gain that much Strength this turn. Exhaust.</summary>
public class MassConversion() : TimCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Convert", 3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        var amount = DynamicVars["Convert"].BaseValue;
        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) return;
        await PowerCmd.Apply<StrengthPower>(ctx, enemies, -amount, Owner.Creature, this);
        // Vanilla Flex Potion's power: Strength that wears off at the end of the turn.
        await PowerCmd.Apply<FlexPotionPower>(ctx, Owner.Creature, amount * enemies.Count, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Convert"].UpgradeValueBy(1);
}
