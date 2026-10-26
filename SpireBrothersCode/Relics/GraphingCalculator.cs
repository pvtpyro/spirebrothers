using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>The first time Exact triggers each turn, gain 4 Block. Exact.Check runs relic listeners too.</summary>
public class GraphingCalculator : DavidRelic, IExactListener
{
    public const int BlockAmount = 4;
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Exact)];

    // Combat-only, so it isn't saved.
    private bool _usedThisTurn;

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner) _usedThisTurn = false;
        return Task.CompletedTask;
    }

    public async Task OnExact(PlayerChoiceContext choiceContext, Player player)
    {
        if (_usedThisTurn || player != Owner) return;
        _usedThisTurn = true;
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, BlockAmount, ValueProp.Unpowered, null);
    }
}
