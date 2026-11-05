using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>John's starter (Enshrouded). Start each combat in the Shroud. After each combat you win, heal 6 HP.</summary>
public class FlameAltar : JohnRelic
{
    public virtual int Heal => 6;
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into the Kindled Flame Altar.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<KindledFlameAltar>().ToMutable();

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        await Shroud.Enter(choiceContext, player, null);
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner == null || !Owner.Creature.IsAlive) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, Heal);
    }
}

/// <summary>Touch of Orobas upgrade of the Flame Altar: heal 10.</summary>
public class KindledFlameAltar : FlameAltar
{
    public override int Heal => 10;
    public override RelicModel? GetUpgradeReplacement() => null;
}
