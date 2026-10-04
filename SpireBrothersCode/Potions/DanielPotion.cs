using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Base for Daniel's potions. Images: images/potions/&lt;potion_id&gt;.png and potions/outline/&lt;potion_id&gt;.png.</summary>
[Pool(typeof(DanielPotionPool))]
public abstract class DanielPotion : CustomPotionModel
{
    private string ImageName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string CustomPackedImagePath => ImageName.PotionImagePath();
    public override string CustomPackedOutlinePath => $"outline/{ImageName}".PotionOutlineImagePath();
}
