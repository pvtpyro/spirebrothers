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

/// <summary>Duo with Daniel: the sound card she fried with one tap. Deal 10 damage to ALL enemies. Daniel's Wired cards trigger this turn.</summary>
public class ShoulderTap() : BrandiCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies), IDuoCard
{
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.Daniel);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_lightning").Execute(ctx);
        foreach (var daniel in DuoCards.Partners(CombatState, Partner).Select(p => p.Creature).Where(c => c.IsAlive))
        {
            Banter.Bubble(daniel, "!?");
            if (daniel.GetPower<SpareBatteryPower>() == null)
                await PowerCmd.Apply<SpareBatteryPower>(ctx, daniel, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
