using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class DavidCardPool : CustomCardPoolModel
{
    public override string Title => David.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame). Tweak to taste once in game.
    public override float H => 0.33f;
    public override float S => 0.7f;
    public override float V => 0.9f;

    public override Color DeckEntryCardColor => David.Color;
    public override bool IsColorless => false;
}

public class DavidRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => David.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class DavidPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => David.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
