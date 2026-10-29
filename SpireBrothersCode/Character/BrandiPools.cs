using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class BrandiCardPool : CustomCardPoolModel
{
    public override string Title => Brandi.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame): her red, a deeper crimson than Tim's.
    public override float H => 0.97f;
    public override float S => 0.85f;
    public override float V => 0.8f;

    public override Color DeckEntryCardColor => Brandi.Color;
    public override bool IsColorless => false;
}

public class BrandiRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Brandi.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class BrandiPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Brandi.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
