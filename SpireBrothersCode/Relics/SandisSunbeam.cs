using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// In memory of Sandi, the split-faced tabby she got with her late husband, who always found the sunny spot. At the
/// start of your 2nd turn each combat, gain 1 Energy and draw 1 card. In Brandi's pool, and added to Daniel's in MainFile.
/// </summary>
public class SandisSunbeam : BrandiRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.Creature.CombatState is not { RoundNumber: 2 }) return;
        Flash();
        await PlayerCmd.GainEnergy(1, player);
        await CardPileCmd.Draw(choiceContext, 1, player);
    }
}
