using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>
/// Duo with David: the perfect gift for David, one he'll never spend. Lose 20 Gold. David gains 40 Gold. Exhaust.
/// Needs the 20 Gold to play.
/// </summary>
public class GiftCard() : TimCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.David);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Gold", 20), new DynamicVar("Gift", 40)];

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
        await TimGold.Spend(ctx, Owner, DynamicVars["Gold"].IntValue);
        foreach (var david in DuoCards.Partners(CombatState, Partner))
            await PlayerCmd.GainGold(DynamicVars["Gift"].IntValue, david);
    }

    protected override void OnUpgrade() => DynamicVars["Gift"].UpgradeValueBy(20);
}
