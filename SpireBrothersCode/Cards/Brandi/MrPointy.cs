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

/// <summary>Buffy (her favorite show): Kendra's stake. Deal 8 damage. If the enemy is then below 20% HP and isn't a Boss, it's slain. Exhaust.</summary>
public class MrPointy() : BrandiCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int SlayPercent = 20;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];

    // A boss can't be staked; its minions can.
    private bool IsBoss(Creature target) =>
        CombatState?.Encounter?.RoomType == MegaCrit.Sts2.Core.Rooms.RoomType.Boss && target.GetPower<MinionPower>() == null;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target is not { } target) return;
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_dramatic_stab").Execute(ctx);
        if (target.IsAlive && !IsBoss(target) && target.CurrentHp * 100 < target.MaxHp * SlayPercent)
            await CreatureCmd.Kill(target);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
