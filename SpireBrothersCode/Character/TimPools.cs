using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class TimCardPool : CustomCardPoolModel
{
    public override string Title => Tim.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame). Tweak to taste once in game.
    public override float H => 0.02f;
    public override float S => 0.65f;
    public override float V => 0.9f;

    public override Color DeckEntryCardColor => Tim.Color;
    public override bool IsColorless => false;
}

public class TimRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Tim.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class TimPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Tim.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
