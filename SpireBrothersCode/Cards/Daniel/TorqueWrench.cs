using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Deal 12 damage. Wired: deal 6 more.</summary>
public class TorqueWrench() : DanielCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool HasWiredBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move), new DynamicVar("WiredDamage", 6)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        decimal dmg = DynamicVars.Damage.BaseValue + (wired ? DynamicVars["WiredDamage"].BaseValue : 0);
        await CommonActions.CardAttack(this, play, play.Target, dmg, ValueProp.Move, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["WiredDamage"].UpgradeValueBy(2);
    }
}
