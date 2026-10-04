using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Monkey Island. The first time each enemy is Insulted in a combat, it also loses 1 Strength.</summary>
public class StoneMonkeyHead : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [MonkeySpeak.Translation("relics", Id.Entry), HoverTipFactory.FromPower<InsultedPower>()];

    // Enemies already hit this combat. Combat-only, so it isn't saved.
    private readonly HashSet<Creature> _alreadyInsulted = [];

    public override Task BeforeCombatStart()
    {
        _alreadyInsulted.Clear();
        return Task.CompletedTask;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not InsultedPower || amount <= 0) return;
        var enemy = power.Owner;
        if (enemy.IsPlayer || !enemy.IsAlive || !_alreadyInsulted.Add(enemy)) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -1, Owner.Creature, null);
    }
}
