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

/// <summary>Duo with John (WoW). You and John each gain 6 Block. Your next Attack this turn deals 4 more.</summary>
public class LookingForGroup() : BrandiCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public override bool GainsBlock => true;
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.John);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move), new PowerVar<LookingForGroupPower>(4)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        foreach (var john in DuoCards.Partners(CombatState, Partner).Select(p => p.Creature).Where(c => c.IsAlive && c != Owner.Creature))
            await CreatureCmd.GainBlock(john, DynamicVars.Block, play);
        await CommonActions.ApplySelf<LookingForGroupPower>(ctx, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars["LookingForGroupPower"].UpgradeValueBy(2);
    }
}
