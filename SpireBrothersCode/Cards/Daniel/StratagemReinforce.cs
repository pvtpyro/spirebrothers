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

/// <summary>Helldivers 2. Costs 0. Requires Logic, Hands this turn. Share: a player heals 8 HP. Exhaust.</summary>
public class StratagemReinforce() : DanielCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    protected override IReadOnlyList<CardKeyword> StratagemCombo => [BrotherKeywords.Logic, BrotherKeywords.Hands];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Stratagem, BrotherKeywords.Share, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(8)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.Heal(ShareTarget(play), DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Heal.UpgradeValueBy(4);
}
