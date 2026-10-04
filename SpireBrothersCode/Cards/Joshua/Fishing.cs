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

/// <summary>Stardew Valley. Draw 1 card. If it's a Song, gain 1 Verse and draw another.</summary>
public class Fishing() : JoshuaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new DynamicVar("VerseGain", 1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var drawn = (await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner)).ToList();
        if (!drawn.Any(c => c.Keywords.Contains(BrotherKeywords.Song))) return;
        await Verses.Gain(ctx, Owner, DynamicVars["VerseGain"].BaseValue, this);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
