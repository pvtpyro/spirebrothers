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
using SpireBrothers.SpireBrothersCode.Cards.Daniel;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Apophis, System Lord (Stargate SG-1): the mod's first boss, Act 2. Two Jaffa stand in front of him, the Sarcophagus
/// behind. Daniel (Daniel Jackson's namesake) starts with Shol'va and gets the opening line; David answers (sideReply).
/// Lines: monsters.json SPIREBROTHERS-APOPHIS.*, plus the mid-fight pools kara, interrupted, carried, rise, sarcBroken.
/// The map icon is images/map/apophis_boss_icon.png + _outline.png (white; the map tints it like vanilla's placeholders).
/// </summary>
public class ApophisBoss() : BibleElite(RoomType.Boss)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<Jaffa>(), ModelDb.Monster<Apophis>(), ModelDb.Monster<Sarcophagus>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        var first = (Jaffa)ModelDb.Monster<Jaffa>().ToMutable();
        var second = (Jaffa)ModelDb.Monster<Jaffa>().ToMutable();
        second.KreeFirst = true;
        return [(first, null), (second, null), (ModelDb.Monster<Apophis>().ToMutable(), null), (ModelDb.Monster<Sarcophagus>().ToMutable(), null)];
    }

    // Four bodies, one of them big: zoom out a little more than Jericho's three.
    public override float GetCameraScaling() => 0.8f;

    private const string IconPath = $"{MainFile.ResPath}/images/map/apophis_boss_icon";
    public override string BossNodePath => IconPath;
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;
    public override string? CustomRunHistoryIconPath => IconPath + ".png";
    public override string? CustomRunHistoryIconOutlinePath => IconPath + "_outline.png";

    public override string LinesKey => "APOPHIS";
    protected override bool IsHero(Player p) => p.Character is Character.Daniel;
    protected override bool IsSidekick(Player p) => p.Character is Character.David;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<SholVa>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is Apophis;

    /// <summary>Front to back: the Jaffa, Apophis, the Sarcophagus (for summons mid-fight).</summary>
    public static int Order(Creature c) => c.Monster switch { Jaffa => 0, Apophis => 1, _ => 2 };

    /// <summary>Who on the party's side says the fight's lines: Daniel, else David, else anyone who talks.</summary>
    public Creature? PartyVoice(ICombatState combat) =>
        (Hero(combat) ?? Sidekick(combat) ?? combat.Players.FirstOrDefault(p => Banter.Talks(p) && p.Creature.IsAlive))?.Creature;

    public override void AfterDeath(Creature creature)
    {
        var combat = creature.CombatState;
        if (combat == null) return;
        var apophis = combat.Enemies.FirstOrDefault(e => e.Monster is Apophis);
        if (creature.Monster is Sarcophagus)
        {
            // Broken while he lies in it: that's the win. Broken before he fell: a cheer. Spent by his rising: nothing.
            if (apophis is { IsDead: true }) HeroWins(combat);
            else if (apophis?.Monster is Apophis { Reborn: false } && PartyVoice(combat) is { } voice) Line(voice, "sarcBroken");
            return;
        }
        if (creature.Monster is Apophis && creature.GetPower<SarcophagusPower>() is { WillRise: true })
        {
            Line(creature, "carried");
            return;
        }
        base.AfterDeath(creature);
    }
}

/// <summary>
/// Apophis. Phase 1 (200 HP): Kneel Before Your God! (once), Staff Blast, Raise the Kara Kesh, Ribbon Device, Jaffa
/// Kree!, then Staff Blast again. His Personal Shield caps every hit at 12. Raising the Kara Kesh gives him KaraKeshPower:
/// land 30 damage before it fires and he's stunned out of it. Falling with the Sarcophagus standing, he's carried in
/// (SarcophagusPower) and Rises next turn at 180 HP with Strength, and phase 2 swaps Staff Blast for I Am Your God! and
/// Ha'tak Bombardment. Numbers sit with the vanilla Act 2 bosses (320-420 HP all told, big hits of 18-31).
/// </summary>
public class Apophis : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 215, 200);
    public override int MaxInitialHp => MinInitialHp;
    private int RebornHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 195, 180);
    private int ShieldCap => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 10, 12);
    private const int KaraKeshThreshold = 30;

    private int StaffDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);
    private int RibbonDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 34, 30);
    private int GodDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);
    private const int GodHits = 3;
    private int HatakDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 12);
    private const int HatakVulnerable = 1;
    private const int KneelWeak = 2;
    private int RebornStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    public const int JaffaRanks = 2;

    /// <summary>True after he rises from the Sarcophagus (phase 2).</summary>
    public bool Reborn { get; private set; }

    protected override string ArtFolder => "apophis";
    protected override float BodyHeight => 400f;
    protected override bool TipOver => true;

    private const string BeamSfx = "event:/sfx/enemy/enemy_attacks/knowledge_demon/knowledge_demon_flame";
    private const string StaffSfx = "event:/sfx/enemy/enemy_attacks/crossbow_ruby_raider/crossbow_ruby_raider";
    public override string? CustomAttackSfx => StaffSfx;
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_knock_out";

    private MoveState _rise = null!;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<PersonalShieldPower>(ctx, Creature, ShieldCap, Creature, null);
        await PowerCmd.Apply<SarcophagusPower>(ctx, Creature, 1, Creature, null);
        await PowerCmd.Apply<SystemLordPower>(ctx, Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var kneel = new MoveState("KNEEL", Kneel, new DebuffIntent());
        var staff = new MoveState("STAFF_BLAST", StaffBlast, new SingleAttackIntent(StaffDamage));
        var raise = new MoveState("RAISE_KARA_KESH", RaiseKaraKesh, new BuffIntent());
        var ribbon = new MoveState("RIBBON_DEVICE", RibbonDevice, new SingleAttackIntent(RibbonDamage));
        var kree = new MoveState("JAFFA_KREE", JaffaKree, new SummonIntent());
        var god = new MoveState("I_AM_YOUR_GOD", IAmYourGod, new MultiAttackIntent(GodDamage, GodHits));
        var hatak = new MoveState("HATAK_BOMBARDMENT", HatakBombardment, new SingleAttackIntent(HatakDamage), new DebuffIntent());
        var phase = new ConditionalBranchState("PHASE");
        _rise = new MoveState("RISE", Rise, new HealIntent(), new BuffIntent()) { MustPerformOnceBeforeTransitioning = true };

        kneel.FollowUpState = staff;
        staff.FollowUpState = raise;
        raise.FollowUpState = ribbon;
        ribbon.FollowUpState = kree;
        kree.FollowUpState = phase;
        phase.AddState(staff, () => !Reborn);
        phase.AddState(god, () => Reborn);
        god.FollowUpState = hatak;
        hatak.FollowUpState = raise;
        _rise.FollowUpState = god;
        return new MonsterMoveStateMachine([kneel, staff, raise, ribbon, kree, god, hatak, phase, _rise], kneel);
    }

    private ApophisBoss? Boss => Creature.CombatState?.Encounter as ApophisBoss;

    // "Kneel before your god!"
    private async Task Kneel(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        SfxCmd.Play("event:/sfx/debuff");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, KneelWeak, Creature, null);
    }

    private async Task StaffBlast(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(StaffDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithAttackerFx(null, StaffSfx + "_attack")
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    // The hand device glows. The party has until his next turn to knock him out of it.
    private async Task RaiseKaraKesh(IReadOnlyList<Creature> targets)
    {
        Boss?.Line(Creature, "kara");
        SfxCmd.Play("event:/sfx/buff");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<KaraKeshPower>(new ThrowingPlayerChoiceContext(), Creature, KaraKeshThreshold, Creature, null);
    }

    private async Task RibbonDevice(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Remove<KaraKeshPower>(Creature);
        await DamageCmd.Attack(RibbonDamage).FromMonster(this).WithAttackerAnim("Cast", 0.5f)
            .WithAttackerFx(null, BeamSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    /// <summary>KaraKeshPower ran out: the beam is cancelled, he's stunned, then calls his Jaffa.</summary>
    public async Task Interrupt()
    {
        if (Creature.CombatState is { } combat && Boss?.PartyVoice(combat) is { } voice) Boss.Line(voice, "interrupted");
        await CreatureCmd.Stun(Creature, "JAFFA_KREE");
    }

    // "Jaffa, kree!" Fills the ranks back up to two. No Block of his own: his cover is the Jaffa (it stacked with theirs).
    private async Task JaffaKree(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        int missing = JaffaRanks - (Creature.CombatState?.Enemies.Count(e => e.IsAlive && e.Monster is Jaffa) ?? JaffaRanks);
        for (int i = 0; i < missing; i++) await Alexander.Summon(Creature, ModelDb.Monster<Jaffa>(), order: ApophisBoss.Order);
    }

    private async Task IAmYourGod(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(GodDamage).WithHitCount(GodHits).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, StaffSfx + "_attack")
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    // His mothership in orbit opens fire.
    private async Task HatakBombardment(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(HatakDamage).FromMonster(this).WithAttackerAnim("Cast", 0.5f)
            .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/kaiser_crab/kaiser_crab_rocket")
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, HatakVulnerable, Creature, null);
    }

    /// <summary>SarcophagusPower: he fell with the Sarcophagus standing. Next turn he rises.</summary>
    public Task CarriedIn()
    {
        SetMoveImmediate(_rise, forceTransition: true);
        return Task.CompletedTask;
    }

    private async Task Rise(IReadOnlyList<Creature> targets)
    {
        var combat = Creature.CombatState;
        var sarcophagus = combat?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Sarcophagus);
        if (combat == null || sarcophagus == null) return;   // broken while he lay in it: the fight is already over
        Creature.GetPower<SarcophagusPower>()?.Risen();
        Reborn = true;
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/test_subject/test_subject_revive_two_heads");
        decimal hp = Creature.ScaleHpForMultiplayer(RebornHp, combat.Encounter, combat.Players.Count, combat.RunState.CurrentActIndex);
        await CreatureCmd.SetMaxHp(Creature, hp);
        await CreatureCmd.Heal(Creature, hp);
        await PowerCmd.Remove<SarcophagusPower>(Creature);
        await PowerCmd.Remove<KaraKeshPower>(Creature);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, RebornStrength, Creature, null);
        Boss?.Line(Creature, "rise");
        // The Sarcophagus is spent.
        await CreatureCmd.Kill(sarcophagus);
    }
}
