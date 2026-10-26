using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Payday 2. Whenever an enemy dies, gain 20 Gold.</summary>
public class PaydayMask : DavidRelic
{
    public const int GoldPerKill = 20;
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (target.Side == Owner.Creature.Side) return;
        Flash();
        await PlayerCmd.GainGold(GoldPerKill, Owner);
    }
}
