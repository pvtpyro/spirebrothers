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
/// Enshrouded. At the end of each of John's turns in the Shroud, he gains Amount Block per depth (done in ShroudPower).
/// Until 2026-11-05 it made the Shroud last longer and cost no HP, which worked against his lose-HP-to-hit-hard style.
/// </summary>
public class FlameAltarUpgradePower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnShroudTurnEnd(int depth)
    {
        if (depth <= 0) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, depth * Amount, ValueProp.Unpowered, null);
    }
}
