using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>V Corps (Camp Victory was named for it). At the start of each Elite or Boss combat, gain 1 Energy and 3 Aim.</summary>
public class CampVictory : BrandiRelic
{
    public const int AimGain = 3;
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.Creature.CombatState is not { RoundNumber: 1 } combat) return;
        if (combat.Encounter?.RoomType is not (RoomType.Elite or RoomType.Boss)) return;
        Flash();
        await PlayerCmd.GainEnergy(1, player);
        await Aim.Gain(choiceContext, player, AimGain, null);
    }
}
