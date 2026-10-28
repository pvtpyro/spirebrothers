using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Shared body and sound code for the Jericho defenders (pixelgen's art, mirrored to face the party).</summary>
public abstract class JerichoMonster : CustomMonsterModel, Mechanics.IPhotoArt
{
    protected abstract string ArtFolder { get; }
    protected abstract float BodyHeight { get; }
    protected virtual bool TipOver => true;

    private string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/{ArtFolder}/{file}";

    // Family Album photos (IPhotoArt).
    string Mechanics.IPhotoArt.PhotoArt => ArtPath("frames/idle_0.png");
    float Mechanics.IPhotoArt.PhotoHeight => BodyHeight;
    bool Mechanics.IPhotoArt.PhotoTipOver => TipOver;

    private const string RaiderSfx = "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider";
    public override string? CustomAttackSfx => RaiderSfx + "_attack";
    public override string? CustomDeathSfx => RaiderSfx + "_die";

    public override IEnumerable<string> AssetPaths => base.AssetPaths.Skip(1);

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        try
        {
            if (PixelBody.LoadFrames(ArtPath, 5, out var first) is { } frames && first != null)
                return PixelBody.AnimatedBody(frames, first, BodyHeight, faceLeft: true, tipOver: TipOver);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build the {ArtFolder} body: {e}");
        }
        return null;
    }

    protected static Creature? Wall(Creature anyone) =>
        anyone.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is WallOfJericho);

    /// <summary>The defenders lose their footing when the walls fall (SevenTrumpetsPower).</summary>
    public static bool Fallen(Creature anyone) => Wall(anyone)?.Monster is WallOfJericho { IsFallen: true };
}

/// <summary>
/// The Walls of Jericho (Joshua 6), an Act 2 elite. The Wall is the one to beat (Act 2 elites run 145-155 HP); it
/// barely attacks, it stacks Block, and its defenders are minions, so the fight ends when it falls.
/// SevenTrumpetsPower counts down from 7: once per round as the party marches around, and once more for every Song or
/// Chorus anyone plays. At 0 the walls come tumbling down: the Wall loses its Block, becomes Vulnerable for good, and
/// stops acting; the defenders fall and lose half their HP.
/// Moves: Fortify (Block), Falling Rubble (attack), repeat; Crumbling once fallen.
/// </summary>
public class WallOfJericho : JerichoMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 160, 150);
    public override int MaxInitialHp => MinInitialHp;

    private int FortifyBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 16);
    private int RubbleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);

    protected override string ArtFolder => "wall_of_jericho";
    protected override float BodyHeight => 400f;
    // It crumbles in its own frames.
    protected override bool TipOver => false;
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<SevenTrumpetsPower>(new ThrowingPlayerChoiceContext(), Creature, SevenTrumpetsPower.Start, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var fortify = new MoveState("FORTIFY", Fortify, new DefendIntent());
        var rubble = new MoveState("FALLING_RUBBLE", FallingRubble, new SingleAttackIntent(RubbleDamage));
        var crumbling = new MoveState("CRUMBLING", _ => Task.CompletedTask, new StunIntent());
        var branch = new ConditionalBranchState("WALL_STANDING");
        fortify.FollowUpState = rubble;
        rubble.FollowUpState = branch;
        crumbling.FollowUpState = crumbling;
        branch.AddState(fortify, () => !Fallen(Creature));
        branch.AddState(crumbling, () => Fallen(Creature));
        return new MonsterMoveStateMachine([fortify, rubble, crumbling, branch], fortify);
    }

    private async Task Fortify(IReadOnlyList<Creature> targets)
    {
        if (Fallen(Creature)) return;
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.GainBlock(Creature, FortifyBlock, ValueProp.Move, null);
    }

    private async Task FallingRubble(IReadOnlyList<Creature> targets)
    {
        if (Fallen(Creature)) return;
        await DamageCmd.Attack(RubbleDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/flail_knight/flail_knight_ram")
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    public bool IsFallen { get; private set; }

    /// <summary>The walls fall (SevenTrumpetsPower at 0).</summary>
    public async Task Collapse()
    {
        if (IsFallen) return;
        IsFallen = true;
        var ctx = new ThrowingPlayerChoiceContext();
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_die");
        await CreatureCmd.TriggerAnim(Creature, "Hit", 0f);
        await CreatureCmd.LoseBlock(Creature, Creature.Block);
        await PowerCmd.Apply<VulnerablePower>(ctx, Creature, 99, Creature, null);
        var defenders = Creature.CombatState?.Enemies.Where(e => e.IsAlive && e != Creature).ToList() ?? [];
        foreach (var d in defenders)
            await CreatureCmd.Damage(ctx, d, Math.Ceiling(d.CurrentHp / 2m), ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }
}

/// <summary>A gate guard: holds the gate (Block for the Wall) or thrusts his spear. A minion.</summary>
public class JerichoGuard : JerichoMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 34, 30);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 36, 32);

    private int GateBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);
    private int SpearDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);

    protected override string ArtFolder => "jericho_guard";
    protected override float BodyHeight => 350f;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var spear = new MoveState("SPEAR_THRUST", SpearThrust, new SingleAttackIntent(SpearDamage));
        var gate = new MoveState("HOLD_THE_GATE", HoldTheGate, new DefendIntent());
        spear.FollowUpState = gate;
        gate.FollowUpState = spear;
        return new MonsterMoveStateMachine([spear, gate], spear);
    }

    private async Task SpearThrust(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SpearDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    // Shores up the Wall; once it has fallen there's nothing to hold, so he ducks behind the rubble himself.
    private async Task HoldTheGate(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        var wall = Fallen(Creature) ? null : Wall(Creature);
        await CreatureCmd.GainBlock(wall ?? Creature, GateBlock, ValueProp.Move, null);
    }
}

/// <summary>An archer on the ramparts: a two-arrow volley or one aimed shot. A minion.</summary>
public class JerichoArcher : JerichoMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 30, 26);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 32, 28);

    private int VolleyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    private const int Arrows = 2;
    private int AimedDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 11);

    protected override string ArtFolder => "jericho_archer";
    protected override float BodyHeight => 350f;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var volley = new MoveState("VOLLEY", Volley, new MultiAttackIntent(VolleyDamage, Arrows));
        var aimed = new MoveState("AIMED_SHOT", AimedShot, new SingleAttackIntent(AimedDamage));
        volley.FollowUpState = aimed;
        aimed.FollowUpState = volley;
        return new MonsterMoveStateMachine([volley, aimed], aimed);
    }

    private async Task Volley(IReadOnlyList<Creature> targets)
    {
        if (!Fallen(Creature)) (Creature.CombatState?.Encounter as BibleElite)?.Taunt(Creature);
        await DamageCmd.Attack(VolleyDamage).WithHitCount(Arrows).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    private async Task AimedShot(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(AimedDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }
}
