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

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>
/// The Stimm Lab's talents, shown as cards on its pick-1-of-3 screen (they never go in a deck). Token rarity keeps them
/// out of rewards, shops, and random generation. Each one's rule lives where it acts (see Mechanics/StimmLab).
/// </summary>
public abstract class TalentCard() : JohnCard(0, CardType.Power, CardRarity.Token, TargetType.Self)
{
    public abstract StimmLab.Talent Talent { get; }
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    // Shows how many ranks he has, e.g. "Rank 1/3".
    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("Rank", StimmLab.Rank(Owner, Talent));
        description.Add("MaxRank", StimmLab.MaxRank(Talent));
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => Task.CompletedTask;
}
