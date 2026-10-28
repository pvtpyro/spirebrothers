using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.Shared;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Runaway Roomba, an easy Act 1 fight: one robot vacuum that has had enough.</summary>
public class RunawayRoombaWeak() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;
    public override bool IsWeak => true;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Roomba>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [(ModelDb.Monster<Roomba>().ToMutable(), null)];
}

/// <summary>Bumps you, backs off (Block), then vacuums: Dust Bunnies into your discard pile, like vanilla slimes' Slimed.</summary>
public class Roomba : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 35, 32);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 37, 34);
    private int BumpDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private int BackOffBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 7);
    private const int DustBunnies = 2;

    protected override string ArtFolder => "roomba";
    protected override float BodyHeight => 90f;

    private const string BotSfx = "event:/sfx/enemy/enemy_attacks/axebot/axebot";
    public override string? CustomAttackSfx => BotSfx + "_attack_spin";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_minion_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var bump = new MoveState("BUMP", Bump, new SingleAttackIntent(BumpDamage));
        var backOff = new MoveState("BACK_OFF", BackOff, new DefendIntent());
        var vacuum = new MoveState("VACUUM", Vacuum, new StatusIntent(DustBunnies));
        bump.FollowUpState = backOff;
        backOff.FollowUpState = vacuum;
        vacuum.FollowUpState = bump;
        return new MonsterMoveStateMachine([bump, backOff, vacuum], bump);
    }

    private async Task Bump(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BumpDamage).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(null);
    }

    private async Task BackOff(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.GainBlock(Creature, BackOffBlock, ValueProp.Move, null);
    }

    private async Task Vacuum(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await CardPileCmd.AddToCombatAndPreview<DustBunny>(targets, PileType.Discard, DustBunnies, null);
    }
}
