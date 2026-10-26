using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Duo with Joshua: Rocket League, which they both love. Deal 8 damage. Joshua gains 2 Verses. Script.</summary>
public class CenteringPass() : TimCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDuoCard
{
    public Type Partner => typeof(Brothers.Joshua);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Script];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new DynamicVar("Verses", 2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
        foreach (var joshua in DuoCards.Partners(CombatState, Partner))
            await Verses.Gain(ctx, joshua, DynamicVars["Verses"].BaseValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}
