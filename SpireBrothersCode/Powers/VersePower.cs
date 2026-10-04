using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Joshua's Verses. Song cards add them, Chorus cards spend them all. See Mechanics/Verses.cs.</summary>
public class VersePower : BrothersPower
{
    // Type None so enemy buff-stripping can't take them.
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;
}
