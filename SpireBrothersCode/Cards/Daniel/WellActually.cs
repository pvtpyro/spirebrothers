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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Monkey Island. Comeback: deal 5 damage to ALL enemies, +3 per Insulted on each, then remove their Insulted.</summary>
public class WellActually() : DanielCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Comeback];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(5),
        new ExtraDamageVar(3),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(InsultStacks)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    // Damage is base + ExtraDamage * this, per enemy. Scaling the stack count folds Monkey Wrench's per-stack bonus in (like Snappy Comeback).
    private static decimal InsultStacks(CardModel card, Creature? target)
    {
        var stacks = Comeback.Stacks(target);
        var extra = card.DynamicVars.ExtraDamage.BaseValue;
        return extra <= 0 ? stacks : stacks * (extra + Comeback.PerInsultBonus(card.Owner)) / extra;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            decimal damage = DynamicVars.CalculatedDamage.Calculate(enemy);
            await DamageCmd.Attack(damage).FromCard(this).Targeting(enemy).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
            await Comeback.ClearInsulted(Owner, enemy);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
