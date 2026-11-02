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

/// <summary>Defend. Pilot: gain 5 Block. Titan (Electric Smoke): apply 1 Weak to ALL enemies.</summary>
public class DefendDanny() : DannyCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move), new PowerVar<WeakPower>(1)];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => await CommonActions.CardBlock(this, play);

    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => ApplyAll<WeakPower>(ctx, DynamicVars["WeakPower"].BaseValue);

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars["WeakPower"].UpgradeValueBy(1);
    }
}
