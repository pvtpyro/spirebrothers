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

/// <summary>Sniper + fire. Shot. Deal 6 damage (+3 per Aim). Apply Ember equal to the Aim spent.</summary>
public class TracerRound() : BrandiCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override bool IsShot => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Shot];
    protected override IEnumerable<DynamicVar> CanonicalVars => ShotVars(6);
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AimPower>(), HoverTipFactory.FromPower<EmberPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int spent = await Shoot(ctx, play, "vfx/vfx_fire_burst");
        if (play.Target is { IsAlive: true } target && spent > 0) await Ember.Apply(ctx, target, spent, Owner, this);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3);
}
