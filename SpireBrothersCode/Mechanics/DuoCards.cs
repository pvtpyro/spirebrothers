using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// A Duo card: one brother's card that helps a specific other brother. It only shows up (rewards, shops, random
/// card generation) when that brother is in the party. Give it the Duo keyword so the hover tip explains that.
/// </summary>
public interface IDuoCard
{
    /// <summary>The brother this card works with (Character.Daniel, Character.David, ...).</summary>
    Type Partner { get; }
}

/// <summary>Keeps Duo cards out of card rewards and shops unless their partner is in the party.</summary>
public class DuoCards() : CustomSingletonModel(HookType.Run)
{
    /// <summary>True unless this is a Duo card whose partner isn't in the run (the player themselves counts).</summary>
    public static bool Allowed(CardModel card, Player player) =>
        card is not IDuoCard duo || player.RunState.Players.Any(p => p.Character.GetType() == duo.Partner);

    /// <summary>Living players who are this card's partner (usually one; co-op could have two of the same brother).</summary>
    public static List<Player> Partners(ICombatState? combat, Type partner) =>
        combat?.Players.Where(p => p.Character.GetType() == partner && p.Creature.IsAlive).ToList() ?? [];

    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (options.CustomCardPool != null)
            return options.CustomCardPool.All(c => Allowed(c, player))
                ? options
                : options.WithCustomPool(options.CustomCardPool.Where(c => Allowed(c, player)));
        var filter = options.CardPoolFilter;
        return options.WithCardPools(options.CardPools.ToList(), c => (filter == null || filter(c)) && Allowed(c, player));
    }

    public override IEnumerable<CardModel> ModifyMerchantCardPool(Player player, IEnumerable<CardModel> options) =>
        options.Where(c => Allowed(c, player));
}
