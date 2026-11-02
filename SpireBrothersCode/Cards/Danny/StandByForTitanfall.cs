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

/// <summary>Token, handed over when the Titan Meter fills (Mechanics/Titan). Call in your Titan. Retain. Exhaust.</summary>
public class StandByForTitanfall() : DannyCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> MoreHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.TitanMeter)];

    protected override bool IsPlayable => base.IsPlayable && Titan.IsReady(Owner);
    protected override bool ExtraGlow => Titan.IsReady(Owner);

    protected override Task OnPilot(PlayerChoiceContext ctx, CardPlay play) => Titan.Call(ctx, Owner);

    // Its Titan is already here.
    protected override Task OnTitan(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;
}
