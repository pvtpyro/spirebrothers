using System.Text.RegularExpressions;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Powers;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>The bonuses Spire Brothers give each other when they share a party. Loc keys use the name in UPPER_SNAKE.</summary>
public enum Bond
{
    HeatedDebate,      // Daniel + David
    WiredForSound,     // Daniel + Joshua
    CodeReview,        // Daniel + Tim
    Royalties,         // David + Joshua
    FamilyLoan,        // David + Tim
    KidsChoir,         // Joshua + Tim
    GroupChat,         // any 3 brothers
    NobodyLeftBehind,  // all 4
    // The friends (Honorary Brothers).
    UncleAndNephew,    // John + Huffman
    BestFriends,       // John + David
    Coworkers,         // Huffman + Daniel
    StrikeTeam,        // Daniel + John + Huffman (Darktide)
    // Brandi (Daniel's wife).
    HusbandAndWife,    // Brandi + Daniel
    InLaws,            // Brandi + any of David, Joshua, Tim
}

/// <summary>
/// Family Bonds: each pair of brothers in the party unlocks a bonus that links their mechanics, with extras for 3
/// and 4 brothers. Every brother gets the FamilyBondsPower icon on turn 1, which lists what's active. The effects run
/// here and in the hooks they ride on (Verses.SpendForChorus, ScriptPower.AfterScriptRan, AgeUp's cost,
/// FamilyBondsPower for death saves and after-combat gold).
/// </summary>
public class FamilyBonds() : CustomSingletonModel(HookType.Combat)
{
    public const int HeatedDebateInsulted = 1;
    public const int WiredForSoundVerses = 1;
    public const int RoyaltiesGold = 5;
    public const int FamilyLoanDiscount = 25;
    public const int FamilyLoanGold = 10;
    public const int GroupChatEnergy = 1;
    public const int NobodyLeftBehindHeal = 10;
    public const int UncleAndNephewToxin = 2;
    public const int CoworkersMetal = 2;
    public const int StrikeTeamBlock = 3;
    public const int HusbandAndWifeStatic = 2;
    public const int InLawsAim = 2;

    // The round each once-per-turn bond last fired, per player. Rounds instead of a reset flag, so it doesn't matter
    // whether Script replays run before or after the turn-start reset.
    private static readonly SpireField<PlayerCombatState, int> WiredForSoundRound = new(() => -1);
    private static readonly SpireField<PlayerCombatState, int> CodeReviewRound = new(() => -1);
    private static readonly SpireField<PlayerCombatState, int> CoworkersRound = new(() => -1);
    private static readonly SpireField<PlayerCombatState, int> BestFriendsRound = new(() => -1);
    private static readonly SpireField<PlayerCombatState, int> HusbandAndWifeRound = new(() => -1);

    private static readonly (Bond Bond, Func<Player, bool> A, Func<Player, bool> B)[] Pairs =
    [
        (Bond.HeatedDebate, p => p.Character is Daniel, p => p.Character is David),
        (Bond.WiredForSound, p => p.Character is Daniel, p => p.Character is Joshua),
        (Bond.CodeReview, p => p.Character is Daniel, p => p.Character is Tim),
        (Bond.Royalties, p => p.Character is David, p => p.Character is Joshua),
        (Bond.FamilyLoan, p => p.Character is David, p => p.Character is Tim),
        (Bond.KidsChoir, p => p.Character is Joshua, p => p.Character is Tim),
        (Bond.UncleAndNephew, p => p.Character is John, p => p.Character is Huffman),
        (Bond.BestFriends, p => p.Character is John, p => p.Character is David),
        (Bond.Coworkers, p => p.Character is Huffman, p => p.Character is Daniel),
        (Bond.HusbandAndWife, p => p.Character is Brandi, p => p.Character is Daniel),
        (Bond.InLaws, p => p.Character is Brandi, IsInLaw),
    ];

    private static bool IsInLaw(Player p) => p.Character is David or Joshua or Tim;

    /// <summary>Different brothers in the party (two Daniels count once, and only form a bond with someone else).</summary>
    public static int DistinctBrothers(IEnumerable<Player> players) =>
        players.Where(BrothersInArms.IsBrother).Select(p => p.Character.GetType()).Distinct().Count();

    public static List<Bond> Active(IEnumerable<Player>? players)
    {
        var list = players?.ToList() ?? [];
        var bonds = Pairs.Where(x => list.Any(x.A) && list.Any(x.B)).Select(x => x.Bond).ToList();
        int brothers = DistinctBrothers(list);
        if (brothers >= 3) bonds.Add(Bond.GroupChat);
        if (brothers >= 4) bonds.Add(Bond.NobodyLeftBehind);
        if (list.Any(p => p.Character is Daniel) && list.Any(p => p.Character is John) && list.Any(p => p.Character is Huffman))
            bonds.Add(Bond.StrikeTeam);
        return bonds;
    }

    public static bool IsActive(ICombatState? combat, Bond bond) => combat != null && Active(combat.Players).Contains(bond);

    /// <summary>Living players of one brother (usually just one, but co-op could have two Davids).</summary>
    private static List<Player> Living(ICombatState combat, Func<Player, bool> who) =>
        combat.Players.Where(p => who(p) && p.Creature.IsAlive).ToList();

    private static void FlashIcon(Player p) => p.Creature.GetPower<FamilyBondsPower>()?.Pulse();

    /// <summary>Loc key for a bond's title or description.</summary>
    public static string Key(Bond bond, string part) =>
        $"SPIREBROTHERS-FAMILY_BONDS_POWER.{Regex.Replace(bond.ToString(), "(?<!^)([A-Z])", "_$1").ToUpperInvariant()}.{part}";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await StrikeTeam(player);
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        var bonds = Active(combat.Players);
        if (bonds.Count == 0) return;

        // The friends get the icon too (Honorary Brothers).
        if (Banter.Talks(player) && player.Creature.GetPower<FamilyBondsPower>() == null)
            await PowerCmd.Apply<FamilyBondsPower>(choiceContext, player.Creature, 1, player.Creature, null, silent: true);

        // In-Laws: Brandi has to keep up with her brothers-in-law. Aim for each one here.
        if (player.Character is Brandi && bonds.Contains(Bond.InLaws))
        {
            int inLaws = combat.Players.Where(IsInLaw).Select(p => p.Character.GetType()).Distinct().Count();
            FlashIcon(player);
            await Aim.Gain(choiceContext, player, InLawsAim * inLaws, null);
        }

        // Group Chat: everyone gets the energy, including a vanilla fourth player.
        if (bonds.Contains(Bond.GroupChat))
        {
            FlashIcon(player);
            await PlayerCmd.GainEnergy(GroupChatEnergy, player);
        }
    }

    // Strike Team: Darktide coherency. Daniel, John, and Huffman each gain Block every turn while all three are standing.
    private static async Task StrikeTeam(Player player)
    {
        if (player.Character is not (Daniel or John or Huffman)) return;
        var combat = player.Creature.CombatState;
        if (combat == null || !IsActive(combat, Bond.StrikeTeam)) return;
        if (Living(combat, p => p.Character is Daniel).Count == 0 || Living(combat, p => p.Character is John).Count == 0
            || Living(combat, p => p.Character is Huffman).Count == 0) return;
        FlashIcon(player);
        await CreatureCmd.GainBlock(player.Creature, StrikeTeamBlock, ValueProp.Unpowered, null);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        var combat = owner?.Creature?.CombatState;
        if (owner == null || combat == null) return;

        // Heated Debate: David's Rants leave the enemy Insulted, ready for a Comeback.
        if (owner.Character is David && cardPlay.Card.Keywords.Contains(BrotherKeywords.Rant)
            && IsActive(combat, Bond.HeatedDebate))
        {
            var target = cardPlay.Target is { IsPlayer: false, IsAlive: true } t ? t : null;
            if (target == null)
            {
                var enemies = combat.HittableEnemies.Where(e => e.IsAlive).ToList();
                if (enemies.Count > 0) target = owner.RunState.Rng.CombatTargets.NextItem(enemies);
            }
            if (target != null)
            {
                FlashIcon(owner);
                await PowerCmd.Apply<InsultedPower>(choiceContext, target, HeatedDebateInsulted, owner.Creature, cardPlay.Card);
            }
        }

        // Wired for Sound: the first Wired each turn gives Joshua a Verse.
        var state = owner.PlayerCombatState;
        if (owner.Character is Daniel && state != null && TurnTracker.WiredFiredThisTurn.Get(state)
            && WiredForSoundRound.Get(state) != combat.RoundNumber && IsActive(combat, Bond.WiredForSound))
        {
            WiredForSoundRound.Set(state, combat.RoundNumber);
            FlashIcon(owner);
            foreach (var joshua in Living(combat, p => p.Character is Joshua))
                await Verses.Gain(choiceContext, joshua, WiredForSoundVerses, null);
        }

        // Coworkers: Daniel's first Wired each turn sends Huffman some Metal. IT always has spare parts.
        if (owner.Character is Daniel && state != null && TurnTracker.WiredFiredThisTurn.Get(state)
            && CoworkersRound.Get(state) != combat.RoundNumber && IsActive(combat, Bond.Coworkers))
        {
            CoworkersRound.Set(state, combat.RoundNumber);
            FlashIcon(owner);
            foreach (var huffman in Living(combat, p => p.Character is Huffman))
                await Metal.Gain(choiceContext, huffman, CoworkersMetal, null);
        }
    }

    /// <summary>Called from Wired.Check. Husband and Wife: Daniel's first Wired each turn gives Brandi Static. It's always her fault.</summary>
    public static async Task AfterWired(PlayerChoiceContext ctx, Player daniel)
    {
        var combat = daniel.Creature.CombatState;
        var state = daniel.PlayerCombatState;
        if (combat == null || state == null || daniel.Character is not Daniel) return;
        if (HusbandAndWifeRound.Get(state) == combat.RoundNumber || !IsActive(combat, Bond.HusbandAndWife)) return;
        HusbandAndWifeRound.Set(state, combat.RoundNumber);
        FlashIcon(daniel);
        foreach (var brandi in Living(combat, p => p.Character is Brandi))
            await StaticPower.Gain(ctx, brandi, HusbandAndWifeStatic, null);
    }

    /// <summary>Called from Exact.Check. Best Friends: David's first Exact each turn makes one of John's Grind cards in hand Grind.</summary>
    public static async Task AfterExact(PlayerChoiceContext ctx, Player david)
    {
        var combat = david.Creature.CombatState;
        var state = david.PlayerCombatState;
        if (combat == null || state == null || david.Character is not David) return;
        if (BestFriendsRound.Get(state) == combat.RoundNumber || !IsActive(combat, Bond.BestFriends)) return;
        BestFriendsRound.Set(state, combat.RoundNumber);
        FlashIcon(david);
        foreach (var john in Living(combat, p => p.Character is John))
        {
            var grinders = PileType.Hand.GetPile(john).Cards.OfType<JohnCard>().Where(c => c.HasGrind).ToList();
            if (grinders.Count == 0) continue;
            var card = david.RunState.Rng.CombatTargets.NextItem(grinders)!;
            await card.GrindTimes(ctx, 1);
            TimCard.RefreshHand(john);
        }
    }

    /// <summary>Called from Mastiff.Attack. Uncle and Nephew: the dog's bites leave some of John's Toxin behind.</summary>
    public static async Task AfterBite(PlayerChoiceContext ctx, Player huffman, Creature target)
    {
        var combat = huffman.Creature.CombatState;
        if (combat == null || !target.IsAlive || !IsActive(combat, Bond.UncleAndNephew)) return;
        FlashIcon(huffman);
        await Toxin.Apply(ctx, target, UncleAndNephewToxin, huffman, null);
    }

    /// <summary>Called from Verses.SpendForChorus whenever anyone plays a Chorus.</summary>
    public static async Task AfterChorus(PlayerChoiceContext ctx, Player singer)
    {
        var combat = singer.Creature.CombatState;
        if (combat == null || singer.Character is not Joshua) return;
        var bonds = Active(combat.Players);

        // Royalties: half of Joshua's songs are about David, and David wants his cut.
        if (bonds.Contains(Bond.Royalties))
        {
            FlashIcon(singer);
            foreach (var david in Living(combat, p => p.Character is David))
                await PlayerCmd.GainGold(RoyaltiesGold, david);
        }

        // Kids' Choir: Uncle Josh leads, the Kids join in (loudly, at the enemies).
        if (bonds.Contains(Bond.KidsChoir))
        {
            FlashIcon(singer);
            foreach (var tim in Living(combat, p => p.Character is Tim))
                await Kids.Act(ctx, tim);
        }
    }

    /// <summary>Called from ScriptPower.AfterScriptRan. Code Review: the first Script effect each turn hands Daniel a Gear.</summary>
    public static async Task AfterScriptRan(Player player)
    {
        var combat = player.Creature.CombatState;
        var state = player.PlayerCombatState;
        if (combat == null || state == null || player.Character is not Tim) return;
        if (CodeReviewRound.Get(state) == combat.RoundNumber || !IsActive(combat, Bond.CodeReview)) return;
        CodeReviewRound.Set(state, combat.RoundNumber);
        FlashIcon(player);
        foreach (var daniel in Living(combat, p => p.Character is Daniel))
        {
            var gear = combat.CreateCard<Gear>(daniel);
            await CardPileCmd.AddGeneratedCardToCombat(gear, PileType.Hand, daniel);
        }
    }

    /// <summary>Family Loan: David spots Tim some of the Age Up price. Tim only, and only with David in the party.</summary>
    public static int AgeUpDiscount(Player? tim)
    {
        if (tim == null || tim.Character is not Tim) return 0;
        var players = tim.RunState?.Players;
        return players != null && Active(players).Contains(Bond.FamilyLoan) ? FamilyLoanDiscount : 0;
    }
}
