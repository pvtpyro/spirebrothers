using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Duo with Daniel: Joshua sings, Daniel goes in. Song. Share: a player gains 3 Strength this turn.</summary>
public class HypeMan() : JoshuaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer), IDuoCard
{
    public Type Partner => typeof(Brothers.Daniel);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Song, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Strength", 3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        // Vanilla Flex Potion's power: Strength that wears off at the end of the turn.
        await PowerCmd.Apply<FlexPotionPower>(ctx, ShareTarget(play), DynamicVars["Strength"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Strength"].UpgradeValueBy(2);
}
