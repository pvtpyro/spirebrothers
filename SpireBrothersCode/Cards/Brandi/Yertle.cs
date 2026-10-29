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

/// <summary>Her mean 22-year-old turtle, who ate her 3 brothers. Exhaust up to 3 other cards in your hand. Deal 8 damage, +8 for each card Exhausted.</summary>
public class Yertle() : BrandiCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int MaxEaten = 3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new DynamicVar("PerCard", 8)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { } target) return;
        var eaten = (await CardSelectCmd.FromHand(ctx, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, MaxEaten), c => c != this, this)).ToList();
        foreach (var card in eaten) await CardCmd.Exhaust(ctx, card);
        decimal damage = DynamicVars.Damage.BaseValue + DynamicVars["PerCard"].BaseValue * eaten.Count;
        await DamageCmd.Attack(damage).FromCard(this).Targeting(target).WithHitFx("vfx/vfx_bite").Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["PerCard"].UpgradeValueBy(2);
    }
}
