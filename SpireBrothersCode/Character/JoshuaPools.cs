using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class JoshuaCardPool : CustomCardPoolModel
{
    public override string Title => Joshua.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame). Tweak to taste once in game.
    public override float H => 0.11f;
    public override float S => 0.65f;
    public override float V => 0.9f;

    public override Color DeckEntryCardColor => Joshua.Color;
    public override bool IsColorless => false;
}

public class JoshuaRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Joshua.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class JoshuaPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Joshua.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
