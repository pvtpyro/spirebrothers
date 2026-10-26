using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>Duo with Tim: a loan, with interest, obviously. Tim gains 30 Gold. Exact: you gain 30 Gold too. Exhaust.</summary>
public class Iou() : DavidCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.Tim);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Exact, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Gold", 30)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int exact = await Exact.Check(ctx, this);
        int gold = DynamicVars["Gold"].IntValue;
        foreach (var tim in DuoCards.Partners(CombatState, Partner))
            await PlayerCmd.GainGold(gold, tim);
        if (exact > 0) await PlayerCmd.GainGold(exact * gold, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Gold"].UpgradeValueBy(10);
}
