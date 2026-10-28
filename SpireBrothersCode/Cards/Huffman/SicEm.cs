using BaseLib.Abstracts;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Huffman;

/// <summary>Starter. Mark an enemy. The Mastiff gains 2 Bite. Upgrade: costs 0.</summary>
public class SicEm() : HuffmanCard(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy), ITranscendenceCard
{
    // Archaic Tooth swaps it for this.
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<ReleaseTheHound>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Mastiff];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bite", 2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MarkedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target != null) await Mastiff.Mark(ctx, Owner, play.Target, this);
        await Mastiff.GainBite(ctx, Owner, DynamicVars["Bite"].IntValue, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
