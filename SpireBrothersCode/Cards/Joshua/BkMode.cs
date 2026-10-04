using BaseLib.Cards.Variables;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>
/// Archipelago: stuck waiting on everyone else's items, until they all show up at once.
/// Can only be played if you have no other playable cards. Check 3 times.
/// </summary>
public class BkMode() : JoshuaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Check];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Checks", 3)];

    // Skips other BK Modes so two in hand don't ask each other forever.
    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            try
            {
                if (!IsInCombat || Owner?.PlayerCombatState == null) return true;
                return !PileType.Hand.GetPile(Owner).Cards.Any(c => c != this && c is not BkMode && c.CanPlay());
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
        await Archipelago.Check(ctx, Owner, this, DynamicVars["Checks"].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars["Checks"].UpgradeValueBy(1);
}
