using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
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
/// Goliath of Gath (1 Samuel 17), an Act 1 elite. Numbers sit with the vanilla Act 1 elites (Bygone Effigy 127 HP,
/// Terror Eel 140, big hits of 13-19). His bronze coat of mail is vanilla Plating, and his Shield Bearer stands in
/// front of him adding Block. ChampionOfGathPower + GoliathElite (a BibleElite) do the talking and hand David a Smooth Stone.
/// Moves: Choose Your Champion (once), then Weaver's Beam, Bronze Charge, Forty Days, and around again.
/// Body: images/monsters/goliath/frames/ (pixelgen's GoliathArt), mirrored to face the party.
/// </summary>
public class Goliath : CustomMonsterModel
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 128, 120);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 132, 124);

    private int ArmorAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 7, 6);
    private int SpearDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);
    private int ChargeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);
    private const int Charges = 2;
    private const int WeakAmount = 2;
    private int FortyDaysStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    /// <summary>
    /// How tall he stands in combat (the brothers are 400). He's a giant: six cubits and a span. His intent is held down to
    /// IntentHeight (over his crest) so it doesn't disappear behind the top bar.
    /// </summary>
    private const float BodyHeight = 540f, IntentHeight = 500f;

    private static string ArtPath(string file) => $"{MainFile.ResPath}/images/monsters/goliath/{file}";

    // Borrowed from the vanilla knights and the Ceremonial Beast (big, heavy, armored). Without these his hits are silent.
    private const string RamSfx = "event:/sfx/enemy/enemy_attacks/flail_knight/flail_knight_ram";
    private const string SpearSfx = "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_heavy_attack";
    public override string? CustomAttackSfx => RamSfx;
    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_die";

    // His body is built in code from frames, so there's no scene to preload; keep the intents' assets.
    public override IEnumerable<string> AssetPaths => base.AssetPaths.Skip(1);

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        try
        {
            if (PixelBody.LoadFrames(ArtPath, 5, out var first) is { } frames && first != null)
                return PixelBody.AnimatedBody(frames, first, BodyHeight, faceLeft: true, maxIntentHeight: IntentHeight);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't build Goliath's body: {e}");
        }
        return null;
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<PlatingPower>(ctx, Creature, ArmorAmount, Creature, null);
        await PowerCmd.Apply<ChampionOfGathPower>(ctx, Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var champion = new MoveState("CHOOSE_YOUR_CHAMPION", ChooseYourChampion, new DebuffIntent());
        var spear = new MoveState("WEAVERS_BEAM", WeaversBeam, new SingleAttackIntent(SpearDamage));
        var charge = new MoveState("BRONZE_CHARGE", BronzeCharge, new MultiAttackIntent(ChargeDamage, Charges));
        var fortyDays = new MoveState("FORTY_DAYS", FortyDays, new BuffIntent());
        champion.FollowUpState = spear;
        spear.FollowUpState = charge;
        charge.FollowUpState = fortyDays;
        fortyDays.FollowUpState = spear;
        return new MonsterMoveStateMachine([champion, spear, charge, fortyDays], champion);
    }

    // "Choose you a man for you, and let him come down to me." The whole army was dismayed.
    private async Task ChooseYourChampion(IReadOnlyList<Creature> targets)
    {
        SfxCmd.Play("event:/sfx/debuff");
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, WeakAmount, Creature, null);
    }

    // A spear whose shaft was like a weaver's beam.
    private async Task WeaversBeam(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(SpearDamage).FromMonster(this).WithAttackerAnim("Attack", 0.5f)
            .WithAttackerFx(null, SpearSfx)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(null);
    }

    private async Task BronzeCharge(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ChargeDamage).WithHitCount(Charges).FromMonster(this).WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    // He came out morning and evening for forty days to taunt them. Every time, a little louder.
    private async Task FortyDays(IReadOnlyList<Creature> targets)
    {
        (Creature.CombatState?.Encounter as BibleElite)?.Taunt(Creature);
        SfxCmd.Play(FmodSfx.fakeMerchantLaugh);
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.5f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, FortyDaysStrength, Creature, null);
    }
}
