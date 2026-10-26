using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Duo with Tim: every room gets ethernet. Every. Room. Tim gains 2 Kids. Wired: gain 8 Block. Exhaust.</summary>
public class WireTheKidsRooms() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.Tim);
    public override bool GainsBlock => true;
    protected override bool HasWiredBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [BrotherKeywords.Logic, BrotherKeywords.Duo, BrotherKeywords.Wired, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Kids", 2), new BlockVar(8, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Kids)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        foreach (var tim in DuoCards.Partners(CombatState, Partner))
            await Kids.Gain(ctx, tim, DynamicVars["Kids"].IntValue, this);
        if (wired) await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade() => DynamicVars["Kids"].UpgradeValueBy(1);
}
