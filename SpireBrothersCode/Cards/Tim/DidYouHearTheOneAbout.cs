using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>
/// Ancient (Archaic Tooth, replaces Dad Joke): the joke so bad it hits the whole room. Deal 8 damage to ALL enemies.
/// Apply 2 Insulted and 1 Weak to ALL enemies.
/// </summary>
public class DidYouHearTheOneAbout() : TimCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies), IInsultCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new PowerVar<InsultedPower>(2), new PowerVar<WeakPower>(1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
        if (CombatState == null) return;
        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        await PowerCmd.Apply<InsultedPower>(ctx, enemies, DynamicVars["InsultedPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(ctx, enemies, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["InsultedPower"].UpgradeValueBy(1);
    }
}
