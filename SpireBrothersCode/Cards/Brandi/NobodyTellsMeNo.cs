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
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;

namespace SpireBrothers.SpireBrothersCode.Cards.Brandi;

/// <summary>She always wanted to be a sniper; in the Spire, she is. Shot. Deal 20 damage (+6 per Aim). Costs 1 less for every 3 Aim you have.</summary>
public class NobodyTellsMeNo() : BrandiCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), ICostReducer
{
    public const int AimPerEnergy = 3;
    public override bool IsShot => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Shot];
    protected override IEnumerable<DynamicVar> CanonicalVars => ShotVars(20, 6);
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AimPower>()];

    public int CostReduction => Aim.Count(Owner) / AimPerEnergy;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await Shoot(ctx, play, "vfx/vfx_heavy_blunt");

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(6);
}
