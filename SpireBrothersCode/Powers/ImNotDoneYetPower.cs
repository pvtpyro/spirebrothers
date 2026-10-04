using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The first time you would die this combat, heal Amount% of your Max HP instead, then this power goes away.
/// Same hooks as the vanilla Lizard Tail relic.
/// </summary>
public class ImNotDoneYetPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldDieLate(Creature creature) => creature != Owner;

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner) return;
        Flash();
        decimal amount = Math.Max(1m, creature.MaxHp * (Amount / 100m));
        await CreatureCmd.Heal(creature, amount);
        await PowerCmd.Remove(this);
    }
}
