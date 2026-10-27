using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>
/// Token: Joshua starts the Walls of Jericho fight with one in hand (JerichoElite). Song. Deal 4 damage to ALL enemies.
/// Counts the walls down twice (once as a Song, once more in SevenTrumpetsPower). Retain, Exhaust.
/// </summary>
public class TrumpetBlast() : JoshuaCard(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Song, CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}
