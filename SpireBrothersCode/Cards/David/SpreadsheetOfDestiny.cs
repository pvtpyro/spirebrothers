using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>Ancient (Dusty Tome). Exact works with 1 energy left and triggers an extra time; no more energy gain.</summary>
public class SpreadsheetOfDestiny() : DavidCard(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Exact];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<SpreadsheetOfDestinyPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
