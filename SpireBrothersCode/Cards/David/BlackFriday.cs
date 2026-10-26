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

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// The one day he finally buys something: a doorbuster deal. Lose 40 Gold, gain 3 Strength (4 upgraded). Exhaust.
/// Can't be played without the 40 Gold.
/// </summary>
public class BlackFriday() : DavidCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Gold", 40), new PowerVar<StrengthPower>(3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            try
            {
                return !IsMutable || Owner == null || Owner.Gold >= DynamicVars["Gold"].IntValue;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Playable check failed on {GetType().Name}: {e}");
                return false;
            }
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await LoseGold(DynamicVars["Gold"].IntValue);
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }

    protected override void OnUpgrade() => DynamicVars.Strength.UpgradeValueBy(1);
}
