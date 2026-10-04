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

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Splinter Cell. Deal 8 damage. If the enemy isn't attacking this turn, deal 6 more.</summary>
public class Takedown() : TimCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(8),
        new ExtraDamageVar(6),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(NotAttacking)
    ];

    // Same intent check as vanilla Go for the Eyes.
    private static decimal NotAttacking(CardModel card, Creature? target) =>
        target?.Monster is { IntendsToAttack: false } ? 1 : 0;

    protected override bool ExtraGlow => CombatState?.HittableEnemies.Any(e => e.Monster is { IntendsToAttack: false }) ?? false;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.ExtraDamage.UpgradeValueBy(3);
}
