using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Helldivers 2. Requires Hands, Hands, Logic this turn. Gain 15 Block and 1 energy.</summary>
public class StratagemResupply() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IReadOnlyList<CardKeyword> StratagemCombo => [BrotherKeywords.Hands, BrotherKeywords.Hands, BrotherKeywords.Logic];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Stratagem];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15, ValueProp.Move), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5);
}
