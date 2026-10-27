using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// David's Rants. Until the end of its turn, its attacks deal <see cref="PerStack"/>% less damage per stack, up to
/// <see cref="Cap"/>%. (Was a flat 1 less per hit per stack until 2026-10-27, which was too weak. The cap keeps it
/// from stacking with Weak into harmless enemies, the reason the old extra 25% cut was dropped.) A damage modifier,
/// not Strength, so Artifact can't make it backfire.
/// </summary>
public class RantedAtPower : BrothersPower
{
    public const int PerStack = 15;
    public const int Cap = 40;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Shown in the tooltip as {Percent}; kept in step with Amount below.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", 0)];

    private int Percent => Math.Min(Amount * PerStack, Cap);

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 1m;
        return 1m - Percent / 100m;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        DynamicVars["Percent"].BaseValue = Percent;
        return base.AfterApplied(applier, cardSource);
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this) DynamicVars["Percent"].BaseValue = Percent;
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
