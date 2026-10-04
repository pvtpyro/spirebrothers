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

/// <summary>Share: a player gains 8 Block. Wired: they also lose a debuff.</summary>
public class TechSupport() : DanielCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
{
    protected override bool HasWiredBonus => true;
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands, BrotherKeywords.Share, BrotherKeywords.Wired];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        bool wired = await Wired.Check(this);
        var target = ShareTarget(play);
        await CreatureCmd.GainBlock(target, DynamicVars.Block, play);
        if (wired)
        {
            var debuff = target.Powers.FirstOrDefault(p => p.Type == PowerType.Debuff);
            if (debuff != null) await PowerCmd.Remove(debuff);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}
