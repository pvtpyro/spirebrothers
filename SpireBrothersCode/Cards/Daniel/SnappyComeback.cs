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

/// <summary>Monkey Island. Comeback: deal 6 damage, +4 per Insulted on the target, then remove Insulted.</summary>
public class SnappyComeback() : DanielCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Comeback];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6),
        new ExtraDamageVar(4),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(InsultStacks)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    // Damage is base + ExtraDamage * this. Scaling the stack count folds Monkey Wrench's per-stack bonus in.
    private static decimal InsultStacks(CardModel card, Creature? target)
    {
        var stacks = Comeback.Stacks(target);
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
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
}
