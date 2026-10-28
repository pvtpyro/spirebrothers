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

namespace SpireBrothers.SpireBrothersCode.Cards.John;

/// <summary>Duo with Huffman (uncle and nephew, same age). You and Huffman each gain 2 Strength this turn.</summary>
public class GrewUpTogether() : JohnCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.Huffman);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Strength", 2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var pair = new List<Creature> { Owner.Creature };
        pair.AddRange(DuoCards.Partners(CombatState, Partner).Select(p => p.Creature).Where(c => c.IsAlive && c != Owner.Creature));
        await PowerCmd.Apply<FlexPotionPower>(ctx, pair, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Strength"].UpgradeValueBy(1);
}
