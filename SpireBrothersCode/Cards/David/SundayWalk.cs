using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// His walk with Dad every Sunday. Power: at the start of your turn, heal 2 HP. Every 7th turn (Sunday), also draw 2
/// cards.
/// </summary>
public class SundayWalk() : DavidCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(SundayWalkPower.BaseHeal), new CardsVar(SundayWalkPower.SundayCards)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SundayWalkPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<SundayWalkPower>(ctx, Owner.Creature, DynamicVars.Heal.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Heal.UpgradeValueBy(1);
}
