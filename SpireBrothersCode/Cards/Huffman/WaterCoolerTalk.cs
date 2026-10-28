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

/// <summary>Duo with Daniel (coworkers: the IT guy and the software engineer). You and Daniel each draw 1 card and gain 1 Energy next turn.</summary>
public class WaterCoolerTalk() : HuffmanCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.Daniel);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var pair = new List<Creature> { Owner.Creature };
        pair.AddRange(DuoCards.Partners(CombatState, Partner).Select(p => p.Creature).Where(c => c.IsAlive && c != Owner.Creature));
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, pair, DynamicVars.Cards.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<EnergyNextTurnPower>(ctx, pair, DynamicVars.Energy.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
