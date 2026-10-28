using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class HuffmanCardPool : CustomCardPoolModel
{
    public override string Title => Huffman.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame): a steel grey with a little blue, for his shirt and the Arbites.
    public override float H => 0.6f;
    public override float S => 0.2f;
    public override float V => 0.75f;

    public override Color DeckEntryCardColor => Huffman.Color;
    public override bool IsColorless => false;
}

public class HuffmanRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Huffman.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class HuffmanPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Huffman.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
