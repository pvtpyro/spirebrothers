using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Danny's starter: the pilot-Titan neural link. No effect for now (it used to start the Titan Meter at 10; the meter
/// starts empty now, and its new job is waiting on playtesting).
/// </summary>
public class NeuralLink : DannyRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => false;

    // Touch of Orobas (the Ancient that upgrades starter relics) turns this into Protocol 1.
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<ProtocolOne>().ToMutable();
}

/// <summary>Touch of Orobas upgrade of the Neural Link ("Protocol 1: Link to Pilot"). No effect for now either.</summary>
public class ProtocolOne : NeuralLink
{
    public override RelicModel? GetUpgradeReplacement() => null;
}
