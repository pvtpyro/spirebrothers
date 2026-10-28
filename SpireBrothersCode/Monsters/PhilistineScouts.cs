using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Philistine Scouts (Goliath's army), an Act 1 normal fight: three scouts, about 48 HP in all. They share one
/// Guard, Thrust, Thrust cycle but each starts at a different point in it, so every round exactly one of them guards
/// (Block for the other two) while two attack. Kill the guard and that round's Block never comes.
/// </summary>
public class PhilistineScoutsNormal() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<PhilistineScout>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        Enumerable.Range(0, 3).Select(i =>
        {
            var scout = (PhilistineScout)ModelDb.Monster<PhilistineScout>().ToMutable();
            scout.StartStep = i;
            return ((MonsterModel)scout, (string?)null);
        }).ToList();
}

public class PhilistineScout : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 16, 15);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 17, 16);
    private int ThrustDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private int GuardBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    /// <summary>Where in the Guard, Thrust, Thrust cycle this scout starts (0 = guards first). Set by the encounter.</summary>
    public int StartStep { get; set; }

    protected override string ArtFolder => "philistine_scout";
    protected override float BodyHeight => 300f;
    protected override bool TipOver => true;

    private const string RaiderSfx = "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider";
    public override string? CustomAttackSfx => RaiderSfx + "_attack";
    public override string? CustomDeathSfx => RaiderSfx + "_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var guard = new MoveState("GUARD", Guard, new DefendIntent());
        var thrust = new MoveState("THRUST", Thrust, new SingleAttackIntent(ThrustDamage));
        var thrustAgain = new MoveState("THRUST_AGAIN", Thrust, new SingleAttackIntent(ThrustDamage));
        guard.FollowUpState = thrust;
        thrust.FollowUpState = thrustAgain;
        thrustAgain.FollowUpState = guard;
        var start = (StartStep % 3) switch { 0 => guard, 1 => thrustAgain, _ => thrust };
        return new MonsterMoveStateMachine([guard, thrust, thrustAgain], start);
    }

    // Raises his shield over the others (or himself, if he's the last one standing).
    private async Task Guard(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        var others = Creature.CombatState?.Enemies.Where(e => e.IsAlive && e != Creature && e.Monster is PhilistineScout).ToList() ?? [];
        if (others.Count == 0) others = [Creature];
        foreach (var scout in others) await CreatureCmd.GainBlock(scout, GuardBlock, ValueProp.Move, null);
    }

    private async Task Thrust(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ThrustDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }
}
