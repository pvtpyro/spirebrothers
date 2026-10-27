using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Space Base. Each card you play scores 2 points; at 40, deal Amount damage to ALL enemies and start over.
/// Amount is the damage (40 per copy); the icon shows the points instead.
/// </summary>
public class RaceToFortyPower : BrothersPower
{
    public const int Goal = 40;
    public const int PointsPerCard = 2;

    private int _points;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _points;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || CombatState == null) return;
        _points += PointsPerCard;
        if (_points >= Goal)
        {
            _points -= Goal;
            Flash();
            foreach (var enemy in CombatState.HittableEnemies.Where(e => e.IsAlive).ToList())
            {
                VfxCmd.PlayOnCreature(enemy, "vfx/vfx_heavy_blunt");
                await CreatureCmd.Damage(choiceContext, enemy, Amount, ValueProp.Unpowered, Owner);
            }
        }
        InvokeDisplayAmountChanged();
    }
}
