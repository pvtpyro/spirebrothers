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

/// <summary>Age of Empires. Deal 14 damage. Castle Age or later: hits ALL enemies.</summary>
public class Trebuchet() : TimCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override int? AgeBonusAt => Ages.Castle;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Age];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (AgeBonusActive && CombatState != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
                .TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_heavy_blunt", null, null)
                .Execute(ctx);
        }
        else
        {
            await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
