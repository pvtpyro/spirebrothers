using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
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

/// <summary>
/// Captain Blockbeard, the insult pirate (Monkey Island meets Minecraft). Act 3 elite, so by the time the brothers
/// meet him they've picked up Insult and Comeback cards; numbers sit with the vanilla Act 3 elites (Soul Nexus
/// 234 HP, Mecha Knight 300). His gimmick is the insult duel
/// (InsultDuelPower): Cutting Remark insults the party, and if nobody retorts before their turn ends he gloats and
/// gains Strength; a retort leaves him Speechless. Moves loop: Cutting Remark, Swashbuckle, Look Behind You!,
/// Cutting Remark, Swig of Grog, Swashbuckle.
/// Body: images/monsters/blockbeard/frames/ (pixelgen's PirateArt), mirrored to face the party.
/// </summary>
public class Blockbeard : CustomMonsterModel, Mechanics.IPhotoArt
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 255, 240);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 265, 250);

    private int RemarkDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);
    private int SlashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 8);
    private const int Slashes = 4;
    private const int WeakAmount = 2, VulnerableAmount = 2;
    private int GrogHeal => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 25);
    private int GrogBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 25, 20);

    /// <summary>How tall he stands in combat (the brothers are 400). A little bigger: he's an elite.</summary>
    private const float BodyHeight = 460f;

    private static string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/blockbeard/{file}";

    // Family Album photos (IPhotoArt).
    string Mechanics.IPhotoArt.PhotoArt => ArtPath("frames/idle_0.png");
    float Mechanics.IPhotoArt.PhotoHeight => BodyHeight;

    // Sounds borrowed from the vanilla Ruby Raiders (fellow pirates). Without these the game looks for
    // ".../spirebrothers-blockbeard/..._attack", which doesn't exist, and his hits are silent.
    private const string RaiderSfx = "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider";
    public override string? CustomAttackSfx => RaiderSfx + "_attack";
    public override string? CustomDeathSfx => RaiderSfx + "_die";

    // His body is built in code from frames, so there's no scene to preload; keep the intents' assets.
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
            MainFile.Logger.Error($"Couldn't build Blockbeard's body: {e}");
        }
        return null;
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<InsultDuelPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var remark1 = new MoveState("CUTTING_REMARK", CuttingRemark, new SingleAttackIntent(RemarkDamage), new BuffIntent());
        var slash1 = new MoveState("SWASHBUCKLE", Swashbuckle, new MultiAttackIntent(SlashDamage, Slashes));
        var trick = new MoveState("LOOK_BEHIND_YOU", LookBehindYou, new DebuffIntent());
        var remark2 = new MoveState("CUTTING_REMARK_2", CuttingRemark, new SingleAttackIntent(RemarkDamage), new BuffIntent());
        var grog = new MoveState("SWIG_OF_GROG", SwigOfGrog, new DefendIntent(), new BuffIntent());
        var slash2 = new MoveState("SWASHBUCKLE_2", Swashbuckle, new MultiAttackIntent(SlashDamage, Slashes));
        remark1.FollowUpState = slash1;
        slash1.FollowUpState = trick;
        trick.FollowUpState = remark2;
        remark2.FollowUpState = grog;
        grog.FollowUpState = slash2;
        slash2.FollowUpState = remark1;
        return new MonsterMoveStateMachine([remark1, slash1, trick, remark2, grog, slash2], remark1);
    }

    // The insult: the party has until the end of their next turn to retort (see InsultDuelPower).
    private async Task CuttingRemark(IReadOnlyList<Creature> targets)
    {
        if (Creature.GetPower<InsultDuelPower>() is { } duel) await duel.Insult();
        await DamageCmd.Attack(RemarkDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await GloatingPower.Spend(Creature);
    }

    private async Task Swashbuckle(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SlashDamage).WithHitCount(Slashes).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await GloatingPower.Spend(Creature);
    }

    // "Look behind you!" Everyone looks. Cheap shot.
    private async Task LookBehindYou(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/debuff");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakAmount, Creature, null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, VulnerableAmount, Creature, null);
    }

    private async Task SwigOfGrog(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play(FmodSfx.heal);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await CreatureCmd.Heal(Creature, GrogHeal);
        await CreatureCmd.GainBlock(Creature, GrogBlock, ValueProp.Move, null);
    }
}
