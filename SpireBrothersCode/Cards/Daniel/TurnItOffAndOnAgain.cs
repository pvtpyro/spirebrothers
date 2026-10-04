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

/// <summary>Share: a player loses 1 stack of a random debuff and draws 2 cards next turn.</summary>
public class TurnItOffAndOnAgain() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Logic, BrotherKeywords.Share];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var target = ShareTarget(play);
        var debuffs = target.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
        if (debuffs.Count > 0)
        {
            // Run RNG so co-op stays in sync.
            var debuff = Owner.RunState.Rng.CombatTargets.NextItem(debuffs)!;
            if (debuff.StackType == PowerStackType.Single || Math.Abs(debuff.Amount) <= 1)
                await PowerCmd.Remove(debuff);
            else // Some debuffs (like lowered Strength) count down from below zero.
                await PowerCmd.ModifyAmount(ctx, debuff, debuff.Amount < 0 ? 1 : -1, Owner.Creature, this);
        }
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, target, DynamicVars.Cards.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
