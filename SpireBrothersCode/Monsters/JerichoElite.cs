using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Joshua;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The Walls of Jericho (Joshua 6): a gate guard, the Wall, and an archer. Act 2 elite. The defenders are minions,
/// so the fight ends when the Wall goes down. Joshua's last word comes when the walls fall, not at the end.
/// </summary>
public class JerichoElite : BibleElite
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<JerichoGuard>(), ModelDb.Monster<WallOfJericho>(), ModelDb.Monster<JerichoArcher>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<JerichoGuard>().ToMutable(), null),
        (ModelDb.Monster<WallOfJericho>().ToMutable(), null),
        (ModelDb.Monster<JerichoArcher>().ToMutable(), null),
    ];

    // Three full-size bodies are too wide for the default view (the game logged them off-screen); zoom out a little,
    // like vanilla's three-knight elite (0.87).
    public override float GetCameraScaling() => 0.85f;

    public override string LinesKey => "JERICHO";
    protected override bool IsHero(Player p) => p.Character is Character.Joshua;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<TrumpetBlast>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is WallOfJericho;

    // Walls don't talk: the defenders answer Joshua (the archer first, he's up top).
    public override Creature? Speaker(ICombatState combat) =>
        combat.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is JerichoArcher)
        ?? combat.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is JerichoGuard)
        ?? base.Speaker(combat);

    // Joshua already spoke when the walls fell.
    public override void AfterDeath(Creature creature) { }
}
