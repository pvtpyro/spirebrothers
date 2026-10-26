using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Helldivers 2. At the start of each combat, add a random Stratagem to your hand.</summary>
public class SuperDestroyer : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Stratagem)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        var stratagems = ModelDb.CardPool<DanielCardPool>()
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanonicalKeywords.Contains(BrotherKeywords.Stratagem))
            .ToList();
        var card = CardFactory.GetDistinctForCombat(player, stratagems, 1, player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (card == null) return;
        Flash();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
    }
}
