using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Stryder's passive (Mechanics/Chassis): while his Titan stands, he draws Amount more cards each turn. Given on
/// Titanfall, removed on eject. Type None so nothing strips it.
/// </summary>
public class ThrustersPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHandDraw(Player player, decimal count) =>
        player.Creature == Owner && Titan.IsIn(player) ? count + Amount : count;
}
