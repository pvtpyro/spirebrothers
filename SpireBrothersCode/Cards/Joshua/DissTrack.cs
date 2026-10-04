using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>
/// Monkey Island (music). Chorus + Comeback: deal 3 damage to ALL enemies per Verse, +3 for each Insulted
/// on each enemy, then remove their Insulted.
/// </summary>
public class DissTrack() : JoshuaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Chorus, BrotherKeywords.Comeback];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new ExtraDamageVar(3),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(VersesAndInsults)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>()];

    // Each Verse and each Insulted stack is worth ExtraDamage; Monkey Wrench adds to the Insulted part.
    private static decimal VersesAndInsults(CardModel card, Creature? target)
    {
        var extra = card.DynamicVars.ExtraDamage.BaseValue;
        var stacks = Comeback.Stacks(target);
        var insults = extra <= 0 ? stacks : stacks * (extra + Comeback.PerInsultBonus(card.Owner)) / extra;
        return ChorusVerses(card, target) + insults;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await ChorusAttack(ctx, play, "vfx/vfx_heavy_blunt");
        if (CombatState == null) return;
        foreach (var enemy in CombatState.HittableEnemies.ToList()) await Comeback.ClearInsulted(Owner, enemy);
    }

    protected override void OnUpgrade() => DynamicVars.ExtraDamage.UpgradeValueBy(1);
}
