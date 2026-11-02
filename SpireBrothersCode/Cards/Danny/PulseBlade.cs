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

/// <summary>Titanfall. Pilot: apply 2 Vulnerable, draw 1. Titan (Sonar Lock): apply 2 Vulnerable to ALL enemies.</summary>
public class PulseBlade() : DannyCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(2), new CardsVar(1), new PowerVar<VulnerablePower>("TitanVulnerable", 2)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play)
    {
        await Apply<VulnerablePower>(ctx, play.Target!, DynamicVars["VulnerablePower"].BaseValue);
        await Draw(ctx, DynamicVars.Cards.BaseValue);
    }

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => ApplyAll<VulnerablePower>(ctx, DynamicVars["TitanVulnerable"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
        DynamicVars["TitanVulnerable"].UpgradeValueBy(1);
    }
}
