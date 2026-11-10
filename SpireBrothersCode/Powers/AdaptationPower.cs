using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On every Replicator (ReplicatorBoss). They adapt: whatever card last damaged a Replicator deals half damage to every
/// Replicator until a different card hits them. The swarm's memory is on the encounter (ReplicatorBoss.AdaptedTo), so
/// a fresh bug knows it too. It adapts once the card has finished (AfterCardPlayed), so an AoE or multi-hit card lands
/// whole the first time. Upgraded copies count as the same card. The Anti-Replicator Gun is never adapted to.
/// Damage that isn't from a card (Poison, Bleed, pets, powers) is never halved and teaches them nothing.
/// One hover box names the card. Type None so nothing strips it.
/// </summary>
public class AdaptationPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;

    private ReplicatorBoss? Boss => Owner?.CombatState?.Encounter as ReplicatorBoss;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(new LocString("powers", Id.Entry + ".adaptedTo"),
            Boss?.AdaptedTitle ?? new LocString("powers", Id.Entry + ".nothingYet").GetFormattedText()),
    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || Boss is not { } boss || !boss.IsAdaptedTo(cardSource)) return 1m;
        return 0.5m;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && cardSource != null && result.TotalDamage > 0) Boss?.NoteHurtBy(cardSource);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Every Replicator's copy runs this; the first one does the adapting, the rest find nothing pending.
        if (Owner?.CombatState is { } combat && Boss is { } boss && boss.Adapt(combat))
            foreach (var r in combat.Enemies.Where(e => e.IsAlive))
                if (r.GetPower<AdaptationPower>() is { } power) power.Flash();
        return Task.CompletedTask;
    }
}
