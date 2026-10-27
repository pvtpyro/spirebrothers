using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Spreadsheet of Destiny (Ancient). Exact also triggers with 1 energy left and triggers an extra time (both read by
/// Mechanics/Exact.cs). The catch: no energy gain outside the turn-start refill, like vanilla's NoEnergyGainPower
/// (the refill doesn't go through ModifyEnergyGain).
/// </summary>
public class SpreadsheetOfDestinyPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyEnergyGain(Player player, decimal amount) =>
        player == Owner.Player ? 0m : amount;

    public override Task AfterModifyingEnergyGain()
    {
        Flash();
        return Task.CompletedTask;
    }
}
