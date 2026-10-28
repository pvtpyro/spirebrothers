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
/// Chores, an Act 2 normal fight: a Laundry Pile, Dirty Dishes, and an Overgrown Lawn, about 100 HP in all. Each one
/// carries PilingUpPower: any chore nobody hit during the party's turn gains Strength. You can't ignore any of them.
/// </summary>
public class ChoresNormal() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<LaundryPile>(), ModelDb.Monster<DirtyDishes>(), ModelDb.Monster<OvergrownLawn>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<LaundryPile>().ToMutable(), null),
        (ModelDb.Monster<DirtyDishes>().ToMutable(), null),
        (ModelDb.Monster<OvergrownLawn>().ToMutable(), null),
    ];
}

/// <summary>Shared by the three chores: the Piling Up power, sounds, and the two-move loop.</summary>
public abstract class Chore : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 35, 32);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 37, 34);

    public override string? CustomAttackSfx => "event:/sfx/enemy/enemy_attacks/axebot/axebot_attack_spin";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_minion_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<PilingUpPower>(new ThrowingPlayerChoiceContext(), Creature, PilingUpPower.StrengthPerTurn, Creature, null);
    }

    protected async Task Hit(int damage, int times = 1, string vfx = "vfx/vfx_attack_blunt")
    {
        await DamageCmd.Attack(damage).WithHitCount(times).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx(vfx)
            .Execute(null);
    }
}

/// <summary>Piles on (two hits), then folds itself up (Block).</summary>
public class LaundryPile : Chore
{
    private int PileOnDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    private int FoldBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);

    protected override string ArtFolder => "laundry_pile";
    protected override float BodyHeight => 170f;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var pileOn = new MoveState("PILE_ON", _ => Hit(PileOnDamage, 2), new MultiAttackIntent(PileOnDamage, 2));
        var fold = new MoveState("FOLD", Fold, new DefendIntent());
        pileOn.FollowUpState = fold;
        fold.FollowUpState = pileOn;
        return new MonsterMoveStateMachine([pileOn, fold], pileOn);
    }

    private async Task Fold(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.GainBlock(Creature, FoldBlock, ValueProp.Move, null);
    }
}

/// <summary>Stacks up (Block for every chore), then clatters (a big hit). Starts on Stack so the three don't all attack at once.</summary>
public class DirtyDishes : Chore
{
    private int ClatterDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);
    private int StackBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    protected override string ArtFolder => "dirty_dishes";
    protected override float BodyHeight => 200f;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var stack = new MoveState("STACK", Stack, new DefendIntent());
        var clatter = new MoveState("CLATTER", _ => Hit(ClatterDamage), new SingleAttackIntent(ClatterDamage));
        stack.FollowUpState = clatter;
        clatter.FollowUpState = stack;
        return new MonsterMoveStateMachine([stack, clatter], stack);
    }

    private async Task Stack(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        var chores = Creature.CombatState?.Enemies.Where(e => e.IsAlive && e.Monster is Chore).ToList() ?? [Creature];
        foreach (var chore in chores) await CreatureCmd.GainBlock(chore, StackBlock, ValueProp.Move, null);
    }
}

/// <summary>Grass stains (a hit with Weak), then creeps in (a bigger hit).</summary>
public class OvergrownLawn : Chore
{
    private int StainDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int StainWeak = 1;
    private int CreepDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 10);

    protected override string ArtFolder => "overgrown_lawn";
    protected override float BodyHeight => 150f;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var stains = new MoveState("GRASS_STAINS", GrassStains, new SingleAttackIntent(StainDamage), new DebuffIntent());
        var creep = new MoveState("CREEP_IN", _ => Hit(CreepDamage, vfx: "vfx/vfx_attack_slash"), new SingleAttackIntent(CreepDamage));
        stains.FollowUpState = creep;
        creep.FollowUpState = stains;
        return new MonsterMoveStateMachine([stains, creep], stains);
    }

    private async Task GrassStains(IReadOnlyList<Creature> targets)
    {
        await Hit(StainDamage, vfx: "vfx/vfx_attack_slash");
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, StainWeak, Creature, null);
    }
}
