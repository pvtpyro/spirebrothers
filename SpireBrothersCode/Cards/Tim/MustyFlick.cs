using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Rocket League. Deal 9 damage. If it's the last card in your hand, deal 9 more.</summary>
public class MustyFlick() : TimCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(9),
        new ExtraDamageVar(9),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(LastCard)
    ];

    // In hand: is it the only card? While being played it has already left the hand, so the hand must be empty.
    private static decimal LastCard(CardModel card, Creature? _)
    {
        if (card.Owner == null || card.CombatState == null) return 0;
        var hand = PileType.Hand.GetPile(card.Owner).Cards;
        bool inHand = hand.Contains(card);
        return (inHand ? hand.Count == 1 : hand.Count == 0) ? 1 : 0;
    }

    protected override bool ExtraGlow => LastCard(this, null) > 0;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3);
        DynamicVars.ExtraDamage.UpgradeValueBy(3);
    }
}
