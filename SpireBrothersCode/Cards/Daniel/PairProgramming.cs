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

/// <summary>Share: a player draws 1 extra card at the start of each of their turns.</summary>
public class PairProgramming() : DanielCard(2, CardType.Power, CardRarity.Rare, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PairProgrammingPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.Apply<PairProgrammingPower>(ctx, ShareTarget(play), this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
