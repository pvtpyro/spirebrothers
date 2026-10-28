using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Potions;

/// <summary>Base for David's potions. Same image layout as Daniel's.</summary>
[Pool(typeof(DavidPotionPool))]
public abstract class DavidPotion : CustomPotionModel
{
    private string ImageName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string CustomPackedImagePath => ImageName.PotionImagePath();
    public override string CustomPackedOutlinePath => $"outline/{ImageName}".PotionOutlineImagePath();
}

/// <summary>Base for Joshua's potions. Same image layout as Daniel's.</summary>
[Pool(typeof(JoshuaPotionPool))]
public abstract class JoshuaPotion : CustomPotionModel
{
    private string ImageName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string CustomPackedImagePath => ImageName.PotionImagePath();
    public override string CustomPackedOutlinePath => $"outline/{ImageName}".PotionOutlineImagePath();
}

/// <summary>Base for Tim's potions. Same image layout as Daniel's.</summary>
[Pool(typeof(TimPotionPool))]
public abstract class TimPotion : CustomPotionModel
{
    private string ImageName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string CustomPackedImagePath => ImageName.PotionImagePath();
    public override string CustomPackedOutlinePath => $"outline/{ImageName}".PotionOutlineImagePath();
}

/// <summary>Base for John's potions (a friend). Same image layout as Daniel's.</summary>
[Pool(typeof(JohnPotionPool))]
public abstract class JohnPotion : CustomPotionModel
{
    private string ImageName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string CustomPackedImagePath => ImageName.PotionImagePath();
    public override string CustomPackedOutlinePath => $"outline/{ImageName}".PotionOutlineImagePath();
}

/// <summary>Base for Huffman's potions (a friend). Same image layout as Daniel's.</summary>
[Pool(typeof(HuffmanPotionPool))]
public abstract class HuffmanPotion : CustomPotionModel
{
    private string ImageName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
    public override string CustomPackedImagePath => ImageName.PotionImagePath();
    public override string CustomPackedOutlinePath => $"outline/{ImageName}".PotionOutlineImagePath();
}
