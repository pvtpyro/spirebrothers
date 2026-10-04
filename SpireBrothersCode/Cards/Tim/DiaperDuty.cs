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

/// <summary>Dad of eight. Exhaust a Status or Curse in your hand. Gain 1 Kid.</summary>
public class DiaperDuty() : TimCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Kids];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("KidGain", 1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        static bool Dirty(CardModel c) => c.Type is CardType.Status or CardType.Curse;
        if (PileType.Hand.GetPile(Owner).Cards.Any(Dirty))
        {
            var picked = await CardSelectCmd.FromHand(ctx, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), Dirty, this);
            var card = picked.FirstOrDefault();
            if (card != null) await CardCmd.Exhaust(ctx, card);
        }
        await Kids.Gain(ctx, Owner, DynamicVars["KidGain"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["KidGain"].UpgradeValueBy(1);
}
