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
using SpireBrothers.SpireBrothersCode.Orbs;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Karaoke. Chorus: draw 1 card per 2 Verses. (Was ALL players, next turn, until 2026-10-26.)</summary>
public class KaraokeNight() : JoshuaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Chorus];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("VersesPerCard", 2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int verses = await Chorus(ctx);
        int cards = verses / DynamicVars["VersesPerCard"].IntValue;
        if (cards > 0) await CardPileCmd.Draw(ctx, cards, Owner);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
