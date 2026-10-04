using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
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

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>Monkey Island Comeback. Deal 10 damage, +5 for each Insulted on the target, then remove it.</summary>
public class Checkmate() : DavidCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Comeback];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10),
        new ExtraDamageVar(5),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(InsultStacks)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    private static decimal InsultStacks(CardModel card, Creature? target)
    {
        var stacks = Comeback.Stacks(target);
        var extra = card.DynamicVars.ExtraDamage.BaseValue;
        return extra <= 0 ? stacks : stacks * (extra + Comeback.PerInsultBonus(card.Owner)) / extra;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
        await Comeback.ClearInsulted(Owner, play.Target);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
