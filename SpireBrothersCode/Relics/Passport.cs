using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>She's traveled all over the world. Each new Act gives a stamp (saved in BrandiSaves); draw 1 more card on turn 1 per stamp.</summary>
public class Passport : BrandiRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override bool ShowCounter => true;
    public override int DisplayAmount => Owner == null ? 0 : BrandiSaves.Stamps.Get(Owner);

    public override Task AfterActEntered()
    {
        if (Owner == null) return Task.CompletedTask;
        BrandiSaves.Stamps.Set(Owner, BrandiSaves.Stamps.Get(Owner) + 1);
        Flash();
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner || player.Creature.CombatState is not { RoundNumber: 1 }) return count;
        return count + BrandiSaves.Stamps.Get(player);
    }
}
