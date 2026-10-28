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
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>The Enderman (Minecraft), an Act 2 normal fight: just him.</summary>
public class TheEndermanNormal() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Enderman>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [(ModelDb.Monster<Enderman>().ToMutable(), null)];
}

/// <summary>
/// Mostly calm: strikes, stares (Weak), teleports (vanilla Intangible 2, so the party's next turn only does 1 damage per
/// hit). Hit him 3 times in one turn and he's provoked (ProvokedPower): his next move turns into Ender Rage, a big hit.
/// That rewards a few big hits over lots of small ones, and 1-damage hits while he's teleported still count.
/// </summary>
public class Enderman : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 118, 110);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 122, 114);
    private int StrikeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 8);
    private int StareDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int StareWeak = 2;
    private int RageDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 25, 22);
    private const int TeleportIntangible = 2;   // Intangible ticks down at the end of the enemy turn: 2 covers the party's next turn

    protected override string ArtFolder => "enderman";
    protected override float BodyHeight => 430f;

    private const string GhostSfx = "event:/sfx/enemy/enemy_attacks/obscura/obscura";
    public override string? CustomAttackSfx => GhostSfx + "_attack";
    public override string? CustomDeathSfx => GhostSfx + "_die";

    private MoveState? _rage;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<ProvokedPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var strike = new MoveState("STRIKE", Strike, new SingleAttackIntent(StrikeDamage));
        var stare = new MoveState("STARE", Stare, new SingleAttackIntent(StareDamage), new DebuffIntent());
        var teleport = new MoveState("TELEPORT", Teleport, new BuffIntent());
        _rage = new MoveState("ENDER_RAGE", Rage, new SingleAttackIntent(RageDamage));
        strike.FollowUpState = stare;
        stare.FollowUpState = teleport;
        teleport.FollowUpState = strike;
        _rage.FollowUpState = strike;   // replaced in Provoke by the move the rage interrupted
        return new MonsterMoveStateMachine([strike, stare, teleport, _rage], strike);
    }

    /// <summary>Called by ProvokedPower on the third hit in a turn. Does nothing if he's already set to rage. Afterwards he does the move the rage replaced.</summary>
    public void Provoke()
    {
        if (_rage == null || NextMove == _rage || !Creature.IsAlive) return;
        _rage.FollowUpState = NextMove;   // the rage cuts in line, then he goes back to what he was about to do
        SfxCmd.Play(GhostSfx + "_buff");
        SetMoveImmediate(_rage, forceTransition: true);
    }

    private async Task Strike(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(StrikeDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(null);
    }

    private async Task Stare(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(StareDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, StareWeak, Creature, null);
    }

    private async Task Teleport(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(GhostSfx + "_summon");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.6f);
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), Creature, TeleportIntangible, Creature, null);
    }

    private async Task Rage(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(RageDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }
}
