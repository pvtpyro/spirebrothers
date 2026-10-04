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

/// <summary>Token. Gain 2 Block, +1 for every Gear played this combat. Exhaust.</summary>
public class Gear() : DanielCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(2),
        new CalculationExtraVar(1),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(GearsSoFar)
    ];

    private static decimal GearsSoFar(CardModel card, Creature? _)
    {
        var state = card.Owner?.PlayerCombatState;
        return state == null ? 0 : TurnTracker.GearsPlayed.Get(state);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        var state = Owner.PlayerCombatState;
        if (state != null) TurnTracker.GearsPlayed.Set(state, TurnTracker.GearsPlayed.Get(state) + 1);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(2);
}
