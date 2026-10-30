using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On Apophis, once: while the Sarcophagus stands, falling doesn't end him. The Jaffa carry him in, and on his next turn
/// he rises (Apophis.Rise, which removes this). Copies vanilla's AdaptablePower (the Test Subject's revive): he stays in
/// combat, can't be hit while he lies there, and the fight can't end. Break the Sarcophagus first, or while he lies in
/// it, and he stays down: once it's gone this stops holding the fight open.
/// </summary>
public class SarcophagusPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _rising;

    public bool Rising => _rising;

    private bool SarcophagusStands => Owner.CombatState?.Enemies.Any(e => e.IsAlive && e.Monster is Sarcophagus) == true;

    /// <summary>True if Apophis falling right now would send him into the Sarcophagus.</summary>
    public bool WillRise => SarcophagusStands;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Owner || !SarcophagusStands || Owner.Monster is not Apophis apophis) return;
        _rising = true;
        await apophis.CarriedIn();
    }

    /// <summary>Called by Apophis.Rise as he gets up.</summary>
    public void Risen() => _rising = false;

    // While he lies in the Sarcophagus nothing can touch him (or put powers on him).
    public override bool ShouldAllowHitting(Creature creature) => creature != Owner || !_rising;

    public override bool ShouldStopCombatFromEnding() => SarcophagusStands;

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature) => creature != Owner || !SarcophagusStands;

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;
}
