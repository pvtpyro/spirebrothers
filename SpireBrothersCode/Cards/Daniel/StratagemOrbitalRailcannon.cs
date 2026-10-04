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

/// <summary>Helldivers 2. Costs 0. Requires Hands, Logic, Hands this turn. Deal 30 damage.</summary>
public class StratagemOrbitalRailcannon() : DanielCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IReadOnlyList<CardKeyword> StratagemCombo => [BrotherKeywords.Hands, BrotherKeywords.Logic, BrotherKeywords.Hands];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Stratagem];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(10);
}
