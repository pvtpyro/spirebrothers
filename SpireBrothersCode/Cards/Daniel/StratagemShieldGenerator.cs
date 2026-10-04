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

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Helldivers 2. Costs 0. Requires Logic, Logic this turn. Gain 4 Plating. Exhaust.</summary>
public class StratagemShieldGenerator() : DanielCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IReadOnlyList<CardKeyword> StratagemCombo => [BrotherKeywords.Logic, BrotherKeywords.Logic];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Stratagem, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PlatingPower>(4)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PlatingPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }

    protected override void OnUpgrade() => DynamicVars["PlatingPower"].UpgradeValueBy(2);
}
