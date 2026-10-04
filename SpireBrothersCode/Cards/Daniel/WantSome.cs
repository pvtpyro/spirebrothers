using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Share: a player heals 3 (6 if it's you, since only Daniel likes grapefruit) and gains 1 energy next turn. Exhaust.</summary>
public class WantSome() : DanielCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Share, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(3), new DynamicVar("SelfHeal", 6), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var target = ShareTarget(play);
        var heal = target == Owner.Creature ? DynamicVars["SelfHeal"].BaseValue : DynamicVars.Heal.BaseValue;
        await CreatureCmd.Heal(target, heal);
        await PowerCmd.Apply<EnergyNextTurnPower>(ctx, target, DynamicVars.Energy.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2);
        DynamicVars["SelfHeal"].UpgradeValueBy(3);
    }
}
