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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;
/// <summary>
/// Age of Empires. Tim's starting card and his only way to advance an Age: pay gold (50, 100, then 150; upgraded 40, 80, 120) to go up one
/// Age for the rest of the run. Once per act: after he Ages Up, it stays out of his combats until the next act.
/// Once he reaches Imperial it leaves his deck.
/// </summary>
public class AgeUp() : TimCard(3, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Age, CardKeyword.Exhaust];

    // The gold price for the next Age. Kept as a var (refreshed whenever the text is drawn) so the upgrade preview
    // shows the lower price in green like any other upgraded number.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("AgeCost", Ages.GoldCost(Ages.Dark))];

    // The compendium shows the canonical (ownerless) card, and reading Owner on it throws, so check IsMutable first.
    private int Age => !IsMutable || Owner == null ? Ages.Dark : Ages.Get(Owner);
    private int Cost => !IsMutable || Owner == null ? Ages.GoldCost(Age, IsUpgraded) : Ages.GoldCost(Owner, Age, IsUpgraded);

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            try
            {
                if (!IsMutable || Owner == null) return true;
                return Age < Ages.Imperial && Owner.Gold >= Cost && !Ages.AgedUpThisAct(Owner);
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Playable check failed on {GetType().Name}: {e}");
                return false;
            }
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        if (IsMutable) DynamicVars["AgeCost"].BaseValue = Cost;
        description.Add("NextAge", Ages.Name(Math.Min(Age + 1, Ages.Imperial)));
        description.Add("Maxed", Age >= Ages.Imperial);
    }

    // Once per act: after he Ages Up, the card sits out every combat until the next act starts.
    public override async Task BeforeCombatStart()
    {
        if (Owner != null && Ages.AgedUpThisAct(Owner)) await CardPileCmd.RemoveFromCombat(this, skipVisuals: true);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int cost = Cost;
        if (Age >= Ages.Imperial || Owner.Gold < cost || Ages.AgedUpThisAct(Owner)) return;
        await TimGold.Spend(ctx, Owner, cost);
        Ages.AgeUp(Owner);
        if (Ages.Get(Owner) >= Ages.Imperial && DeckVersion != null) await CardPileCmd.RemoveFromDeck(DeckVersion);
    }

    // Upgrading lowers the gold (40 / 80 / 120), not the energy, so it stays a big turn to play.
    protected override void OnUpgrade() => DynamicVars["AgeCost"].UpgradeValueBy(Ages.GoldCost(Ages.Dark, true) - Ages.GoldCost(Ages.Dark));
}
