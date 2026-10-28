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

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Goliath's Shield Bearer ("one bearing a shield went before him", 1 Samuel 17:7). A minion: he stands in front of
/// Goliath, and every other turn gives him Block. Knock him out first, or chew through the Block. Goliath falling ends
/// the fight even if he's still standing (vanilla MinionPower).
/// Body: images/monsters/shield_bearer/frames/ (pixelgen's ShieldBearerArt), mirrored to face the party.
/// </summary>
public class ShieldBearer : CustomMonsterModel, Mechanics.IPhotoArt
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 30, 26);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 32, 28);

    private int ShieldBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);
    private int BashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private const float BodyHeight = 380f;

    private static string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/shield_bearer/{file}";

    // Family Album photos (IPhotoArt).
    string Mechanics.IPhotoArt.PhotoArt => ArtPath("frames/idle_0.png");
    float Mechanics.IPhotoArt.PhotoHeight => BodyHeight;

    private const string RaiderSfx = "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider";
    public override string? CustomAttackSfx => RaiderSfx + "_attack";
    public override string? CustomDeathSfx => RaiderSfx + "_die";

    public override IEnumerable<string> AssetPaths => base.AssetPaths.Skip(1);

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        try
        {
            if (PixelBody.LoadFrames(ArtPath, 5, out var first) is { } frames && first != null)
                return PixelBody.AnimatedBody(frames, first, BodyHeight, faceLeft: true);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build the Shield Bearer's body: {e}");
        }
        return null;
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var raise = new MoveState("RAISE_SHIELD", RaiseShield, new DefendIntent());
        var bash = new MoveState("SHIELD_BASH", ShieldBash, new SingleAttackIntent(BashDamage));
        raise.FollowUpState = bash;
        bash.FollowUpState = raise;
        return new MonsterMoveStateMachine([raise, bash], raise);
    }

    private Creature? Goliath => Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Goliath);

    // Holds the shield up in front of Goliath. If Goliath is gone, he hides behind it himself.
    private async Task RaiseShield(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.block);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.GainBlock(Goliath ?? Creature, ShieldBlock, ValueProp.Move, null);
    }

    private async Task ShieldBash(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BashDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }
}
