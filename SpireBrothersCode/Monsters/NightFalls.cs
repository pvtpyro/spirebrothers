using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Night Falls (Minecraft), an Act 1 normal fight: a Creeper, a Spider, and a Skeleton, about 55 HP in all. The Creeper
/// is the puzzle: its Fuse counts down and it explodes (and dies) on its third turn unless it's killed first.
/// </summary>
public class NightFallsNormal() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<Creeper>(), ModelDb.Monster<Spider>(), ModelDb.Monster<Skeleton>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<Creeper>().ToMutable(), null),
        (ModelDb.Monster<Spider>().ToMutable(), null),
        (ModelDb.Monster<Skeleton>().ToMutable(), null),
    ];
}

/// <summary>Creeps, hisses, explodes. The explosion is a death blow (like vanilla Gas Bomb): it hits, then the Creeper dies.</summary>
public class Creeper : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 17, 15);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 18, 16);
    private int ExplodeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);
    public const int FuseTurns = 3;

    protected override string ArtFolder => "creeper";
    protected override float BodyHeight => 250f;

    private const string ExplodeSfx = "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_explode";
    public override string? CustomAttackSfx => ExplodeSfx;
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_minion_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<FusePower>(new ThrowingPlayerChoiceContext(), Creature, FuseTurns, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var creep = new MoveState("CREEP", Creep, new UnknownIntent());
        var hiss = new MoveState("HISS", Hiss, new UnknownIntent());
        var explode = new MoveState("EXPLODE", Explode, new DeathBlowIntent(() => ExplodeDamage));
        creep.FollowUpState = hiss;
        hiss.FollowUpState = explode;
        explode.FollowUpState = explode;
        return new MonsterMoveStateMachine([creep, hiss, explode], creep);
    }

    private async Task BurnFuse()
    {
        if (Creature.GetPower<FusePower>() is { } fuse) await PowerCmd.Decrement(fuse);
    }

    private async Task Creep(IReadOnlyList<Creature> targets) => await BurnFuse();

    private async Task Hiss(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.6f);
        await BurnFuse();
    }

    private async Task Explode(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ExplodeDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, ExplodeSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
        await CreatureCmd.Kill(Creature);
    }
}

/// <summary>Shoots, shoots, takes aim (Strength), and repeats.</summary>
public class Skeleton : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 20, 18);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 22, 20);
    private int ArrowDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private const int Arrows = 2;
    private int AimStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    protected override string ArtFolder => "skeleton";
    protected override float BodyHeight => 330f;

    private const string BowSfx = "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider";
    public override string? CustomAttackSfx => BowSfx + "_attack";
    public override string? CustomDeathSfx => BowSfx + "_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var shoot = new MoveState("SHOOT", Shoot, new MultiAttackIntent(ArrowDamage, Arrows));
        var shootAgain = new MoveState("SHOOT_AGAIN", Shoot, new MultiAttackIntent(ArrowDamage, Arrows));
        var aim = new MoveState("TAKE_AIM", TakeAim, new BuffIntent());
        shoot.FollowUpState = shootAgain;
        shootAgain.FollowUpState = aim;
        aim.FollowUpState = shoot;
        return new MonsterMoveStateMachine([shoot, shootAgain, aim], shoot);
    }

    private async Task Shoot(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ArrowDamage).WithHitCount(Arrows).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    private async Task TakeAim(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider_reload");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, AimStrength, Creature, null);
    }
}

/// <summary>Bites (with Weak), then climbs the wall (Block), and repeats.</summary>
public class Spider : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 23, 21);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 24, 22);
    private int BiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private const int BiteWeak = 1;
    private int ClimbBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);

    protected override string ArtFolder => "spider";
    protected override float BodyHeight => 150f;

    private const string RatSfx = "event:/sfx/enemy/enemy_attacks/two_tail_rats/two_tail_rats";
    public override string? CustomAttackSfx => RatSfx + "_attack_bite";
    public override string? CustomDeathSfx => RatSfx + "_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var bite = new MoveState("BITE", Bite, new SingleAttackIntent(BiteDamage), new DebuffIntent());
        var climb = new MoveState("CLIMB", Climb, new DefendIntent());
        bite.FollowUpState = climb;
        climb.FollowUpState = bite;
        return new MonsterMoveStateMachine([bite, climb], bite);
    }

    private async Task Bite(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BiteDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, BiteWeak, Creature, null);
    }

    private async Task Climb(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.GainBlock(Creature, ClimbBlock, ValueProp.Move, null);
    }
}
