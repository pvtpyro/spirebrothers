using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Monkey Island. At the start of each combat, apply 1 Insulted to ALL enemies. Description is monkey-speak.</summary>
public class MonkeyPhrasebook : DanielRelic
{
    public const int Insults = 1;
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [MonkeySpeak.Translation("relics", Id.Entry), HoverTipFactory.FromPower<InsultedPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        Flash();
        await PowerCmd.Apply<InsultedPower>(choiceContext, combat.HittableEnemies, Insults, player.Creature, null);
    }
}
