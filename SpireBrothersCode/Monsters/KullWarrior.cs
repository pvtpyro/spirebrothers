using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// One of Anubis's Kull Warriors (AnubisBoss): an armored super-soldier, all black plate with a blank faceplate. A minion
/// in Kull Armor (Attacks deal half damage to it). Fires its wrist blaster, then marches forward and grows stronger
/// (Strength, so a Kull left alone keeps hitting harder), and around again; one of the starting pair opens with the march. Also the body Anubis takes when he falls (HalfAscendedPower).
/// </summary>
public class KullWarrior : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 48, 44);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 50, 46);
    private int BlasterDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 11);
    // (March was 8 Block until 2026-10-30: with Kull Armor halving Attacks first, it soaked 16 attack damage. Too tanky.)
    private int MarchStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    /// <summary>Set by the encounter so the pair doesn't do the same thing each turn.</summary>
    public bool MarchFirst { get; set; }

    protected override string ArtFolder => "kull_warrior";
    protected override float BodyHeight => 330f;
    protected override bool TipOver => true;

    private const string BlasterSfx = "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider";
    public override string? CustomAttackSfx => BlasterSfx + "_attack";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<MinionPower>(ctx, Creature, 1, null, null);
        await PowerCmd.Apply<KullArmorPower>(ctx, Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var blaster = new MoveState("WRIST_BLASTER", WristBlaster, new SingleAttackIntent(BlasterDamage));
        var march = new MoveState("MARCH", March, new BuffIntent());
        blaster.FollowUpState = march;
        march.FollowUpState = blaster;
        return new MonsterMoveStateMachine([blaster, march], MarchFirst ? march : blaster);
    }

    private async Task WristBlaster(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BlasterDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    private async Task March(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, MarchStrength, Creature, null);
    }
}
