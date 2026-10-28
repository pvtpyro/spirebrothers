using BaseLib.Abstracts;
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

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The Jackals, an easy Act 1 fight: two jackals, about 22 HP between them. They alternate Bite (2 x2) and Lunge, out
/// of step with each other. When one dies, the other howls for its packmate and gains Strength.
/// </summary>
public class TheJackalsWeak() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;
    public override bool IsWeak => true;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Jackal>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        Enumerable.Range(0, 2).Select(i =>
        {
            var jackal = (Jackal)ModelDb.Monster<Jackal>().ToMutable();
            jackal.LungesFirst = i == 1;
            return ((MonsterModel)jackal, (string?)null);
        }).ToList();
}

public class Jackal : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 12, 11);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 13, 12);
    private int BiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private const int Bites = 2;
    private int LungeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private int HowlStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    /// <summary>The second jackal starts on Lunge, so the two are out of step. Set by the encounter.</summary>
    public bool LungesFirst { get; set; }

    protected override string ArtFolder => "jackal";
    protected override float BodyHeight => 150f;

    private const string RatSfx = "event:/sfx/enemy/enemy_attacks/two_tail_rats/two_tail_rats";
    public override string? CustomAttackSfx => RatSfx + "_attack_bite";
    public override string? CustomDeathSfx => RatSfx + "_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var bite = new MoveState("BITE", Bite, new MultiAttackIntent(BiteDamage, Bites));
        var lunge = new MoveState("LUNGE", Lunge, new SingleAttackIntent(LungeDamage));
        bite.FollowUpState = lunge;
        lunge.FollowUpState = bite;
        return new MonsterMoveStateMachine([bite, lunge], LungesFirst ? lunge : bite);
    }

    // Its packmate fell: it howls and gets meaner.
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        if (wasRemovalPrevented || creature == Creature || creature.Monster is not Jackal || !Creature.IsAlive) return;
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/byrdonis/byrdonis_attack");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, HowlStrength, Creature, null);
    }

    private async Task Bite(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BiteDamage).WithHitCount(Bites).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    private async Task Lunge(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(LungeDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }
}
