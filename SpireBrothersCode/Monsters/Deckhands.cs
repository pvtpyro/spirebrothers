using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// Blockbeard's Deckhands (Monkey Island), an easy Act 1 fight: a Lookout and a Powder Monkey, about 23 HP between them.
/// Both start Insulted (they've been at sea with Blockbeard), so Comeback cards hit hard here.
/// </summary>
public class DeckhandsWeak() : CustomEncounterModel(RoomType.Monster)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;
    public override bool IsWeak => true;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Lookout>(), ModelDb.Monster<PowderMonkey>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<Lookout>().ToMutable(), null), (ModelDb.Monster<PowderMonkey>().ToMutable(), null)];
}

/// <summary>Shared by both deckhands: they start Insulted.</summary>
public abstract class Deckhand : PixelMonster
{
    public const int StartingInsulted = 2;
    protected override float BodyHeight => 280f;
    protected override bool TipOver => true;

    private const string RaiderSfx = "event:/sfx/enemy/enemy_attacks/axe_ruby_raider/axe_ruby_raider";
    public override string? CustomAttackSfx => RaiderSfx + "_attack";
    public override string? CustomDeathSfx => RaiderSfx + "_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<InsultedPower>(new ThrowingPlayerChoiceContext(), Creature, StartingInsulted, null, null);
    }
}

/// <summary>Shouts an insult (Weak, with a speech bubble), then jabs with his spyglass, and repeats.</summary>
public class Lookout : Deckhand
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 13, 12);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 14, 13);
    private int JabDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
    private const int InsultWeak = 1;
    /// <summary>Lines are monsters.json SPIREBROTHERS-LOOKOUT.insults.0..N-1.</summary>
    private const int InsultLines = 6;

    protected override string ArtFolder => "lookout";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var insult = new MoveState("INSULT", Insult, new DebuffIntent());
        var jab = new MoveState("JAB", Jab, new SingleAttackIntent(JabDamage));
        insult.FollowUpState = jab;
        jab.FollowUpState = insult;
        return new MonsterMoveStateMachine([insult, jab], insult);
    }

    private async Task Insult(IReadOnlyList<Creature> targets)
    {
        // The line follows the round (never the run RNG), so every co-op screen shows the same one.
        int round = Creature.CombatState?.RoundNumber ?? 0;
        Banter.Bubble(Creature, new LocString("monsters", $"{Id.Entry}.insults.{round % InsultLines}").GetFormattedText());
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, InsultWeak, Creature, null);
    }

    private async Task Jab(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(JabDamage).FromMonster(this).WithAttackerAnim("Attack", 0.35f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(null);
    }
}

/// <summary>Lights a keg, then throws it the next turn. Kill him in between and the keg never comes.</summary>
public class PowderMonkey : Deckhand
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 11, 10);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 12, 11);
    private int KegDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 12);

    protected override string ArtFolder => "powder_monkey";
    protected override float BodyHeight => 250f;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var light = new MoveState("LIGHT_FUSE", LightFuse, new UnknownIntent());
        var toss = new MoveState("THROW_KEG", ThrowKeg, new SingleAttackIntent(KegDamage));
        light.FollowUpState = toss;
        toss.FollowUpState = light;
        return new MonsterMoveStateMachine([light, toss], light);
    }

    private async Task LightFuse(IReadOnlyList<Creature> targets) => await CreatureCmd.TriggerAnim(Creature, "Cast", 0.6f);

    private async Task ThrowKeg(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(KegDamage).FromMonster(this).WithAttackerAnim("Attack", 0.45f)
            .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/living_fog/living_fog_explode")
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }
}
