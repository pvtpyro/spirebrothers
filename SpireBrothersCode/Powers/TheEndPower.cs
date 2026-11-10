using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.Shared;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On the Ender Dragon (EnderDragonBoss): her End Crystals heal her every turn, and a crystal that breaks blasts her for
/// <see cref="CrystalBlast"/>. Also the fight's FoePower: a random player starts with a Bed.
/// </summary>
public class TheEndPower : FoePower
{
    public const int CrystalBlast = 12;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Bed>();

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Monster is not EndCrystal || !Owner.IsAlive) return;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, CrystalBlast, ValueProp.Unpowered, null, null);
    }
}
