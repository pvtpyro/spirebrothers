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

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// National Treasure: an inside joke between David and Daniel (the scrambled guess at "Valley Forge").
/// Draw 2 cards. Exact: you cracked it! Exhaust this and add Valley Forge to your hand.
/// </summary>
public class AGloveFry() : DavidCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Exact];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<ValleyForge>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int exact = await Exact.Check(ctx, this);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        if (exact == 0 || CombatState == null) return;

        ExhaustOnNextPlay = true;
        for (int i = 0; i < exact; i++)
        {
            var forge = CombatState.CreateCard<ValleyForge>(Owner);
            if (IsUpgraded) CardCmd.Upgrade(forge);
            await CardPileCmd.AddGeneratedCardToCombat(forge, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
