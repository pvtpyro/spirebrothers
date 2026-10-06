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

/// <summary>Helldivers 2. Costs 0. Requires Logic, Logic, Hands this turn. Deal 20 damage, and 6 damage to ALL other enemies.</summary>
public class StratagemEagle500kgBomb() : DanielCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IReadOnlyList<CardKeyword> StratagemCombo => [BrotherKeywords.Logic, BrotherKeywords.Logic, BrotherKeywords.Hands];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Stratagem];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20, ValueProp.Move), new DynamicVar("Splash", 6)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
        if (CombatState == null) return;
        foreach (var enemy in CombatState.HittableEnemies.Where(e => e.IsAlive && e != play.Target).ToList())
        {
            VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_blunt");
            await CreatureCmd.Damage(ctx, enemy, DynamicVars["Splash"].BaseValue, ValueProp.Move, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars["Splash"].UpgradeValueBy(3);
    }
}
