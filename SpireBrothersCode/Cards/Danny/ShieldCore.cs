using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards.Danny;

/// <summary>
/// The Ogre's Core (Mechanics/Chassis), handed over on Titanfall. Gain 20 Block: the one card of his that can give him
/// Block in the Titan (GiveBlock has no card source, so Titan.BlocksBlock lets it through), and his Block soaks hits
/// before they reach the Titan.
/// </summary>
public class ShieldCore() : DannyCard(0, CardType.Skill, CardRarity.Token, TargetType.Self), ICoreCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Shield", 20)];

    protected override bool IsPlayable => base.IsPlayable && InTitan;
    protected override bool ExtraGlow => InTitan;

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => GiveBlock(DynamicVars["Shield"].BaseValue);

    protected override void OnUpgrade() => DynamicVars["Shield"].UpgradeValueBy(8);
}
