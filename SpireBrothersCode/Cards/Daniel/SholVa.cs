using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>
/// Token: Daniel starts the Apophis fight with one in hand (SystemLordPower). "Shol'va" is what Apophis called Teal'c,
/// his First Prime who turned on him. The front Jaffa hits Apophis from inside his Personal Shield (not capped), then
/// is Stunned. Playable only while a Jaffa and Apophis both stand. Retain, Exhaust.
/// </summary>
public class SholVa() : DanielCard(0, CardType.Skill, CardRarity.Token, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move)];

    private Creature? Turncoat => CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Jaffa);
    private Creature? Apophis => CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Monsters.Apophis);

    protected override bool IsPlayable => base.IsPlayable && Turncoat != null && Apophis != null;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (Turncoat is not { } jaffa || Apophis is not { } apophis) return;
        await CreatureCmd.TriggerAnim(jaffa, "Attack", 0.3f);
        await CreatureCmd.Damage(ctx, apophis, DynamicVars.Damage.BaseValue, ValueProp.Move, jaffa, this);
        if (jaffa.IsAlive) await CreatureCmd.Stun(jaffa, jaffa.Monster?.NextMove.Id);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
