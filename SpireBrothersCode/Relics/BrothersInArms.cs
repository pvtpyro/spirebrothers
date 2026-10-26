using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// Shared (all brothers). At the start of each combat, ALL players gain 1 Strength and 1 Dexterity for each
/// Spire Brother in the party (you count). Solo: 1 each. All four brothers: 4 each.
/// </summary>
public class BrothersInArms : DanielRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<DexterityPower>()];

    public static bool IsBrother(Player p) => p.Character is Daniel or David or Joshua or Tim;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var combat = player.Creature.CombatState;
        if (combat == null || combat.RoundNumber != 1) return;
        int brothers = combat.Players.Count(IsBrother);
        if (brothers <= 0) return;
        Flash();
        foreach (var p in combat.Players.Where(p => p.Creature.IsAlive).ToList())
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, p.Creature, brothers, Owner.Creature, null);
            await PowerCmd.Apply<DexterityPower>(choiceContext, p.Creature, brothers, Owner.Creature, null);
        }
    }
}
