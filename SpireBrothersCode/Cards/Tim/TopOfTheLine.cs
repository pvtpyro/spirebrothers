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
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>He has to have the best of everything. Lose 25 Gold: Upgrade ALL your cards for the rest of combat. Exhaust.</summary>
public class TopOfTheLine() : TimCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Gold", 25)];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            try
            {
                return !IsMutable || Owner == null || Owner.Gold >= DynamicVars["Gold"].IntValue;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Playable check failed on {GetType().Name}: {e}");
                return false;
            }
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await TimGold.Spend(ctx, Owner, DynamicVars["Gold"].IntValue);
        var upgradable = new[] { PileType.Hand, PileType.Draw, PileType.Discard }
            .SelectMany(p => p.GetPile(Owner).Cards)
            .Where(c => c != this && c.IsUpgradable)
            .ToList();
        if (upgradable.Count > 0) CardCmd.Upgrade(upgradable, CardPreviewStyle.HorizontalLayout);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
