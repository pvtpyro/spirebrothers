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

/// <summary>Share: next turn, a player gains 1 energy and draws 1 card.</summary>
public class LendAHand() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var target = ShareTarget(play);
        await PowerCmd.Apply<EnergyNextTurnPower>(ctx, target, DynamicVars.Energy.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, target, DynamicVars.Cards.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
