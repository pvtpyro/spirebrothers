using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// Ancient (Archaic Tooth, replaces Here We Go Again): he's really going now. Deal 10 damage. Apply 3 Ranted At.
/// Rant: +4 damage per Rant this turn. Counts as 2 Rants (DavidTracker reads RantsCounted).
/// </summary>
public class DontGetMeStarted() : DavidCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public const int RantsCounted = 2;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Rant];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10),
        new ExtraDamageVar(4),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(RantsSoFar),
        new DynamicVar("RantStrength", 3)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RantedAtPower>()];

    private static decimal RantsSoFar(CardModel card, Creature? _) => DavidTracker.RantsThisTurn(card.Owner);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
        if (play.Target is { IsAlive: true }) await RantAt(ctx, play.Target, DynamicVars["RantStrength"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
