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

/// <summary>Law: whenever an enemy attacks Huffman, it gains Amount Vulnerable.</summary>
public class AssaultingAnOfficerPower : LawPower
{
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (!IsFoe(command.Attacker) || !HitBy(command).Contains(Owner)) return;
        Enforce();
        for (int i = 0; i < Times; i++) await PowerCmd.Apply<VulnerablePower>(choiceContext, command.Attacker!, Amount, Owner, null);
    }
}
