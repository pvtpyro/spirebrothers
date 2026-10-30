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

/// <summary>Grind. Deal 1 damage for every time your cards have Ground this run (up to +30).</summary>
public class Completionist() : JohnCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    /// <summary>Most Grinds that count. The run-long count has no end, so uncapped it hit for ~100 by Act 3.</summary>
    public const int MaxGrinds = 30;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Grind];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new ExtraDamageVar(1),
        new DynamicVar("Max", MaxGrinds),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(Grinds)
    ];

    private static decimal Grinds(CardModel card, Creature? _) => Math.Min(MaxGrinds, Grind.TimesThisRun(card.Owner));

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(8);
}
