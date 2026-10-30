using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.David;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Ba'al and his clones (Stargate SG-1): the mod's silly Act 1 boss. Three identical Ba'als, one of them real (picked
/// from the encounter's Rng, the same on every co-op client). Every one carries WhichOneIsRealPower, which looks the
/// same on all of them: it marks the clones as secondary enemies (so the real one falling ends the fight, and the game
/// kills the rest), reveals a Ba'al at half HP, sends a fresh clone in when one falls (3 in reserve), and hands David
/// Due Diligence. David opens, Daniel replies.
/// Lines: monsters.json SPIREBROTHERS-BAAL.*, plus the mid-fight pools found, notMe, arrive, takeover.
/// The map icon is images/map/baal_boss_icon.png + _outline.png (mobgen).
/// </summary>
public class BaalBoss() : BibleElite(RoomType.Boss)
{
    public const int StartingBaals = 3;

    public override bool IsValidForAct(ActModel act) => act.Index == 0;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Baal>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        int real = Rng.NextInt(StartingBaals);
        return Enumerable.Range(0, StartingBaals).Select(i =>
        {
            var baal = (Baal)ModelDb.Monster<Baal>().ToMutable();
            baal.StartStep = i;
            baal.IsReal = i == real;
            return ((MonsterModel)baal, (string?)null);
        }).ToList();
    }

    public override float GetCameraScaling() => 0.85f;

    private const string IconPath = $"{MainFile.ResPath}/images/map/baal_boss_icon";
    public override string BossNodePath => IconPath;
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;
    public override string? CustomRunHistoryIconPath => IconPath + ".png";
    public override string? CustomRunHistoryIconOutlinePath => IconPath + "_outline.png";

    public override string LinesKey => "BAAL";
    protected override bool IsHero(Player p) => p.Character is Character.David;
    protected override bool IsSidekick(Player p) => p.Character is Character.Daniel;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<DueDiligence>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is Baal { IsReal: true };

    // Any of them may talk (the real one doing all the talking would give him away): the first one standing.
    public override Creature? Speaker(ICombatState combat) => combat.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Baal);

    private int _quipRound = -1;

    /// <summary>A line, but only one Ba'al gets to talk per round (three of them taking turns would bury the screen).</summary>
    public void Quip(Creature speaker, string pool)
    {
        int round = speaker.CombatState?.RoundNumber ?? 0;
        if (round == _quipRound) return;
        _quipRound = round;
        Line(speaker, pool);
    }
}

/// <summary>
/// One Ba'al (real or clone; nothing on the outside tells them apart until half HP). Moves cycle Zat Blast, Hostile
/// Takeover (a small hit that steals Gold from every player; this Ba'al gives it back when he falls), Smirk (Block, and
/// Weak on everyone), each starting at a different point. The real one also holds the reserve of clones.
/// </summary>
public class Baal : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 74, 70);
    public override int MaxInitialHp => MinInitialHp;
    private int ZatDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 8);
    private int TakeoverDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int TakeoverGold = 10;
    private const int SmirkBlock = 6, SmirkWeak = 1;
    public int FoundStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    /// <summary>Set by the encounter.</summary>
    public bool IsReal { get; set; }
    public int StartStep { get; set; }

    /// <summary>Clones still waiting offstage (only the real one's count is used).</summary>
    public int ClonesLeft { get; set; } = 3;

    // Who he's stolen from and how much, so he can give it back. Made on first use: the canonical model never steals,
    // so each live Ba'al gets its own.
    private Dictionary<Player, int>? _stolen;

    protected override string ArtFolder => "baal";
    protected override float BodyHeight => 300f;
    protected override bool TipOver => true;

    private const string ZatSfx = "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider";
    public override string? CustomAttackSfx => ZatSfx + "_attack";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider_die";

    private BaalBoss? Boss => Creature.CombatState?.Encounter as BaalBoss;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<WhichOneIsRealPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var zat = new MoveState("ZAT_BLAST", ZatBlast, new SingleAttackIntent(ZatDamage));
        var takeover = new MoveState("HOSTILE_TAKEOVER", HostileTakeover, new SingleAttackIntent(TakeoverDamage), new DebuffIntent());
        var smirk = new MoveState("SMIRK", Smirk, new DefendIntent(), new DebuffIntent());
        zat.FollowUpState = takeover;
        takeover.FollowUpState = smirk;
        smirk.FollowUpState = zat;
        var start = (StartStep % 3) switch { 0 => zat, 1 => takeover, _ => smirk };
        return new MonsterMoveStateMachine([zat, takeover, smirk], start);
    }

    private async Task ZatBlast(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ZatDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(null);
    }

    // Ba'al Industries is acquiring your Gold.
    private async Task HostileTakeover(IReadOnlyList<Creature> targets)
    {
        Boss?.Quip(Creature, "takeover");
        await DamageCmd.Attack(TakeoverDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(null);
        foreach (var target in targets)
        {
            if (target.Player is not { } player || target.IsDead) continue;
            int amount = Math.Min(TakeoverGold, player.Gold);
            if (amount <= 0) continue;
            await PlayerCmd.LoseGold(amount, player, GoldLossType.Stolen);
            _stolen ??= [];
            _stolen[player] = _stolen.GetValueOrDefault(player) + amount;
        }
    }

    private async Task Smirk(IReadOnlyList<Creature> targets)
    {
        Boss?.Quip(Creature, "taunts");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.GainBlock(Creature, SmirkBlock, ValueProp.Move, null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, SmirkWeak, Creature, null);
    }

    /// <summary>He fell: everything he took goes back to its owner.</summary>
    public async Task ReturnGold()
    {
        if (_stolen == null) return;
        foreach (var (player, amount) in _stolen.ToList())
            if (amount > 0) await PlayerCmd.GainGold(amount, player, wasStolenBack: true);
        _stolen.Clear();
    }
}
