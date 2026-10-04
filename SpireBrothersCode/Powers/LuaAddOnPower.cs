using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Lua Add-On. Whenever a Script effect happens, deal Amount damage to a random enemy.</summary>
public class LuaAddOnPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task Trigger(PlayerChoiceContext ctx)
    {
        if (CombatState == null || Owner.Player == null || !Owner.IsAlive) return;
        var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) return;
        Flash();
        var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await CreatureCmd.Damage(ctx, target, Amount, ValueProp.Unpowered, Owner);
    }
}
