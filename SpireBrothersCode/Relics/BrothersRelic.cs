using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>Base for Daniel's relics. Icons: images/relics/<relic_id>.png, <relic_id>_outline.png, big/<relic_id>.png.</summary>
[Pool(typeof(DanielRelicPool))]
public abstract class DanielRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
