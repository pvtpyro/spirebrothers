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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Deal 10 damage. If this kills an enemy, gain 20 gold. (Works like vanilla Hand of Greed.)</summary>
public class BugBounty() : DanielCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override bool CanBeGeneratedInCombat => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move), new DynamicVar("Gold", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target == null) return;
        bool canTriggerFatal = play.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attack = await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
        if (canTriggerFatal && attack.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
            await PlayerCmd.GainGold(DynamicVars["Gold"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["Gold"].UpgradeValueBy(5);
    }
}
