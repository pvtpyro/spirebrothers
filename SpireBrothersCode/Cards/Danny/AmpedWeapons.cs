using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Danny;

/// <summary>Titanfall. Pilot: gain 2 Strength this turn. Titan (Smart Core): gain 4 Strength this turn.</summary>
public class AmpedWeapons() : DannyCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Strength", 2), new DynamicVar("TitanStrength", 4)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<FlexPotionPower>(ctx, Owner.Creature, DynamicVars["Strength"].BaseValue);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Apply<FlexPotionPower>(ctx, Owner.Creature, DynamicVars["TitanStrength"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(1);
        DynamicVars["TitanStrength"].UpgradeValueBy(1);
    }
}
