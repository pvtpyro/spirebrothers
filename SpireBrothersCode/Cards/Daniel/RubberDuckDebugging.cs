using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Scry 3. Draw 1. Wired: draw 1 more.</summary>
public class RubberDuckDebugging() : DanielCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override bool HasWiredBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScryVar(3), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue + (wired ? 1 : 0), Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Scry"].UpgradeValueBy(2);
}
