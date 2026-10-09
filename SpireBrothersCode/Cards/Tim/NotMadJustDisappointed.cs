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

/// <summary>Monkey Island insult (dad of eight). Apply 2 Insulted and 1 Weak.</summary>
public class NotMadJustDisappointed() : TimCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy), IInsultCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<InsultedPower>(2), new PowerVar<WeakPower>(1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InsultedPower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target == null) return;
        await CommonActions.Apply<InsultedPower>(ctx, play.Target, this);
        await CommonActions.Apply<WeakPower>(ctx, play.Target, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["InsultedPower"].UpgradeValueBy(1);
        DynamicVars.Weak.UpgradeValueBy(1);
    }
}
