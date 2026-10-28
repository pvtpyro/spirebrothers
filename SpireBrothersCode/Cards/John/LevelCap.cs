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

/// <summary>WoW (hitting max level). Gain 1 Strength for every 10 times your cards have Ground this run. Upgrade: every 7.</summary>
public class LevelCap() : JohnCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Grind];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Per", 10)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    private int Strength => !IsMutable || Owner == null ? 0 : Grind.TimesThisRun(Owner) / Math.Max(1, DynamicVars["Per"].IntValue);

    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("Now", Strength);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (Strength > 0) await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, Strength, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Per"].UpgradeValueBy(-3);
}
