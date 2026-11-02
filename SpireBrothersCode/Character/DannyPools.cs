using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class DannyCardPool : CustomCardPoolModel
{
    public override string Title => Danny.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame): Vanguard blue, deeper than Daniel's teal.
    public override float H => 0.61f;
    public override float S => 0.7f;
    public override float V => 0.85f;

    public override Color DeckEntryCardColor => Danny.Color;
    public override bool IsColorless => false;
}

public class DannyRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Danny.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class DannyPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Danny.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
