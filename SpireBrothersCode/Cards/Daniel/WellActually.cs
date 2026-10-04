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
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new DynamicVar("PerInsult", 3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            var insult = enemy.GetPower<InsultedPower>();
            decimal damage = DynamicVars.Damage.BaseValue + DynamicVars["PerInsult"].BaseValue * (insult?.Amount ?? 0);
            await DamageCmd.Attack(damage).FromCard(this).Targeting(enemy).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
            if (insult != null && enemy.IsAlive) await PowerCmd.Remove(insult);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["PerInsult"].UpgradeValueBy(1);
    }
}
