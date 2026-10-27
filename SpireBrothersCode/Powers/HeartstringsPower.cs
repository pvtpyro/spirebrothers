using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Joshua (ballads). Whenever any player plays a Song, this creature loses HP equal to Heartstrings (ignores Block).
/// Goes down by 1 at the end of its turn, unless a player has Haunting Melody.
/// </summary>
public class HeartstringsPower : BrothersPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Amount <= 0 || !Owner.IsAlive || cardPlay.Card.Owner == null) return;
        if (!cardPlay.Card.Keywords.Contains(BrotherKeywords.Song)) return;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || !Owner.IsAlive) return;
        if (CombatState?.Players.Any(p => p.Creature.HasPower<HauntingMelodyPower>()) ?? false) return;
        if (Amount <= 1) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
    }
}
