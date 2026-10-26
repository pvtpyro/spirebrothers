using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Duo with Joshua: Daniel fixes everything. Joshua gains 3 Verses. Wired: draw 2 cards.</summary>
public class FixedYourAmp() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.Joshua);
    protected override bool HasWiredBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Duo, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Verses", 3), new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        foreach (var joshua in DuoCards.Partners(CombatState, Partner))
            await Verses.Gain(ctx, joshua, DynamicVars["Verses"].BaseValue, this);
        if (wired) await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Verses"].UpgradeValueBy(1);
}
