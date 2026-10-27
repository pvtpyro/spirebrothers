using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>The Lions' Den (Daniel 6): a Lion and a Lioness. Act 1 elite (either Act 1).</summary>
public class LionsDenElite : BibleElite
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Lioness>(), ModelDb.Monster<Lion>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<Lioness>().ToMutable(), null), (ModelDb.Monster<Lion>().ToMutable(), null)];

    public override string LinesKey => "LIONS_DEN";
    protected override bool IsHero(Player p) => p.Character is Character.Daniel;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<ShutTheirMouths>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is Lion;
    // The Lion (with the mane) does the talking.
    public override Creature? Speaker(ICombatState combat) =>
        combat.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Lion and not Lioness) ?? base.Speaker(combat);
}
