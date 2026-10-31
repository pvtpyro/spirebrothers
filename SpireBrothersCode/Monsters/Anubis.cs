using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
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
/// Anubis (Stargate SG-1): the mod's Act 3 boss. Two Kull Warriors stand in front of him. He's half-ascended
/// (HalfAscendedPower): falling while a Kull stands, his shroud takes that body instead, so the Kull have to go first.
/// The hero isn't tied to a character: one player, picked at random (the encounter's Rng, the same on every co-op
/// client), starts with the Anti-Kull Disruptor and gets the opening line; the next player in the party answers.
/// Lines: monsters.json SPIREBROTHERS-ANUBIS.*, plus the mid-fight pools possess, superweapon, noHosts.
/// The map icon is images/map/anubis_boss_icon.png + _outline.png (mobgen).
/// </summary>
public class AnubisBoss() : BibleElite(RoomType.Boss)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 2;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<KullWarrior>(), ModelDb.Monster<Anubis>()];

    // Who leads: the player this far round from the first one (taken modulo the party size).
    private int _heroRoll;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        _heroRoll = Rng.NextInt(840);   // (divisible by every party size up to 8, so each player is as likely)
        var first = (KullWarrior)ModelDb.Monster<KullWarrior>().ToMutable();
        var second = (KullWarrior)ModelDb.Monster<KullWarrior>().ToMutable();
        second.MarchFirst = true;
        return [(first, null), (second, null), (ModelDb.Monster<Anubis>().ToMutable(), null)];
    }

    public override float GetCameraScaling() => 0.85f;

    private const string IconPath = $"{MainFile.ResPath}/images/map/anubis_boss_icon";
    public override string BossNodePath => IconPath;
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;
    public override string? CustomRunHistoryIconPath => IconPath + ".png";
    public override string? CustomRunHistoryIconOutlinePath => IconPath + "_outline.png";

    public override string LinesKey => "ANUBIS";

    /// <summary>0 for the hero, 1 for the one who answers, and so on round the party; -1 outside combat.</summary>
    private int Seat(Player p)
    {
        if (p.Creature.CombatState?.Players is not { Count: > 0 } players) return -1;
        int n = players.Count, i = players.ToList().IndexOf(p);
        return i < 0 ? -1 : ((i - _heroRoll) % n + n) % n;
    }

    protected override bool IsHero(Player p) => Seat(p) == 0;
    protected override bool IsSidekick(Player p) => Seat(p) == 1;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<AntiKullDisruptor>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is Anubis;

    /// <summary>Front to back: the Kull, then Anubis (for summons mid-fight).</summary>
    public static int Order(Creature c) => c.Monster is KullWarrior ? 0 : 1;

    /// <summary>Who on the party's side says the fight's lines: the hero, else the one who answers, else anyone who talks.</summary>
    public Creature? PartyVoice(ICombatState combat) =>
        (Hero(combat) ?? Sidekick(combat) ?? combat.Players.FirstOrDefault(p => Banter.Talks(p) && p.Creature.IsAlive))?.Creature;

    public override void AfterDeath(Creature creature)
    {
        var combat = creature.CombatState;
        if (combat == null) return;
        if (creature.Monster is KullWarrior)
        {
            // The last body he could take is gone: someone says so.
            var anubis = combat.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Anubis);
            if (anubis != null && !combat.Enemies.Any(e => e != creature && e.IsAlive && e.Monster is KullWarrior)
                && anubis.Monster is Anubis { Possessing: false } && PartyVoice(combat) is { } voice)
                Line(voice, "noHosts");
            return;
        }
        // Fallen with a Kull standing: that's no death, he's about to take it (HalfAscendedPower).
        if (creature.Monster is Anubis && creature.GetPower<HalfAscendedPower>()?.Host != null) return;
        base.AfterDeath(creature);
    }
}

/// <summary>
/// Anubis. 250 HP. Moves: I Am Anubis (Frail on everyone, once), then Shroud Strike, Kull, Rise! (fills the Kull back up
/// to two from a reserve of 2 for the whole fight, and Block; once the reserve is spent, Dark Shroud: Block and Weak on
/// everyone), Touch of Death (three hits), and the Superweapon (a big hit), and around again from Shroud Strike.
/// SuperweaponPower counts down to it. Falling with a Kull standing, he takes it over (Possess): it drops, he gets up with
/// 100 HP, Strength, and no debuffs. Numbers sit with the vanilla Act 3 bosses (500-600 HP all told, big hits of 20-30).
/// </summary>
public class Anubis : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 265, 250);
    public override int MaxInitialHp => MinInitialHp;
    private int PossessHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 110, 100);

    private int StrikeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 23, 20);
    private int TouchDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
    private const int TouchHits = 3;
    private int SuperweaponDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 35, 30);
    private int PossessStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);
    private const int OpeningFrail = 2;
    private const int RiseBlock = 12;
    public const int KullRanks = 2;
    /// <summary>Kull he can call in over the whole fight (KullReservePower), so the bodies run out.</summary>
    private const int KullReserve = 2;
    private int ShroudBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 15);
    private const int ShroudWeak = 1;

    /// <summary>His turns until the superweapon fires, at the start of the fight (it's his fifth move).</summary>
    private const int FirstCountdown = 5;
    /// <summary>Its countdown again after it fires (four moves around the loop).</summary>
    private const int Countdown = 4;

    /// <summary>True while he's taking a Kull over (the Kull's death then isn't "his last body is gone").</summary>
    public bool Possessing { get; private set; }

    protected override string ArtFolder => "anubis";
    protected override float BodyHeight => 380f;
    protected override bool TipOver => true;

    private const string BeamSfx = "event:/sfx/enemy/enemy_attacks/knowledge_demon/knowledge_demon_flame";
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_knock_out";

    private AnubisBoss? Boss => Creature.CombatState?.Encounter as AnubisBoss;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<HalfAscendedPower>(ctx, Creature, 1, null, null);
        await PowerCmd.Apply<SuperweaponPower>(ctx, Creature, FirstCountdown, Creature, null);
        await PowerCmd.Apply<KullReservePower>(ctx, Creature, KullReserve, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var iAm = new MoveState("I_AM_ANUBIS", IAmAnubis, new DebuffIntent());
        var strike = new MoveState("SHROUD_STRIKE", ShroudStrike, new SingleAttackIntent(StrikeDamage));
        var rise = new MoveState("KULL_RISE", KullRise, new SummonIntent(), new DefendIntent());
        var shroud = new MoveState("DARK_SHROUD", DarkShroud, new DefendIntent(), new DebuffIntent());
        var reserve = new ConditionalBranchState("RESERVE");
        reserve.AddState(rise, () => Reserve > 0);
        reserve.AddState(shroud, () => Reserve <= 0);
        var touch = new MoveState("TOUCH_OF_DEATH", TouchOfDeath, new MultiAttackIntent(TouchDamage, TouchHits));
        var superweapon = new MoveState("SUPERWEAPON", Superweapon, new SingleAttackIntent(SuperweaponDamage));
        iAm.FollowUpState = strike;
        strike.FollowUpState = reserve;
        rise.FollowUpState = touch;
        shroud.FollowUpState = touch;
        touch.FollowUpState = superweapon;
        superweapon.FollowUpState = strike;
        return new MonsterMoveStateMachine([iAm, strike, reserve, rise, shroud, touch, superweapon], iAm);
    }

    /// <summary>Sets the superweapon's countdown (his turns until it fires).</summary>
    private async Task CountDown(int turns)
    {
        if (Creature.GetPower<SuperweaponPower>() is not { } power || power.Amount == turns) return;
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), power, turns - power.Amount, null, null);
    }

    // "I am Anubis." Your gods have abandoned you.
    private async Task IAmAnubis(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        SfxCmd.Play("event:/sfx/debuff");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), targets, OpeningFrail, Creature, null);
        await CountDown(4);
    }

    private async Task ShroudStrike(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(StrikeDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
        await CountDown(3);
    }

    /// <summary>Kull left in his reserve (KullReservePower; gone at 0).</summary>
    private int Reserve => Creature.GetPower<KullReservePower>()?.Amount ?? 0;

    // "Kull, rise!" Fills the ranks back up to two, as far as the reserve goes.
    private async Task KullRise(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        int standing = Creature.CombatState?.Enemies.Count(e => e.IsAlive && e.Monster is KullWarrior) ?? KullRanks;
        int called = Math.Min(KullRanks - standing, Reserve);
        for (int i = 0; i < called; i++) await Alexander.Summon(Creature, ModelDb.Monster<KullWarrior>(), order: AnubisBoss.Order);
        if (called > 0 && Creature.GetPower<KullReservePower>() is { } power)
        {
            if (power.Amount <= called) await PowerCmd.Remove(power);
            else await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), power, -called, null, null);
        }
        await CreatureCmd.GainBlock(Creature, RiseBlock, ValueProp.Move, null);
        await CountDown(2);
    }

    // No Kull left to call: he wraps himself in darkness instead.
    private async Task DarkShroud(IReadOnlyList<Creature> targets)
    {
        Boss?.Taunt(Creature);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await CreatureCmd.GainBlock(Creature, ShroudBlock, ValueProp.Move, null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, ShroudWeak, Creature, null);
        await CountDown(2);
    }

    private async Task TouchOfDeath(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(TouchDamage).WithHitCount(TouchHits).FromMonster(this).WithAttackerAnim("Attack", 0.3f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await CountDown(1);
    }

    // His mothership fires the superweapon from orbit.
    private async Task Superweapon(IReadOnlyList<Creature> targets)
    {
        Boss?.Line(Creature, "superweapon");
        await DamageCmd.Attack(SuperweaponDamage).FromMonster(this).WithAttackerAnim("Cast", 0.6f)
            .WithAttackerFx(null, BeamSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
        await CountDown(Countdown);
    }

    /// <summary>
    /// HalfAscendedPower stopped his death: his shroud takes the Kull. He gets up first (he's at 0 HP, and the Kull
    /// falling with him still down would end the fight), sheds his debuffs, grows stronger, and the Kull drops.
    /// </summary>
    public async Task Possess(Creature host)
    {
        var combat = Creature.CombatState;
        if (combat == null) return;
        Possessing = true;
        try
        {
            Boss?.Line(Creature, "possess");
            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/test_subject/test_subject_revive_two_heads");
            decimal hp = Creature.ScaleHpForMultiplayer(PossessHp, combat.Encounter, combat.Players.Count, combat.RunState.CurrentActIndex);
            await CreatureCmd.Heal(Creature, Math.Min(hp, Creature.MaxHp));
            var ctx = new ThrowingPlayerChoiceContext();
            foreach (var debuff in Creature.Powers.Where(p => p.Type == PowerType.Debuff).ToList())
                await PowerCmd.Remove(debuff);
            await PowerCmd.Apply<StrengthPower>(ctx, Creature, PossessStrength, Creature, null);
            await CreatureCmd.Kill(host);
        }
        finally
        {
            Possessing = false;
        }
        // That was the last body he could take.
        if (!combat.Enemies.Any(e => e.IsAlive && e.Monster is KullWarrior) && Boss?.PartyVoice(combat) is { } voice)
            Boss.Line(voice, "noHosts");
    }
}
