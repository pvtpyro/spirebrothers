using BaseLib.Cards.Variables;
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
using SpireBrothers.SpireBrothersCode.Orbs;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>D&amp;D (bard spell). Roll. On 10+, the enemy is laughing too hard to act: it skips its next move. Otherwise apply 2 Weak. Exhaust.</summary>
public class HideousLaughter() : JoshuaCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int Needed = 10;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Roll, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { IsAlive: true } target) return;
        int roll = await Dice.Roll(ctx, Owner);
        if (roll >= Needed) await CreatureCmd.Stun(target, target.Monster?.NextMove.Id);
        else await CommonActions.Apply<WeakPower>(ctx, target, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
