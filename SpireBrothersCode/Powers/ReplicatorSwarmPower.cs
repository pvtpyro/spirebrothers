using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.Shared;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On the Replicator Queen. The number is her loose blocks (ReplicatorQueen.Blocks): her Build move turns them into
/// Replicator bugs, and every bug that falls apart adds one back. Also the fight's FoePower: a random player starts with
/// the Anti-Replicator Gun.
/// </summary>
public class ReplicatorSwarmPower : FoePower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<AntiReplicatorGun>();

    private ReplicatorQueen? Queen => Owner?.Monster as ReplicatorQueen;

    public override int DisplayAmount => Queen?.Blocks ?? 0;

    public void Refresh() => InvokeDisplayAmountChanged();

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Monster is not ReplicatorBug || Owner.IsDead || Queen is not { } queen)
            return Task.CompletedTask;
        queen.Blocks++;
        Flash();
        Refresh();
        return Task.CompletedTask;
    }
}
