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

/// <summary>Law: whenever the Marked enemy attacks, Huffman's dog bites it (Amount times).</summary>
public class ResistingArrestPower : LawPower
{
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (!IsFoe(command.Attacker) || command.Attacker!.GetPower<MarkedPower>() == null || Owner.Player is not { } player) return;
        Enforce();
        for (int i = 0; i < Amount * Times && command.Attacker.IsAlive; i++) await Mastiff.Attack(choiceContext, player, command.Attacker);
    }
}
