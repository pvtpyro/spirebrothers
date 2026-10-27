using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Ancient (Archaic Tooth, replaces Percussive Maintenance). Deal 12 damage. Apply 2 Vulnerable. Wired: hit twice.</summary>
public class HaveYouTriedHittingItHarder() : DanielCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override bool HasWiredBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move), new PowerVar<VulnerablePower>(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        await CommonActions.CardAttack(this, play, wired ? 2 : 1, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
        if (play.Target is { IsAlive: true })
            await CommonActions.Apply<VulnerablePower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
