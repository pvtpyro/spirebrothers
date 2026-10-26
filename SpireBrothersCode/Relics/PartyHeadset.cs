using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Shared (Daniel, David, Joshua; Tim has no Share cards). The first Share card you play each turn: you and its
/// target gain 3 Block (just you, once, if you targeted yourself).
/// </summary>
public class PartyHeadset : DanielRelic
{
    public const int BlockAmount = 3;
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Share)];

    // Combat-only, so it isn't saved.
    private bool _usedThisTurn;

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner) _usedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (_usedThisTurn || card.Owner != Owner || !card.Keywords.Contains(BrotherKeywords.Share)) return;
        _usedThisTurn = true;
        Flash();
        var me = Owner.Creature;
        var target = cardPlay.Target is { IsPlayer: true, IsAlive: true } t ? t : me;
        await CreatureCmd.GainBlock(me, BlockAmount, ValueProp.Unpowered, null);
        if (target != me) await CreatureCmd.GainBlock(target, BlockAmount, ValueProp.Unpowered, null);
    }
}
