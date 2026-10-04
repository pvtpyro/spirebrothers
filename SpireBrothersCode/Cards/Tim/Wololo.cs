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

/// <summary>Age of Empires (monk conversion!). An enemy loses 2 Strength. You gain 2 Strength this turn.</summary>
public class Wololo() : TimCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Convert", 2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var amount = DynamicVars["Convert"].BaseValue;
        if (play.Target != null) await PowerCmd.Apply<StrengthPower>(ctx, play.Target, -amount, Owner.Creature, this);
        // Vanilla Flex Potion's power: Strength that wears off at the end of the turn.
        await PowerCmd.Apply<FlexPotionPower>(ctx, Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Convert"].UpgradeValueBy(1);
}
