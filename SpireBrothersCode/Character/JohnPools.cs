using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class JohnCardPool : CustomCardPoolModel
{
    public override string Title => John.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame): a steel grey with a little green, for the Hive Scum chems.
    public override float H => 0.3f;
    public override float S => 0.2f;
    public override float V => 0.75f;

    public override Color DeckEntryCardColor => John.Color;
    public override bool IsColorless => false;
}

public class JohnRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => John.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class JohnPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => John.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
