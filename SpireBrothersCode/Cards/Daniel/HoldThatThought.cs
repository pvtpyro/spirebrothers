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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>
/// Duo with David: he rants, Daniel interrupts with the comeback. Comeback: deal 8 damage, +3 per Insulted on the
/// target, then remove it. If David played a Rant this turn, the Insulted bonus is doubled.
/// </summary>
public class HoldThatThought() : DanielCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDuoCard
{
    public Type Partner => typeof(Brothers.David);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Duo, BrotherKeywords.Comeback];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(8),
        new ExtraDamageVar(3),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(InsultStacks)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<InsultedPower>(), HoverTipFactory.FromKeyword(BrotherKeywords.Rant)];

    // Glows once David has Ranted this turn, since that's when the bonus doubles.
    protected override bool ShouldGlowGoldInternal => base.ShouldGlowGoldInternal || (IsInCombat && DavidRanted(Owner));

    private static bool DavidRanted(MegaCrit.Sts2.Core.Entities.Players.Player? owner) =>
        DuoCards.Partners(owner?.Creature?.CombatState, typeof(Brothers.David)).Any(p => DavidTracker.RantsThisTurn(p) > 0);

    // Same shape as Snappy Comeback (Monkey Wrench folded in), doubled when David has Ranted.
    private static decimal InsultStacks(CardModel card, Creature? target)
    {
        var stacks = Comeback.Stacks(target) * (DavidRanted(card.Owner) ? 2 : 1);
        var extra = card.DynamicVars.ExtraDamage.BaseValue;
        return extra <= 0 ? stacks : stacks * (extra + Comeback.PerInsultBonus(card.Owner)) / extra;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_slash").Execute(ctx);
        await Comeback.ClearInsulted(Owner, play.Target);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
