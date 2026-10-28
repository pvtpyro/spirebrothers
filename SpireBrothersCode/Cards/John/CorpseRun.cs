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

/// <summary>WoW (running back after dying). Gain 6 Block. If you're under half HP, gain 6 more and draw 2 cards.</summary>
public class CorpseRun() : JohnCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move), new DynamicVar("LowBlock", 6), new CardsVar(2)];
    private bool Low => Owner.Creature.CurrentHp * 2 < Owner.Creature.MaxHp;
    protected override bool ExtraGlow => Low;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool low = Low;
        await CommonActions.CardBlock(this, play);
        if (!low) return;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["LowBlock"].BaseValue, DynamicVars.Block.Props, play);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars["LowBlock"].UpgradeValueBy(3);
    }
}
