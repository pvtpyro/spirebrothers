using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Tim's Burning a Hole. Whenever his cards spend Gold, deal Amount damage to ALL enemies per 10 Gold spent.</summary>
public class BurningAHolePower : BrothersPower
{
    public const int GoldPerHit = 10;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnGoldSpent(PlayerChoiceContext ctx, int spent)
    {
        int damage = spent / GoldPerHit * Amount;
        if (damage <= 0 || CombatState == null) return;
        Flash();
        foreach (var enemy in CombatState.HittableEnemies.ToList())
            await CreatureCmd.Damage(ctx, enemy, damage, ValueProp.Unpowered, Owner);
    }
}
