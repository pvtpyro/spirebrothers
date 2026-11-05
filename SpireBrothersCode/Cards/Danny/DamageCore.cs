using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Danny;

/// <summary>The Atlas's Core (Mechanics/Chassis), handed over on Titanfall. This turn, your Titan's attacks deal 50% more damage.</summary>
public class DamageCore() : DannyCard(0, CardType.Skill, CardRarity.Token, TargetType.Self), ICoreCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", 50)];

    protected override bool IsPlayable => base.IsPlayable && InTitan;
    protected override bool ExtraGlow => InTitan;

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Apply<DamageCorePower>(ctx, Owner.Creature, DynamicVars["Percent"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["Percent"].UpgradeValueBy(25);
}
