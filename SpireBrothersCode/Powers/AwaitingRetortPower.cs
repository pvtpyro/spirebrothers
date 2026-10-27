using MegaCrit.Sts2.Core.Entities.Powers;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Shown on Captain Blockbeard while his insult is waiting for an answer, so the party can see it's their move.
/// InsultDuelPower adds and removes it; it does nothing on its own.
/// </summary>
public class AwaitingRetortPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;
}
