using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
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
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Shared body and sound code for the coppersmith's shop (pixelgen's art, mirrored to face the party).</summary>
public abstract class CopperMonster : CustomMonsterModel
{
    protected abstract string ArtFolder { get; }
    protected abstract float BodyHeight { get; }

    private string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/{ArtFolder}/{file}";

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
            MainFile.Logger.Error($"Couldn't build the {ArtFolder} body: {e}");
        }
        return null;
    }
}

/// <summary>
/// Alexander the Coppersmith ("did me much harm", 2 Timothy 4:14), an Act 2 elite; numbers sit with the vanilla Act 2
/// elites (Entomancer 145 HP, Fabricator 150). Like the Fabricator he builds helpers: Forge adds a Copper Automaton
/// (a minion) while fewer than two are standing; otherwise he does Much Harm (Weak and Frail to everyone).
/// Moves: Hammer, Hot Rivets, Forge or Much Harm, repeat. CoppersmithPower does the talking and Tim's Sound Doctrine.
/// </summary>
public class Alexander : CopperMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 158, 150);
    public override int MaxInitialHp => MinInitialHp;

    private int HammerDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 19, 17);
    private int RivetDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int Rivets = 3;
    private const int HarmAmount = 2;
    public const int MaxAutomatons = 2;

    protected override string ArtFolder => "alexander";
    protected override float BodyHeight => 430f;

    private const string SmithSfx = "event:/sfx/enemy/enemy_attacks/flail_knight/flail_knight_ram";
    public override string? CustomAttackSfx => SmithSfx;
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<CoppersmithPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    private bool CanForge => (Creature.CombatState?.Enemies.Count(e => e.IsAlive && e.Monster is CopperAutomaton) ?? 0) < MaxAutomatons;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var hammer = new MoveState("HAMMER", Hammer, new SingleAttackIntent(HammerDamage));
        var rivets = new MoveState("HOT_RIVETS", HotRivets, new MultiAttackIntent(RivetDamage, Rivets));
        var forge = new MoveState("FORGE", Forge, new SummonIntent());
        var harm = new MoveState("MUCH_HARM", MuchHarm, new DebuffIntent());
        var branch = new ConditionalBranchState("FORGE_OR_HARM");
        hammer.FollowUpState = rivets;
        rivets.FollowUpState = branch;
        forge.FollowUpState = hammer;
        harm.FollowUpState = hammer;
        branch.AddState(forge, () => CanForge);
        branch.AddState(harm, () => !CanForge);
        return new MonsterMoveStateMachine([hammer, rivets, forge, harm, branch], hammer);
    }

    private async Task Hammer(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(HammerDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    private async Task HotRivets(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(RivetDamage).WithHitCount(Rivets).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    private async Task Forge(IReadOnlyList<Creature> targets)
    {
        (Creature.CombatState?.Encounter as BibleElite)?.Taunt(Creature);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await Summon(Creature, ModelDb.Monster<CopperAutomaton>());
    }

    // "Alexander the coppersmith did me much harm."
    private async Task MuchHarm(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/debuff");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<WeakPower>(ctx, targets, HarmAmount, Creature, null);
        await PowerCmd.Apply<FrailPower>(ctx, targets, HarmAmount, Creature, null);
    }

    /// <summary>
    /// Adds a minion mid-fight, like the vanilla Fabricator. Our encounters have no scene with named slots, so the game
    /// doesn't place newcomers; this lines every living enemy back up with the game's own layout (PositionEnemies),
    /// helpers in front, and slides the old ones over.
    /// </summary>
    public static async Task Summon(Creature summoner, MonsterModel monster)
    {
        var combat = summoner.CombatState;
        if (combat == null || !combat.IsLiveCombat()) return;
        var added = await CreatureCmd.Add(monster.ToMutable(), combat, CombatSide.Enemy, null);
        if (added.GetPower<MinionPower>() == null)
            await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), added, 1, summoner, null);
        try
        {
            Relayout(combat, added);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't line up the enemies after a summon: {e}");
        }
    }

    private static void Relayout(ICombatState combat, Creature added)
    {
        if (NCombatRoom.Instance is not { } room) return;
        var nodes = combat.Enemies.Where(e => e.IsAlive)
            .OrderBy(e => e.Monster is CopperAutomaton ? 0 : 1)
            .Select(room.GetCreatureNode).OfType<NCreature>().ToList();
        var before = nodes.ToDictionary(n => n, n => n.Position);
        AccessTools.Method(typeof(NCombatRoom), "PositionEnemies")
            .Invoke(room, [nodes, combat.Encounter?.GetCameraScaling() ?? 1f]);
        foreach (var node in nodes)
        {
            if (node.Entity == added) continue;
            var target = node.Position;
            node.Position = before[node];
            node.CreateTween().TweenProperty(node, "position", target, 0.35).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        }
    }
}

/// <summary>A copper construct off Alexander's anvil: a clank or a grind. A minion.</summary>
public class CopperAutomaton : CopperMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 24, 20);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 26, 22);

    private int ClankDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);
    private int GrindDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private const int Grinds = 2;

    protected override string ArtFolder => "copper_automaton";
    protected override float BodyHeight => 260f;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    private const string BotSfx = "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_heavy_attack";
    public override string? CustomAttackSfx => BotSfx;
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_die";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var clank = new MoveState("CLANK", Clank, new SingleAttackIntent(ClankDamage));
        var grind = new MoveState("GRIND", Grind, new MultiAttackIntent(GrindDamage, Grinds));
        clank.FollowUpState = grind;
        grind.FollowUpState = clank;
        return new MonsterMoveStateMachine([clank, grind], clank);
    }

    private async Task Clank(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ClankDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    private async Task Grind(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(GrindDamage).WithHitCount(Grinds).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }
}
