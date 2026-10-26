using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Shared (all brothers). Dad's always got something useful in there. When you pick this up and at the start of each
/// act, gain a random potion.
/// </summary>
public class DadsTackleBox : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override Task AfterObtained() => GrabSomething();

    public override Task AfterActEntered() => GrabSomething();

    private async Task GrabSomething()
    {
        if (Owner == null) return;
        var potion = PotionFactory.CreateRandomPotionOutOfCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration).ToMutable();
        Flash();
        await PotionCmd.TryToProcure(potion, Owner);
    }
}
