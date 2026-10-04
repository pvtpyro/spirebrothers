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

/// <summary>Family / co-op. ALL players gain 3 Block per Kid. Exhaust.</summary>
public class FamilyReunion() : TimCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Kids, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockPerKid", 3)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var block = DynamicVars["BlockPerKid"].BaseValue * Kids.Count(Owner);
        if (block <= 0) return;
        foreach (var player in AllPlayers) await CreatureCmd.GainBlock(player, block, ValueProp.Move, play);
    }

    protected override void OnUpgrade() => DynamicVars["BlockPerKid"].UpgradeValueBy(1);
}
