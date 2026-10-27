using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>
/// Token: Daniel starts the Lions' Den fight with one in hand (LionsDenElite). "My God sent his angel and shut the
/// lions' mouths" (Daniel 6:22): an enemy skips its next move. Retain, Exhaust.
/// </summary>
public class ShutTheirMouths() : DanielCard(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { IsAlive: true } target) return;
        await CreatureCmd.Stun(target, target.Monster?.NextMove.Id);
    }
}
