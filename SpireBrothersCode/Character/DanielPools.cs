using BaseLib.Abstracts;
using Godot;
using SpireBrothers.SpireBrothersCode.Extensions;

namespace SpireBrothers.SpireBrothersCode.Character;

public class DanielCardPool : CustomCardPoolModel
{
    public override string Title => Daniel.CharacterId; // internal name, not displayed
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card back tint (HSV shader on the base frame). Tweak to taste once in game.
    public override float H => 0.55f;
    public override float S => 0.8f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => Daniel.Color;
    public override bool IsColorless => false;
}

public class DanielRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Daniel.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public class DanielPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Daniel.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
