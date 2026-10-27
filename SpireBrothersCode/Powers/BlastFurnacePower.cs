using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Tim's Blast Furnace (Age of Empires). His cards deal Amount more damage and give Amount more Block for each Age
/// counting the Dark Age (Dark 1, Feudal 2, Castle 3, Imperial 4), on top of the normal Age bonus in TimTracker.
/// </summary>
public class BlastFurnacePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private decimal Bonus => Amount * (1 + Ages.Get(Owner.Player));

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || cardSource?.Owner?.Creature != Owner || !props.IsPoweredAttack()) return 0m;
        return Bonus;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource?.Owner?.Creature != Owner || !props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
        return Bonus;
    }
}
