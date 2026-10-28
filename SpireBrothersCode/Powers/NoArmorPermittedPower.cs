using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>Law: whenever an enemy gains Block, it takes Amount damage. (Like vanilla Juggernaut's timing.)</summary>
public class NoArmorPermittedPower : LawPower
{
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (amount < 1m || !IsFoe(creature)) return;
        Enforce();
        for (int i = 0; i < Times; i++) await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), creature, Amount, ValueProp.Unpowered, Owner, null);
    }
}
