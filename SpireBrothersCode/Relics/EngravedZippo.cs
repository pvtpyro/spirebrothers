using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Brandi's starter ("PVT PYRO" on the side). The first time you apply Ember each combat, apply double (Mechanics/Ember).</summary>
public class EngravedZippo : BrandiRelic
{
    private bool _used;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => false;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into the Eternal Flame.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<EternalFlame>().ToMutable();

    /// <summary>Called with every Ember she applies; returns the amount after the Zippo.</summary>
    public virtual int Boost(int amount, Player by)
    {
        if (_used) return amount;
        _used = true;
        Flash();
        return amount * 2;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _used = false;
        return Task.CompletedTask;
    }
}

/// <summary>Touch of Orobas upgrade of the Engraved Zippo: the first time you apply Ember each turn, apply 2 more.</summary>
public class EternalFlame : EngravedZippo
{
    public const int Extra = 2;
    private int _round = -1;

    public override RelicModel? GetUpgradeReplacement() => null;

    public override int Boost(int amount, Player by)
    {
        int round = by.Creature.CombatState?.RoundNumber ?? -1;
        if (round == _round) return amount;
        _round = round;
        Flash();
        return amount + Extra;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _round = -1;
        return Task.CompletedTask;
    }
}
