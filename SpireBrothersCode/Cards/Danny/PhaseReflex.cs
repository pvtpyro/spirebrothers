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

/// <summary>Titanfall. Pilot: at the start of your turn, gain 4 Block. Titan (Sword Core): gain 3 Strength.</summary>
public class PhaseReflex() : DannyCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Reflex", 4), new PowerVar<StrengthPower>(3)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Apply<PhaseReflexPower>(ctx, Owner.Creature, DynamicVars["Reflex"].BaseValue);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Apply<StrengthPower>(ctx, Owner.Creature, DynamicVars["StrengthPower"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars["Reflex"].UpgradeValueBy(2);
        DynamicVars["StrengthPower"].UpgradeValueBy(1);
    }
}
