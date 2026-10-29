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

/// <summary>Duo with David: her fire, his Bleed. Apply 3 Ember and 3 Bleed.</summary>
public class ShakeAndBake() : BrandiCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy), IDuoCard
{
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.David);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ember", 3), new PowerVar<BleedPower>(3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<EmberPower>(), HoverTipFactory.FromPower<BleedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { IsAlive: true } target) return;
        await Ember.Apply(ctx, target, DynamicVars["Ember"].IntValue, Owner, this);
        await PowerCmd.Apply<BleedPower>(ctx, target, DynamicVars["BleedPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Ember"].UpgradeValueBy(1);
        DynamicVars["BleedPower"].UpgradeValueBy(1);
    }
}
