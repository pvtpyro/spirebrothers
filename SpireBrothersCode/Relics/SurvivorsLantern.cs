using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireBrothers.SpireBrothersCode.Relics;

/// <summary>
/// John (Enshrouded). Whenever he enters the Shroud (not when more turns are added), gain Block. Called from
/// Mechanics/Shroud.Enter. It used to stop the Shroud's HP loss, which was the same as the Shroud Lungs talent.
/// </summary>
public class SurvivorsLantern : JohnRelic
{
    public const int Block = 5;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public async Task OnEnterShroud()
    {
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, Block, ValueProp.Unpowered, null);
    }
}
