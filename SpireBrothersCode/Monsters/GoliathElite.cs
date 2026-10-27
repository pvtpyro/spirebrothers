using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.David;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Goliath and his Shield Bearer: an Act 1 elite (either Act 1, Overgrowth or Underdocks). The bearer stands in front.</summary>
public class GoliathElite : BibleElite
{
    public override bool IsValidForAct(ActModel act) => act.Index == 0;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<ShieldBearer>(), ModelDb.Monster<Goliath>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<ShieldBearer>().ToMutable(), null), (ModelDb.Monster<Goliath>().ToMutable(), null)];

    public override string LinesKey => "GOLIATH";
    protected override bool IsHero(Player p) => p.Character is Character.David;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<SmoothStone>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is Goliath;
}
