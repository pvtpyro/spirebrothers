using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>
/// Token: Tim starts the Alexander fight with one in hand (CoppersmithElite). "Hold to the pattern of sound words"
/// (2 Timothy 1:13). Deal 8 damage; a Copper Automaton is taken apart outright. Retain, Exhaust.
/// </summary>
public class SoundDoctrine() : TimCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { IsAlive: true } target) return;
        if (target.Monster is CopperAutomaton)
        {
            await CreatureCmd.Kill(target);
            return;
        }
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
