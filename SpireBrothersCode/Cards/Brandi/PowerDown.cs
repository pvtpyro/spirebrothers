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

/// <summary>Shutting off Daniel's PC with one touch. Spend all your Static. If you spent 5 or more, Stun an enemy that isn't a Boss. Exhaust.</summary>
public class PowerDown() : BrandiCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Static", 5)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StaticPower>()];
    protected override bool ExtraGlow => StaticPower.On(Owner) >= DynamicVars["Static"].IntValue;

    // A boss can't be shut off; its minions can.
    private bool IsBoss(Creature target) =>
        CombatState?.Encounter?.RoomType == MegaCrit.Sts2.Core.Rooms.RoomType.Boss && target.GetPower<MinionPower>() == null;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int spent = await StaticPower.SpendAll(Owner);
        if (spent < DynamicVars["Static"].IntValue || play.Target is not { IsAlive: true } target || IsBoss(target)) return;
        VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_lightning");
        await CreatureCmd.Stun(target, target.Monster?.NextMove.Id);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
