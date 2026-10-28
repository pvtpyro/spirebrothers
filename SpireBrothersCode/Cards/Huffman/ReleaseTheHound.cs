using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Huffman;

/// <summary>Ancient (Archaic Tooth, replaces Sic 'Em). Mark an enemy. The Mastiff gains 3 Bite, then bites it. Upgrade: costs 0.</summary>
public class ReleaseTheHound() : HuffmanCard(1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Mastiff];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bite", 3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MarkedPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target != null) await Mastiff.Mark(ctx, Owner, play.Target, this);
        await Mastiff.GainBite(ctx, Owner, DynamicVars["Bite"].IntValue, this);
        if (play.Target is { IsAlive: true }) await Mastiff.Attack(ctx, Owner, play.Target);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
