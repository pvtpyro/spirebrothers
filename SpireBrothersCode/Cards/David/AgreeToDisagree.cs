using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// Duo with Daniel: David starts the argument so Daniel can land the Comeback. Deal 6 damage and apply 2 Insulted.
/// Rant: +2 damage for each Rant played earlier this turn.
/// </summary>
public class AgreeToDisagree() : DavidCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDuoCard
{
    public Type Partner => typeof(Brothers.Daniel);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Rant];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6),
        new ExtraDamageVar(2),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(RantsSoFar),
        new PowerVar<InsultedPower>(2)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    private static decimal RantsSoFar(CardModel card, Creature? _) => DavidTracker.RantsThisTurn(card.Owner);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
        if (play.Target is { IsAlive: true }) await CommonActions.Apply<InsultedPower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars["InsultedPower"].UpgradeValueBy(1);
    }
}
