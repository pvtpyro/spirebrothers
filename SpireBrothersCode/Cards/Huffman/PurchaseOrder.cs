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

/// <summary>The IT guy buying for the boss. Gain 4 Metal. Exhaust. Upgrade: costs 0.</summary>
public class PurchaseOrder() : HuffmanCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, BrotherKeywords.Metal];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Metal", 4)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await Metal.Gain(ctx, Owner, DynamicVars["Metal"].IntValue, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
