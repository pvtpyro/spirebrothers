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
/// Lines are SPIREBROTHERS-&lt;LinesKey&gt;.* in monsters.json: intro (no hero), heroIntro, heroReply, taunts, heroWin.
/// Any number of lines per pool works.
/// </summary>
public abstract class BibleElite() : CustomEncounterModel(RoomType.Elite)
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

    private bool _heroHadLastWord;

    /// <summary>Turn 1: the hero speaks and the foe answers, or the foe opens alone.</summary>
    public void Opening(ICombatState combat)
    {
        var speaker = Speaker(combat);
        var hero = Hero(combat);
        if (hero == null)
        {
            if (speaker != null) Say(speaker, "intro", 1);
            return;
        }
        Say(hero.Creature, "heroIntro", 1);
        if (speaker != null) After(ReplyDelay, () => { if (speaker.IsAlive) Say(speaker, "heroReply", 1); });
    }

    public void Taunt(Creature speaker) => Say(speaker, "taunts", speaker.CombatState?.RoundNumber ?? 0);

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
    private void Say(Creature speaker, string pool, int seed)
    {
        string prefix = $"SPIREBROTHERS-{LinesKey}.{pool}";
        try
        {
            int count = 0;
            while (LocString.Exists("monsters", $"{prefix}.{count}")) count++;
            if (count == 0) return;
            int pick = new System.Random(seed * 7919 + pool.Length * 31).Next(count);
            Banter.Bubble(speaker, new LocString("monsters", $"{prefix}.{pick}").GetFormattedText());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"{LinesKey} line failed ({pool}): {e}");
        }
    }

    private static void After(double seconds, Action action)
    {
        if (Engine.GetMainLoop() is SceneTree tree) tree.CreateTimer(seconds).Timeout += action;
        else action();
    }
}
