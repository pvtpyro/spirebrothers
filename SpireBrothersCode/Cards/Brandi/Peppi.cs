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
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;

namespace SpireBrothers.SpireBrothersCode.Cards.Brandi;

/// <summary>Her cat (shredding toilet paper; she says "beep boop"). Deal 2 damage to a random enemy 4 times.</summary>
public class Peppi() : BrandiCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    public const int Hits = 4;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        Banter.Bubble(Owner.Creature, new MegaCrit.Sts2.Core.Localization.LocString("powers", "SPIREBROTHERS-PEPPI_BUBBLE").GetFormattedText());
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this)
            .TargetingRandomOpponents(CombatState).WithHitFx("vfx/vfx_scratch").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}
