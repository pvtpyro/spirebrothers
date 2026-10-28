using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.Shared;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Windows Update, an Act 2 normal fight: one laptop that will not stop updating.</summary>
public class WindowsUpdateNormal() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Laptop>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [(ModelDb.Monster<Laptop>().ToMutable(), null)];
}

/// <summary>
/// Alternates Pop-up (an attack that puts a Close Ad junk card in your hand) and Buffering (Block). After each of its
/// turns the update goes up 25% (UpdatingPower). At 100% its next move becomes Restart: it heals to full, hits hard,
/// and starts over. A race to finish it before the restart.
/// </summary>
public class Laptop : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 98, 90);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 102, 94);
    private int PopupDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);
    private int BufferBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 12);
    private int RestartDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);

    protected override string ArtFolder => "laptop";
    protected override float BodyHeight => 200f;

    private const string BotSfx = "event:/sfx/enemy/enemy_attacks/axebot/axebot";
    public override string? CustomAttackSfx => BotSfx + "_attack_spin";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_minion_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var popup = new MoveState("POPUP", Popup, new SingleAttackIntent(PopupDamage), new StatusIntent(1));
        var buffer = new MoveState("BUFFERING", Buffering, new DefendIntent());
        var restart = new MoveState("RESTART", Restart, new SingleAttackIntent(RestartDamage), new HealIntent());
        // The game rolls the next move right after each move, so the 100% check is a branch, not a mid-move switch.
        var afterPopup = new ConditionalBranchState("AFTER_POPUP");
        afterPopup.AddState(restart, () => UpdateDone);
        afterPopup.AddState(buffer, () => !UpdateDone);
        var afterBuffer = new ConditionalBranchState("AFTER_BUFFERING");
        afterBuffer.AddState(restart, () => UpdateDone);
        afterBuffer.AddState(popup, () => !UpdateDone);
        popup.FollowUpState = afterPopup;
        buffer.FollowUpState = afterBuffer;
        restart.FollowUpState = popup;
        return new MonsterMoveStateMachine([popup, buffer, restart, afterPopup, afterBuffer], popup);
    }

    private bool UpdateDone => Creature.GetPowerAmount<UpdatingPower>() >= UpdatingPower.Steps;

    /// <summary>After a regular move: the update goes up a step. At 100% the branch makes the next move Restart.</summary>
    private Task Progress() =>
        PowerCmd.Apply<UpdatingPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);

    private async Task Popup(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(PopupDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(null);
        await CardPileCmd.AddToCombatAndPreview<CloseAd>(targets, PileType.Hand, 1, null);
        await Progress();
    }

    private async Task Buffering(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.GainBlock(Creature, BufferBlock, ValueProp.Move, null);
        await Progress();
    }

    private async Task Restart(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.6f);
        if (Creature.GetPower<UpdatingPower>() is { } updating) await PowerCmd.Remove(updating);
        await CreatureCmd.Heal(Creature, Creature.MaxHp - Creature.CurrentHp);
        await DamageCmd.Attack(RestartDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }
}
