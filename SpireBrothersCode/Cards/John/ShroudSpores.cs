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

/// <summary>Enshrouded + chems. Apply 3 Toxin to ALL enemies (5 in the Shroud).</summary>
public class ShroudSpores() : JohnCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override bool ShroudBonus => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Shroud];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Toxin", 3), new DynamicVar("ShroudToxin", 5)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ToxinPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int toxin = InShroud ? DynamicVars["ShroudToxin"].IntValue : DynamicVars["Toxin"].IntValue;
        await Toxin.Apply(ctx, CombatState!.HittableEnemies, toxin, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Toxin"].UpgradeValueBy(1);
        DynamicVars["ShroudToxin"].UpgradeValueBy(1);
    }
}
