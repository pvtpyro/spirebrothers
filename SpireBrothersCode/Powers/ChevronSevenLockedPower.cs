using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Stargate SG-1. Each Logic or Hands card you play locks a chevron; at 7 the gate opens (the kawoosh), dealing Amount
/// damage to ALL enemies, and the dialing starts over. Amount is the damage; the icon shows the chevrons locked.
/// </summary>
public class ChevronSevenLockedPower : BrothersPower
{
    public const int Chevrons = 7;

    private int _locked;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _locked;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || CombatState == null) return;
        var keywords = cardPlay.Card.Keywords;
        if (!keywords.Contains(BrotherKeywords.Logic) && !keywords.Contains(BrotherKeywords.Hands)) return;
        _locked++;
        if (_locked >= Chevrons)
        {
            _locked = 0;
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
