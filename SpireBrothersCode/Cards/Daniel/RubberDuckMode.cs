using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Ancient (Dusty Tome). Each turn, enter the first input of a Stratagem in hand for free.</summary>
public class RubberDuckMode() : DanielCard(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<RubberDuckModePower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
