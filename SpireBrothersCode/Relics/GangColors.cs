using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>John (Hive Scum). At the start of each combat, apply 3 Toxin to ALL enemies.</summary>
public class GangColors : JohnRelic
{
    public const int StartingToxin = 3;
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.Creature.CombatState is not { RoundNumber: 1 } combat) return;
        Flash();
        await Toxin.Apply(choiceContext, combat.HittableEnemies, StartingToxin, player, null);
    }
}
