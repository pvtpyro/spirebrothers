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
/// The Ender Dragon (Minecraft; they all play it): the mod's second Act 3 boss (the game rolls it or Anubis). She stands
/// behind three End Crystals (minions; one in an iron cage that renews its Block) that heal her every turn; breaking one
/// blasts her (TheEndPower, also the fight's FoePower). She circles (CirclingPower: Attacks deal half), then lands on the
/// portal (PerchedPower: Attacks deal 50% more, and the Bed token triple) for one player turn, breathes, and takes off.
/// The hero is a random player (the encounter's Rng, like Anubis): starts with the shared Bed token. The next player answers.
/// Lines: monsters.json SPIREBROTHERS-ENDER_DRAGON.*, plus the mid-fight pool perch (the party, when she lands).
/// The map icon is images/map/ender_dragon_boss_icon.png + _outline.png (mobgen).
/// </summary>
public class EnderDragonBoss() : BibleElite(RoomType.Boss)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 2;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<EndCrystal>(), ModelDb.Monster<EnderDragon>()];

    public const int Crystals = 3;

    private int _heroRoll;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        _heroRoll = Rng.NextInt(840);   // (divisible by every party size up to 8, so each player is as likely)
        var list = new List<(MonsterModel, string?)>();
        for (int i = 0; i < Crystals; i++)
        {
            var crystal = (EndCrystal)ModelDb.Monster<EndCrystal>().ToMutable();
            crystal.Caged = i == 1;   // the middle one
            list.Add((crystal, null));
        }
        list.Add((ModelDb.Monster<EnderDragon>().ToMutable(), null));
        return list;
    }

    public override float GetCameraScaling() => 0.8f;

    private const string IconPath = $"{MainFile.ResPath}/images/map/ender_dragon_boss_icon";
    public override string BossNodePath => IconPath;
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;
    public override string? CustomRunHistoryIconPath => IconPath + ".png";
    public override string? CustomRunHistoryIconOutlinePath => IconPath + "_outline.png";

    public override string LinesKey => "ENDER_DRAGON";

    /// <summary>0 for the hero, 1 for the one who answers, and so on round the party; -1 outside combat.</summary>
    private int Seat(Player p)
    {
        if (p.Creature.CombatState?.Players is not { Count: > 0 } players) return -1;
        int n = players.Count, i = players.ToList().IndexOf(p);
        return i < 0 ? -1 : ((i - _heroRoll) % n + n) % n;
    }

    protected override bool IsHero(Player p) => Seat(p) == 0;
    protected override bool IsSidekick(Player p) => Seat(p) == 1;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<Bed>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is EnderDragon;

    public Creature? PartyVoice(ICombatState combat) =>
        (Hero(combat) ?? Sidekick(combat) ?? combat.Players.FirstOrDefault(p => Banter.Talks(p) && p.Creature.IsAlive))?.Creature;
}

/// <summary>
/// The Ender Dragon. 300 HP. Starts Circling. Moves loop: Fireball (a big hit, and Dragon's Breath on everyone), Swoop
/// (three hits, then she lands: Perched), Breath (perched: more Dragon's Breath and a wing buffet, then she takes off).
/// So every third player turn she's Perched. Numbers sit with Anubis (500-600 HP all told counting the crystals' healing).
/// </summary>
public class EnderDragon : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 320, 300);
    public override int MaxInitialHp => MinInitialHp;

    private int FireballDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);
    private const int FireballBreath = 2;
    private int SwoopDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private const int SwoopHits = 3;
    private int BuffetDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);
    private int PerchBreath => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);

    protected override string ArtFolder => "ender_dragon";
    protected override float BodyHeight => 300f;
    protected override bool TipOver => false;   // her dead frames burst into light

    private const string RoarSfx = "event:/sfx/enemy/enemy_attacks/knowledge_demon/knowledge_demon_flame";
    public override string? CustomAttackSfx => "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_heavy_attack";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_knock_out";

    private EnderDragonBoss? Boss => Creature.CombatState?.Encounter as EnderDragonBoss;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<TheEndPower>(ctx, Creature, 1, null, null);
        await PowerCmd.Apply<CirclingPower>(ctx, Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var fireball = new MoveState("FIREBALL", Fireball, new SingleAttackIntent(FireballDamage), new DebuffIntent());
        var swoop = new MoveState("SWOOP", Swoop, new MultiAttackIntent(SwoopDamage, SwoopHits));
        var breath = new MoveState("BREATH", Breath, new SingleAttackIntent(BuffetDamage), new DebuffIntent());
        fireball.FollowUpState = swoop;
        swoop.FollowUpState = breath;
        breath.FollowUpState = fireball;
        return new MonsterMoveStateMachine([fireball, swoop, breath], fireball);
    }

    private IReadOnlyList<Creature> LivingPlayers =>
        Creature.CombatState?.Players.Select(p => p.Creature).Where(c => c.IsAlive).ToList() ?? [];

    private async Task Fireball(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        await DamageCmd.Attack(FireballDamage).FromMonster(this).WithAttackerAnim("Cast", 0.5f)
            .WithAttackerFx(null, RoarSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
        // The purple cloud spreads where the fireball lands.
        await PowerCmd.Apply<DragonsBreathPower>(new ThrowingPlayerChoiceContext(), LivingPlayers, FireballBreath, Creature, null);
    }

    // A pass over the party, then she lands on the portal.
    private async Task Swoop(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SwoopDamage).WithHitCount(SwoopHits).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await Land();
    }

    // Perched: a wing buffet and her breath on everyone, then she takes off.
    private async Task Breath(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        await DamageCmd.Attack(BuffetDamage).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
        SfxCmd.Play(RoarSfx);
        await PowerCmd.Apply<DragonsBreathPower>(new ThrowingPlayerChoiceContext(), LivingPlayers, PerchBreath, Creature, null);
        await TakeOff();
    }

    private async Task Land()
    {
        if (!Creature.IsAlive) return;
        var ctx = new ThrowingPlayerChoiceContext();
        if (Creature.GetPower<CirclingPower>() is { } circling) await PowerCmd.Remove(circling);
        if (Creature.GetPower<PerchedPower>() == null) await PowerCmd.Apply<PerchedPower>(ctx, Creature, 1, Creature, null);
        if (Creature.CombatState is { } combat && Boss?.PartyVoice(combat) is { } voice) Boss.Line(voice, "perch");
    }

    private async Task TakeOff()
    {
        if (!Creature.IsAlive) return;
        var ctx = new ThrowingPlayerChoiceContext();
        if (Creature.GetPower<PerchedPower>() is { } perched) await PowerCmd.Remove(perched);
        if (Creature.GetPower<CirclingPower>() == null) await PowerCmd.Apply<CirclingPower>(ctx, Creature, 1, Creature, null);
    }
}

/// <summary>
/// An End Crystal on top of an obsidian pillar. A minion that doesn't fight: every turn its beam heals the Ender Dragon.
/// The caged one (the middle crystal) also renews its iron bars' Block. When one breaks, it blasts the dragon (TheEndPower).
/// </summary>
public class EndCrystal : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 20, 18);
    public override int MaxInitialHp => MinInitialHp;
    private int HealAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private const int CageBlock = 12;

    /// <summary>Set by the encounter: this one sits in an iron cage.</summary>
    public bool Caged { get; set; }

    protected override string ArtFolder => Caged ? "end_crystal_caged" : "end_crystal";
    protected override float BodyHeight => 280f;
    protected override bool TipOver => false;   // its dead frames explode

    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_explode";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
        if (Caged) await CreatureCmd.GainBlock(Creature, CageBlock, ValueProp.Move, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var heal = Caged
            ? new MoveState("HEAL", Heal, new HealIntent(), new DefendIntent())
            : new MoveState("HEAL", Heal, new HealIntent());
        heal.FollowUpState = heal;
        return new MonsterMoveStateMachine([heal], heal);
    }

    private async Task Heal(IReadOnlyList<Creature> targets)
    {
        var dragon = Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is EnderDragon);
        if (dragon != null && dragon.CurrentHp < dragon.MaxHp)
        {
            await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
            await CreatureCmd.Heal(dragon, HealAmount);
        }
        if (Caged) await CreatureCmd.GainBlock(Creature, CageBlock, ValueProp.Move, null);
    }
}
