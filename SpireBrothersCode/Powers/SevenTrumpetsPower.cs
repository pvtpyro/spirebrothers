using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.Joshua;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On the Wall of Jericho: the countdown to the walls falling (Joshua 6). Amount goes down by 1 at the start of every
/// player turn after the first (marching around the city), by 1 for every Song or Chorus anyone plays, and by 1 more
/// for Joshua's Trumpet Blast. At 0 the Wall collapses (WallOfJericho.Collapse), this goes away, and Joshua gets his line.
/// Also the fight's FoePower: Joshua starts with Trumpet Blast and gets the opening line.
/// </summary>
public class SevenTrumpetsPower : FoePower
{
    public const int Start = 7;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<TrumpetBlast>();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        var combat = Owner.CombatState;
        // Once per round, on the first player's turn start.
        if (combat != null && combat.RoundNumber > 1 && player == combat.Players.FirstOrDefault()) await Tick(choiceContext, 1);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var keywords = cardPlay.Card.Keywords;
        int ticks = keywords.Contains(BrotherKeywords.Song) || keywords.Contains(BrotherKeywords.Chorus) ? 1 : 0;
        if (cardPlay.Card is TrumpetBlast) ticks++;
        if (ticks > 0) await Tick(choiceContext, ticks);
    }

    private async Task Tick(PlayerChoiceContext ctx, int n)
    {
        if (Owner.IsDead || Owner.Monster is not WallOfJericho wall || wall.IsFallen) return;
        Flash();
        if (Amount > n)
        {
            await PowerCmd.ModifyAmount(ctx, this, -n, null, null);
            return;
        }
        var combat = Owner.CombatState;
        await PowerCmd.Remove(this);
        await wall.Collapse();
        if (combat != null) Elite?.HeroWins(combat);
    }
}
