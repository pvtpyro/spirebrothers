using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
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
/// Poxwalker Horde (Darktide), an Act 2 normal fight: four poxwalkers, and at the end of every enemy turn one more
/// shuffles in, up to six at once. Weak alone, scary in a crowd: hit-everyone cards and the Mastiff shine here. The
/// newcomers aren't minions, so the fight ends when every poxwalker is down.
/// </summary>
public class PoxwalkerHordeNormal() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Poxwalker>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        Enumerable.Range(0, Poxwalker.StartingCount).Select(i =>
        {
            var walker = (Poxwalker)ModelDb.Monster<Poxwalker>().ToMutable();
            walker.LungesFirst = i % 2 == 1;
            return ((MonsterModel)walker, (string?)null);
        }).ToList();
}

/// <summary>Claws, then lunges (a smaller hit with Weak). Half the horde starts on the lunge.</summary>
public class Poxwalker : PixelMonster
{
    public const int StartingCount = 4, MaxAlive = 6;

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 13, 12);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 14, 13);
    private int ClawDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    private int LungeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private const int LungeWeak = 1;

    /// <summary>Set by the encounter so the horde doesn't all do the same thing each turn.</summary>
    public bool LungesFirst { get; set; }

    protected override string ArtFolder => "poxwalker";
    protected override float BodyHeight => 280f;
    protected override bool TipOver => true;

    private const string RaiderSfx = "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider";
    public override string? CustomAttackSfx => RaiderSfx + "_attack";
    public override string? CustomDeathSfx => RaiderSfx + "_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var claw = new MoveState("CLAW", Claw, new SingleAttackIntent(ClawDamage));
        var lunge = new MoveState("LUNGE", Lunge, new SingleAttackIntent(LungeDamage), new DebuffIntent());
        claw.FollowUpState = lunge;
        lunge.FollowUpState = claw;
        return new MonsterMoveStateMachine([claw, lunge], LungesFirst ? lunge : claw);
    }

    // At the end of the enemies' turn, one more shuffles in. Only the first poxwalker still standing calls it, so it's
    // one per round however many there are.
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        var combat = Creature.CombatState;
        if (side != CombatSide.Enemy || combat == null || !Creature.IsAlive) return;
        var horde = combat.Enemies.Where(e => e.IsAlive && e.Monster is Poxwalker).ToList();
        if (horde.Count == 0 || horde[0] != Creature || horde.Count >= MaxAlive) return;
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await Alexander.Summon(Creature, ModelDb.Monster<Poxwalker>(), minion: false);
    }

    private async Task Claw(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ClawDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
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
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, LungeWeak, Creature, null);
    }
}
