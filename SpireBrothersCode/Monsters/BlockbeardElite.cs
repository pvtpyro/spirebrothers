using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>Captain Blockbeard's fight: an Act 3 elite. The Insult Duel event (also Act 3) can start it too.</summary>
public class BlockbeardElite() : CustomEncounterModel(RoomType.Elite)
{
    public override bool IsValidForAct(ActModel act) => act.Index == 2;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Blockbeard>()];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [(ModelDb.Monster<Blockbeard>().ToMutable(), null)];
}
