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

/// <summary>
/// Base for Huffman's Laws: Powers that set a rule for the fight and punish any enemy that breaks it. Martial Law makes
/// every Law punish twice.
/// </summary>
public abstract class LawPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected int Times => Owner.GetPower<MartialLawPower>() != null ? 2 : 1;

    /// <summary>A Law punishes someone: flashes the icon and counts toward his end-of-run stats.</summary>
    protected void Enforce()
    {
        Flash();
        RunStats.Add(RunStats.LawsEnforced, Owner.Player);
        Sounds.Play(Sounds.HuffmanLaw);
    }

    protected static bool IsFoe(Creature? creature) => creature is { IsAlive: true, IsEnemy: true };

    protected static IEnumerable<Creature> HitBy(AttackCommand command) =>
        command.Results.SelectMany(r => r).Select(r => r.Receiver).Distinct();
}
