using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Her smiley face collection. Gains a smiley whenever she enters a treasure or event room (saved for the run in
/// BrandiSaves). Start each combat with 1 Block per smiley.
/// </summary>
public class SmileyShelf : BrandiRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => true;
    public override int DisplayAmount => Owner == null ? 0 : BrandiSaves.Smileys.Get(Owner);

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (Owner == null || room.RoomType is not (RoomType.Treasure or RoomType.Event)) return Task.CompletedTask;
        BrandiSaves.Smileys.Set(Owner, BrandiSaves.Smileys.Get(Owner) + 1);
        Flash();
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.Creature.CombatState is not { RoundNumber: 1 }) return;
        int smileys = BrandiSaves.Smileys.Get(player);
        if (smileys <= 0) return;
        Flash();
        await CreatureCmd.GainBlock(player.Creature, smileys, ValueProp.Unpowered, null);
    }
}
