using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The brothers are all named after Bible characters, and each one has an elite from his namesake's story:
/// Goliath (David), the Lions' Den (Daniel), the Walls of Jericho (Joshua), Alexander the Coppersmith (Timothy).
/// The fight works for any party, but if the namesake is there he gets a starting card (<see cref="MakeToken"/>) and
/// the opening and closing lines. FoePower (on <see cref="Speaker"/>) calls into this; Banter stays quiet in these fights.
/// Lines are SPIREBROTHERS-&lt;LinesKey&gt;.* in monsters.json: intro (no hero), heroIntro, heroReply, taunts, heroWin,
/// and sideReply (the <see cref="IsSidekick"/> brother's answer to the opening). Any number of lines per pool works.
/// Not only Bible stories: the Stargate bosses use it too, as Boss rooms (Anubis picks his hero at random).
/// Brandi never talks: any line she'd say becomes a face from Banter's FACES pools.
/// </summary>
public abstract class BibleElite(RoomType roomType = RoomType.Elite) : CustomEncounterModel(roomType)
{
    /// <summary>Seconds between the hero's line and the foe's answer, so the second bubble doesn't cover the first.</summary>
    private const double ReplyDelay = 2.2;

    /// <summary>Loc key under SPIREBROTHERS-, e.g. GOLIATH.</summary>
    public abstract string LinesKey { get; }

    /// <summary>The brother this fight is about.</summary>
    protected abstract bool IsHero(Player p);

    /// <summary>The card the hero starts with.</summary>
    public abstract CardModel MakeToken(ICombatState combat, Player hero);

    /// <summary>The ones that have to go down for the hero's last word (the elite itself, not its helpers).</summary>
    protected abstract bool IsMainFoe(Creature c);

    /// <summary>Who answers the hero and taunts: the first living main foe by default.</summary>
    public virtual Creature? Speaker(ICombatState combat) => combat.Enemies.FirstOrDefault(e => e.IsAlive && IsMainFoe(e));

    public Player? Hero(ICombatState combat) => combat.Players.FirstOrDefault(p => IsHero(p) && p.Creature.IsAlive);

    /// <summary>A second brother who chimes in after the opening (sideReply). None by default.</summary>
    protected virtual bool IsSidekick(Player p) => false;

    public Player? Sidekick(ICombatState combat) => combat.Players.FirstOrDefault(p => IsSidekick(p) && p.Creature.IsAlive);

    private bool _heroHadLastWord;

    /// <summary>Turn 1: the hero speaks and the foe answers, or the foe opens alone. Then the sidekick chimes in.</summary>
    public void Opening(ICombatState combat)
    {
        var speaker = Speaker(combat);
        var hero = Hero(combat);
        double delay = ReplyDelay;
        if (hero == null)
        {
            if (speaker != null) Say(speaker, "intro", 1);
        }
        else
        {
            Say(hero.Creature, "heroIntro", 1);
            if (speaker != null) After(ReplyDelay, () => { if (speaker.IsAlive) Say(speaker, "heroReply", 1); });
            delay += ReplyDelay;
        }
        if (Sidekick(combat) is { } side) After(delay, () => { if (side.Creature.IsAlive) Say(side.Creature, "sideReply", 1); });
    }

    public void Taunt(Creature speaker) => Say(speaker, "taunts", speaker.CombatState?.RoundNumber ?? 0);

    /// <summary>A line from any pool of this fight's (mid-fight moments), seeded from the round like the rest.</summary>
    public void Line(Creature speaker, string pool) => Say(speaker, pool, speaker.CombatState?.RoundNumber ?? 0);

    /// <summary>The hero's last word, once per fight (Jericho says it when the walls fall, the others on the win).</summary>
    public void HeroWins(ICombatState combat)
    {
        if (_heroHadLastWord || Hero(combat) is not { } hero) return;
        _heroHadLastWord = true;
        Say(hero.Creature, "heroWin", combat.RoundNumber);
    }

    /// <summary>Banter.AfterDeath: when the last main foe drops, the hero has the last word.</summary>
    public virtual void AfterDeath(Creature creature)
    {
        var combat = creature.CombatState;
        if (combat == null || !IsMainFoe(creature)) return;
        if (combat.Enemies.Any(e => e != creature && e.IsAlive && IsMainFoe(e))) return;
        HeroWins(combat);
    }

    /// <summary>True if the hero is in this fight and still standing (Banter skips its elite-win line then).</summary>
    public bool HeroHere(ICombatState combat) => Hero(combat) != null;

    // Display only, so the pick is seeded from the round (the same on every co-op client) and never touches the run RNG.
    // Brandi never talks: whatever she'd say, she makes a face (Banter's FACES pools) instead.
    private void Say(Creature speaker, string pool, int seed)
    {
        bool face = speaker.Player?.Character is Character.Brandi;
        string table = face ? "powers" : "monsters";
        string prefix = face ? $"{Banter.Prefix}FACES.{FacePool(pool)}" : $"SPIREBROTHERS-{LinesKey}.{pool}";
        try
        {
            int count = 0;
            while (LocString.Exists(table, $"{prefix}.{count}")) count++;
            if (count == 0) return;
            int pick = new System.Random(seed * 7919 + pool.Length * 31).Next(count);
            Banter.Bubble(speaker, new LocString(table, $"{prefix}.{pick}").GetFormattedText());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"{LinesKey} line failed ({pool}): {e}");
        }
    }

    private string FacePool(string pool) => pool switch
    {
        "intro" or "heroIntro" => "START",
        "heroWin" => roomType == RoomType.Boss ? "BOSS" : "ELITE",
        _ => "REPLY",
    };

    private static void After(double seconds, Action action)
    {
        if (Engine.GetMainLoop() is SceneTree tree) tree.CreateTimer(seconds).Timeout += action;
        else action();
    }
}
