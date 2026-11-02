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

/// <summary>Titanfall. Pilot: draw 2. Titan (Tether Trap): apply 2 Vulnerable.</summary>
public class Grapple() : DannyCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new PowerVar<VulnerablePower>(2)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Draw(ctx, DynamicVars.Cards.BaseValue);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) =>
        Apply<VulnerablePower>(ctx, play.Target!, DynamicVars["VulnerablePower"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}
