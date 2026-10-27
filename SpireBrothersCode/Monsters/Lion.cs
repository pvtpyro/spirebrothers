using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The Lions' Den (Daniel 6), an Act 1 elite: a Lion and a Lioness, about 100 HP between them like the vanilla Act 1
/// elites. Moves loop Roar (every lion in the den gains Strength), Pounce, Maul; the Lioness starts on Pounce so they
/// don't roar on the same turn. The Lion carries DenOfLionsPower (the talking, Daniel's Shut Their Mouths).
/// Body: images/monsters/lion/frames/ (pixelgen's Beasts.cs), mirrored to face the party.
/// </summary>
public class Lion : CustomMonsterModel
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 58, 54);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 60, 56);

    private int PounceDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 10);
    private int MaulDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    private const int Mauls = 2;
    private int RoarStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    protected virtual string ArtFolder => "lion";
    protected virtual float BodyHeight => 260f;
    /// <summary>The Lioness starts on Pounce, so the two of them take turns roaring.</summary>
    protected virtual bool StartsWithRoar => true;
    protected virtual bool Talks => true;

    private string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/{ArtFolder}/{file}";

    // Borrowed from the vanilla Byrdonis (a big Act 1 beast). Without these the hits are silent.
    private const string BeastSfx = "event:/sfx/enemy/enemy_attacks/byrdonis/byrdonis";
    public override string? CustomAttackSfx => BeastSfx + "_attack";
    public override string? CustomDeathSfx => BeastSfx + "_die";

    public override IEnumerable<string> AssetPaths => base.AssetPaths.Skip(1);

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        try
        {
            if (PixelBody.LoadFrames(ArtPath, 5, out var first) is { } frames && first != null)
                return PixelBody.AnimatedBody(frames, first, BodyHeight, faceLeft: true, tipOver: false);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build the {ArtFolder}'s body: {e}");
        }
        return null;
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        if (Talks) await PowerCmd.Apply<DenOfLionsPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var roar = new MoveState("ROAR", Roar, new BuffIntent());
        var pounce = new MoveState("POUNCE", Pounce, new SingleAttackIntent(PounceDamage));
        var maul = new MoveState("MAUL", Maul, new MultiAttackIntent(MaulDamage, Mauls));
        roar.FollowUpState = pounce;
        pounce.FollowUpState = maul;
        maul.FollowUpState = roar;
        return new MonsterMoveStateMachine([roar, pounce, maul], StartsWithRoar ? roar : pounce);
    }

    private async Task Roar(IReadOnlyList<Creature> targets)
    {
        if (Talks) (Creature.CombatState?.Encounter as BibleElite)?.Taunt(Creature);
        SfxCmd.Play(AttackSfx);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        var den = Creature.CombatState?.Enemies.Where(e => e.IsAlive && e.Monster is Lion).ToList() ?? [Creature];
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), den, RoarStrength, Creature, null);
    }

    private async Task Pounce(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(PounceDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    private async Task Maul(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(MaulDamage).WithHitCount(Mauls).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }
}

/// <summary>The Lion's mate: a little smaller, no mane, and she starts on Pounce.</summary>
public class Lioness : Lion
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 50, 46);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 52, 48);
    protected override string ArtFolder => "lioness";
    protected override float BodyHeight => 230f;
    protected override bool StartsWithRoar => false;
    protected override bool Talks => false;
}
