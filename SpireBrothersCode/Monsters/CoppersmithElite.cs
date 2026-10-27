using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Tim;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Alexander the Coppersmith (2 Timothy 4:14) and one Copper Automaton to start. Act 2 elite.</summary>
public class CoppersmithElite : BibleElite
{
    public override bool IsValidForAct(ActModel act) => act.Index == 1;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<CopperAutomaton>(), ModelDb.Monster<Alexander>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<CopperAutomaton>().ToMutable(), null), (ModelDb.Monster<Alexander>().ToMutable(), null)];

    public override string LinesKey => "ALEXANDER";
    protected override bool IsHero(Player p) => p.Character is Character.Tim;
    public override CardModel MakeToken(ICombatState combat, Player hero) => combat.CreateCard<SoundDoctrine>(hero);
    protected override bool IsMainFoe(Creature c) => c.Monster is Alexander;
}
