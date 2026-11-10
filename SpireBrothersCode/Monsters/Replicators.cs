using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.Shared;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The Replicators (Stargate SG-1): the mod's second Act 2 boss (the game rolls it or Apophis). The Replicator Queen stands
/// at the back and builds Replicator bugs (minions, at most 4 standing) out of loose blocks (ReplicatorSwarmPower: every bug
/// that falls apart gives her one block back), so the swarm keeps coming until she goes down. They adapt
/// (AdaptationPower, on all of them): whatever card last damaged a Replicator deals half damage to every Replicator until
/// a different card hits them; the swarm-wide memory lives here on the encounter.
/// The hero is a random player, like Anubis (the encounter's Rng): starts with the shared Anti-Replicator Gun token,
/// which the swarm can't adapt to. The next player answers.
/// Lines: monsters.json SPIREBROTHERS-REPLICATORS.*, plus the mid-fight pools adapt (the party, first time only) and build.
/// The map icon is images/map/replicators_boss_icon.png + _outline.png (mobgen).
/// </summary>
public class ReplicatorBoss() : BibleElite(RoomType.Boss)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<ReplicatorBug>(), ModelDb.Monster<ReplicatorQueen>()];

    private int _heroRoll;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        _heroRoll = Rng.NextInt(840);   // (divisible by every party size up to 8, so each player is as likely)
        AdaptedTo = null;
        AdaptedTitle = null;
        _pending = null;
        _toldAboutAdapting = false;
        var first = (ReplicatorBug)ModelDb.Monster<ReplicatorBug>().ToMutable();
        var second = (ReplicatorBug)ModelDb.Monster<ReplicatorBug>().ToMutable();
        second.ChewFirst = true;
        return [(first, null), (second, null), (ModelDb.Monster<ReplicatorQueen>().ToMutable(), null)];
    }

    public override float GetCameraScaling() => 0.8f;

    private const string IconPath = $"{MainFile.ResPath}/images/map/replicators_boss_icon";
    public override string BossNodePath => IconPath;
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;
    public override string? CustomRunHistoryIconPath => IconPath + ".png";
    public override string? CustomRunHistoryIconOutlinePath => IconPath + "_outline.png";

    public override string LinesKey => "REPLICATORS";

    /// <summary>0 for the hero, 1 for the one who answers, and so on round the party; -1 outside combat.</summary>
    private int Seat(Player p)
    {
        if (p.Creature.CombatState?.Players is not { Count: > 0 } players) return -1;
        int n = players.Count, i = players.ToList().IndexOf(p);
        return i < 0 ? -1 : ((i - _heroRoll) % n + n) % n;
    }

    protected override bool IsHero(Player p) => Seat(p) == 0;
    protected override bool IsSidekick(Player p) => Seat(p) == 1;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<AntiReplicatorGun>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is ReplicatorQueen;

    /// <summary>Front to back: the bugs, then the Queen (for summons mid-fight).</summary>
    public static int Order(Creature c) => c.Monster is ReplicatorBug ? 0 : 1;

    public Creature? PartyVoice(ICombatState combat) =>
        (Hero(combat) ?? Sidekick(combat) ?? combat.Players.FirstOrDefault(p => Banter.Talks(p) && p.Creature.IsAlive))?.Creature;

    // ---- Adaptation (AdaptationPower reads and feeds this) ----

    /// <summary>The card (by ID, so upgraded copies count too) the swarm has adapted to, or null.</summary>
    public string? AdaptedTo { get; private set; }
    /// <summary>That card's title, for the hover text.</summary>
    public string? AdaptedTitle { get; private set; }

    private CardModel? _pending;
    private bool _toldAboutAdapting;

    public bool IsAdaptedTo(CardModel? card) => card != null && AdaptedTo != null && card.Id.Entry == AdaptedTo;

    /// <summary>A card just damaged a Replicator: they'll adapt to it once it finishes (so an AoE or multi-hit stays whole).</summary>
    public void NoteHurtBy(CardModel card)
    {
        if (card is AntiReplicatorGun) return;
        _pending = card;
    }

    /// <summary>After a card resolves: if it hurt them, they adapt to it. True if that's a change.</summary>
    public bool Adapt(ICombatState combat)
    {
        if (_pending is not { } card) return false;
        _pending = null;
        if (card.Id.Entry == AdaptedTo) return false;
        AdaptedTo = card.Id.Entry;
        AdaptedTitle = card.Title;
        if (!_toldAboutAdapting && PartyVoice(combat) is { } voice)
        {
            _toldAboutAdapting = true;
            Line(voice, "adapt");
        }
        return true;
    }
}

/// <summary>
/// The Replicator Queen. 190 HP. Moves: Build (up to 2 bugs from her blocks, never more than 4 standing, and Block; with no
/// blocks or a full swarm, Reinforce instead: more Block), Assimilate (a big hit), Swarm (her bugs gain Strength and she
/// hits twice), and around again. Numbers sit with Apophis (200 HP plus two Jaffa).
/// </summary>
public class ReplicatorQueen : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 205, 190);
    public override int MaxInitialHp => MinInitialHp;

    private int AssimilateDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 17, 15);
    private int SwarmDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int SwarmHits = 2;
    private int SwarmStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    private const int BuildBlock = 8;
    private int ReinforceBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 14);
    public const int MaxBugs = 4;
    private const int BuildPerTurn = 2;
    private const int StartingBlocks = 2;

    /// <summary>Loose blocks she can build bugs from (ReplicatorSwarmPower shows it; every fallen bug adds one).</summary>
    public int Blocks { get; set; }

    protected override string ArtFolder => "replicator_queen";
    protected override float BodyHeight => 230f;
    protected override bool TipOver => false;   // her dead frames fall apart into blocks

    private const string BugSfx = "event:/sfx/enemy/enemy_attacks/axebot/axebot";
    public override string? CustomAttackSfx => BugSfx + "_attack_spin";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_die";

    private ReplicatorBoss? Boss => Creature.CombatState?.Encounter as ReplicatorBoss;

    private int BugsStanding => Creature.CombatState?.Enemies.Count(e => e.IsAlive && e.Monster is ReplicatorBug) ?? MaxBugs;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        Blocks = StartingBlocks;
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<ReplicatorSwarmPower>(ctx, Creature, 1, null, null);
        await PowerCmd.Apply<AdaptationPower>(ctx, Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var build = new MoveState("BUILD", Build, new SummonIntent(), new DefendIntent());
        var reinforce = new MoveState("REINFORCE", Reinforce, new DefendIntent());
        var branch = new ConditionalBranchState("BUILD_OR_REINFORCE");
        branch.AddState(build, () => Blocks > 0 && BugsStanding < MaxBugs);
        branch.AddState(reinforce, () => Blocks <= 0 || BugsStanding >= MaxBugs);
        var assimilate = new MoveState("ASSIMILATE", Assimilate, new SingleAttackIntent(AssimilateDamage));
        var swarm = new MoveState("SWARM", Swarm, new MultiAttackIntent(SwarmDamage, SwarmHits), new BuffIntent());
        build.FollowUpState = assimilate;
        reinforce.FollowUpState = assimilate;
        assimilate.FollowUpState = swarm;
        swarm.FollowUpState = branch;
        // She opens with Assimilate: two bugs are already out.
        return new MonsterMoveStateMachine([branch, build, reinforce, assimilate, swarm], assimilate);
    }

    // Loose blocks skitter back together into bugs.
    private async Task Build(IReadOnlyList<Creature> targets)
    {
        Boss?.Line(Creature, "build");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        int count = Math.Min(Math.Min(BuildPerTurn, Blocks), MaxBugs - BugsStanding);
        for (int i = 0; i < count; i++)
        {
            Blocks--;
            await Alexander.Summon(Creature, ModelDb.Monster<ReplicatorBug>(), order: ReplicatorBoss.Order);
        }
        Creature.GetPower<ReplicatorSwarmPower>()?.Refresh();
        await CreatureCmd.GainBlock(Creature, BuildBlock, ValueProp.Move, null);
    }

    private async Task Reinforce(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await CreatureCmd.GainBlock(Creature, ReinforceBlock, ValueProp.Move, null);
    }

    private async Task Assimilate(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(AssimilateDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    // The whole swarm surges: her bugs grow stronger, and she bites twice.
    private async Task Swarm(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        var ctx = new ThrowingPlayerChoiceContext();
        var bugs = Creature.CombatState?.Enemies.Where(e => e.IsAlive && e.Monster is ReplicatorBug).ToList() ?? [];
        if (bugs.Count > 0) await PowerCmd.Apply<StrengthPower>(ctx, bugs, SwarmStrength, Creature, null);
        await DamageCmd.Attack(SwarmDamage).WithHitCount(SwarmHits).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }
}

/// <summary>
/// A Replicator bug: a little spider of grey blocks. A minion. Bites, then Chews: eats up to 5 Block off every player and
/// keeps it as its own Block (they eat technology). Falls apart into blocks the Queen builds back up (ReplicatorSwarmPower).
/// </summary>
public class ReplicatorBug : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 16, 14);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 18, 16);
    private int BiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private int ChewAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    /// <summary>Set by the encounter so the starting pair doesn't do the same thing each turn.</summary>
    public bool ChewFirst { get; set; }

    protected override string ArtFolder => "replicator_bug";
    protected override float BodyHeight => 100f;
    protected override bool TipOver => false;   // its dead frames fall apart into blocks

    private const string BugSfx = "event:/sfx/enemy/enemy_attacks/two_tail_rats/two_tail_rats";
    public override string? CustomAttackSfx => BugSfx + "_attack_bite";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_minion_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<MinionPower>(ctx, Creature, 1, null, null);
        await PowerCmd.Apply<AdaptationPower>(ctx, Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var bite = new MoveState("BITE", Bite, new SingleAttackIntent(BiteDamage));
        var chew = new MoveState("CHEW", Chew, new DebuffIntent(), new DefendIntent());
        bite.FollowUpState = chew;
        chew.FollowUpState = bite;
        return new MonsterMoveStateMachine([bite, chew], ChewFirst ? chew : bite);
    }

    private async Task Bite(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BiteDamage).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    // They eat technology: a mouthful of each player's Block.
    private async Task Chew(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Attack", 0.3f);
        decimal eaten = 0;
        foreach (var player in Creature.CombatState?.Players.Select(p => p.Creature).Where(c => c.IsAlive && c.Block > 0).ToList() ?? [])
        {
            decimal bite = Math.Min(ChewAmount, player.Block);
            await CreatureCmd.LoseBlock(player, bite);
            eaten += bite;
        }
        if (eaten > 0) await CreatureCmd.GainBlock(Creature, eaten, ValueProp.Move, null);
    }
}
