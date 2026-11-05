using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards.Danny;

/// <summary>The Stryder's Core (Mechanics/Chassis), handed over on Titanfall. Gain 2 Energy. Draw 3 cards.</summary>
public class DashCore() : DannyCard(0, CardType.Skill, CardRarity.Token, TargetType.Self), ICoreCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2), new CardsVar(3)];

    protected override bool IsPlayable => base.IsPlayable && InTitan;
    protected override bool ExtraGlow => InTitan;

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;

    protected override async Task OnTitan(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await Draw(ctx, DynamicVars.Cards.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
