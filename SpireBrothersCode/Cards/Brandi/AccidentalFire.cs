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

/// <summary>The AIT fire. Apply 5 Ember to ALL enemies. Apply 2 Ember to yourself (on a player it fades instead of growing). Exhaust.</summary>
public class AccidentalFire() : BrandiCard(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ember", 5), new DynamicVar("SelfEmber", 2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<EmberPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Ember.Apply(ctx, Enemies, DynamicVars["Ember"].IntValue, Owner, this);
        // Straight onto her, so the Zippo and Private Pyro don't make her own fire bigger.
        await PowerCmd.Apply<EmberPower>(ctx, Owner.Creature, DynamicVars["SelfEmber"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Ember"].UpgradeValueBy(2);
}
