using BaseLib.Cards.Variables;
using BaseLib.Commands;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Legos + kids. ALL enemies lose HP equal to your Kids.</summary>
public class LegoMinefield() : TimCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Kids];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int loss = Kids.Count(Owner);
        if (loss <= 0 || CombatState == null) return;
        foreach (var enemy in CombatState.HittableEnemies.Where(e => e.IsAlive).ToList())
            await CreatureCmd.Damage(ctx, enemy, loss, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
