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

namespace SpireBrothers.SpireBrothersCode.Cards.Huffman;

/// <summary>Duo with John (uncle and nephew who grew up together). You and John each gain 8 Block. The Mastiff gains 2 Bite.</summary>
public class SameAgeTechnically() : HuffmanCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.John);
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move), new DynamicVar("Bite", 2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var pair = new List<Creature> { Owner.Creature };
        pair.AddRange(DuoCards.Partners(CombatState, Partner).Select(p => p.Creature).Where(c => c.IsAlive && c != Owner.Creature));
        foreach (var c in pair) await CreatureCmd.GainBlock(c, DynamicVars.Block, play);
        await Mastiff.GainBite(ctx, Owner, DynamicVars["Bite"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}
