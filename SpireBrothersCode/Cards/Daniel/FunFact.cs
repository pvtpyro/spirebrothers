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

/// <summary>Draw 1. Share: a player draws 1 more card next turn. ("And you know what?")</summary>
public class FunFact() : DanielCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new DynamicVar("ShareCards", 1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, ShareTarget(play), DynamicVars["ShareCards"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
