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

/// <summary>Apply 2 Vulnerable. Wired: apply it to ALL enemies instead.</summary>
public class MergeConflict() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool HasWiredBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        if (wired && CombatState != null)
            await PowerCmd.Apply<VulnerablePower>(ctx, CombatState.HittableEnemies, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        else if (play.Target != null)
            await CommonActions.Apply<VulnerablePower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade() => DynamicVars.Vulnerable.UpgradeValueBy(1);
}
