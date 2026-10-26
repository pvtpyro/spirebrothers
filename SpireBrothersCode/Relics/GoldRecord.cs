using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Whenever you play a Chorus card, ALL players heal 2. Verses.SpendForChorus runs relic listeners too.</summary>
public class GoldRecord : JoshuaRelic, IChorusListener
{
    public const int HealAmount = 2;
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(BrotherKeywords.Chorus)];

    public async Task OnChorus(PlayerChoiceContext choiceContext, Player player, int verses)
    {
        var combat = Owner.Creature.CombatState;
        if (player != Owner || combat == null) return;
        Flash();
        foreach (var p in combat.Players.Where(p => p.Creature.IsAlive).ToList())
            await CreatureCmd.Heal(p.Creature, HealAmount);
    }
}
