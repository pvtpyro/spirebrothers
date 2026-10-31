using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.Shared;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On Anubis. Cast out of the higher plane half-ascended, he can't truly die while he has a body to take: falling with a
/// Kull Warrior standing, his shroud takes it over instead (Anubis.Possess: the warrior drops, he gets back up with HP and
/// Strength, his debuffs gone). Kill the Kull first. Prevents the death like vanilla Lizard Tail (ShouldDieLate).
/// Also the fight's FoePower: a random player starts with the Anti-Kull Disruptor.
/// </summary>
public class HalfAscendedPower : FoePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<AntiKullDisruptor>();

    /// <summary>The Kull Warrior he'd take if he fell right now (the front one), or null.</summary>
    public Creature? Host => Owner.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is KullWarrior);

    public override bool ShouldDieLate(Creature creature) => creature != Owner || Host == null;

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner || Owner.Monster is not Anubis anubis || Host is not { } host) return;
        Flash();
        await anubis.Possess(host);
    }
}
