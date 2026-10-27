using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Ancient (Dusty Tome). Gain a Kid every turn; Kids deal +1 damage per Age.</summary>
public class AllHandsOnDeck() : TimCard(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Kids];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<AllHandsOnDeckPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
