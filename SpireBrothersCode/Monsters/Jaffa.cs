using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// One of Apophis's serpent guard Jaffa (ApophisBoss). A minion: fires his staff weapon, then shouts "Kree!" and gives
/// Apophis Block (himself, if Apophis is down), and around again. One of the starting pair opens with Kree!. Shol'va
/// turns one against Apophis.
/// </summary>
public class Jaffa : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 40, 36);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 42, 38);
    private int StaffDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);
    private int KreeBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);   // (was 10: two Kree!s in step with his own came to 35 Block)

    /// <summary>Set by the encounter so the pair doesn't do the same thing each turn.</summary>
    public bool KreeFirst { get; set; }

    protected override string ArtFolder => "jaffa";
    protected override float BodyHeight => 300f;
    protected override bool TipOver => true;

    private const string StaffSfx = "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider";
    public override string? CustomAttackSfx => StaffSfx + "_attack";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var staff = new MoveState("STAFF_WEAPON", StaffWeapon, new SingleAttackIntent(StaffDamage));
        var kree = new MoveState("KREE", Kree, new DefendIntent());
        staff.FollowUpState = kree;
        kree.FollowUpState = staff;
        return new MonsterMoveStateMachine([staff, kree], KreeFirst ? kree : staff);
    }

    private async Task StaffWeapon(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(StaffDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    private async Task Kree(IReadOnlyList<Creature> targets)
    {
        var apophis = Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Apophis);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.GainBlock(apophis ?? Creature, KreeBlock, ValueProp.Move, null);
    }
}
