using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Tim's out-of-combat rules. He can't Smith at rest sites: his cards improve through Ages instead (see Ages).
/// Kept apart from TimTracker because a run-hook singleton also receives combat hooks, which would double his
/// Age bonus.
/// </summary>
public class TimRunTracker() : CustomSingletonModel(HookType.Run)
{
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player.Character is not SpireBrothers.SpireBrothersCode.Character.Tim) return false;
        var smiths = options.OfType<SmithRestSiteOption>().ToList();
        foreach (var smith in smiths) options.Remove(smith);
        return smiths.Count > 0;
    }
}
