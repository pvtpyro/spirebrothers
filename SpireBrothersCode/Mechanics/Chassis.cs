using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards;
using SpireBrothers.SpireBrothersCode.Cards.Danny;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

public enum ChassisKind { Atlas, Stryder, Ogre }

/// <summary>
/// Danny's Titan chassis (Titanfall 1): picked on the character select screen (Character/ChassisPicker), one character
/// each (Danny is the Atlas; DannyStryder and DannyOgre extend him). The chassis sets the meter size, the hull, the
/// Titan Armor, a passive while the Titan stands, and the Core card he gets on each Titanfall. Plan: DANNY_CHASSIS.md.
/// </summary>
public sealed record TitanChassis(ChassisKind Kind, int MeterSize, int Hull, int Armor)
{
    public static readonly TitanChassis Atlas = new(ChassisKind.Atlas, 40, 30, 5);
    public static readonly TitanChassis Stryder = new(ChassisKind.Stryder, 30, 20, 3);
    public static readonly TitanChassis Ogre = new(ChassisKind.Ogre, 50, 45, 7);

    public static TitanChassis Of(Player? player) => (player?.Character as Character.Danny)?.Chassis ?? Atlas;

    /// <summary>Titanfall: the chassis passive goes on him, and its Core goes in his hand (one at a time).</summary>
    public static async Task AfterTitanfall(PlayerChoiceContext ctx, Player player)
    {
        var chassis = Of(player);
        switch (chassis.Kind)
        {
            case ChassisKind.Atlas: await PowerCmd.Apply<TargetingLinkPower>(ctx, player.Creature, 2, player.Creature, null); break;
            case ChassisKind.Stryder: await PowerCmd.Apply<ThrustersPower>(ctx, player.Creature, 1, player.Creature, null); break;
            case ChassisKind.Ogre: await PowerCmd.Apply<BulwarkPower>(ctx, player.Creature, 1, player.Creature, null); break;
        }

        if (player.Creature.CombatState is not { } combat) return;
        var hand = PileType.Hand.GetPile(player);
        // An unused Core from the last Titan stays in his hand (it Retains), so he doesn't get a second one.
        if (hand.Cards.Any(c => c is ICoreCard)) return;
        CardModel core = chassis.Kind switch
        {
            ChassisKind.Stryder => combat.CreateCard<DashCore>(player),
            ChassisKind.Ogre => combat.CreateCard<ShieldCore>(player),
            _ => combat.CreateCard<DamageCore>(player)
        };
        await CardPileCmd.AddGeneratedCardToCombat(core, PileType.Hand, player);
    }

    /// <summary>Eject: the passive and a running Damage Core go away with the Titan.</summary>
    public static async Task AfterEject(Player player)
    {
        foreach (var power in player.Creature.Powers.Where(p => p is TargetingLinkPower or ThrustersPower or BulwarkPower or DamageCorePower).ToList())
            await PowerCmd.Remove(power);
    }
}

/// <summary>A chassis Core card (Damage Core, Dash Core, Shield Core): handed over on Titanfall, playable only in the Titan.</summary>
public interface ICoreCard;
