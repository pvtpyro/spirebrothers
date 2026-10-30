using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
/// Sparky. Stays until used: whenever Brandi touches something (plays an Attack or a Share card), it discharges for
/// Amount damage to a random enemy (ALL enemies with Static Field), then it's gone. Only the Static she had before the
/// card counts, so a card that gives Static (Static Shock) keeps it for next time, and one that spends it (Arc Flash)
/// leaves nothing to discharge.
/// </summary>
public class StaticPower : BrothersPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _beforeCard;

    public static async Task Gain(PlayerChoiceContext ctx, Player player, int amount, CardModel? card)
    {
        if (amount <= 0 || !player.Creature.IsAlive) return;
        await PowerCmd.Apply<StaticPower>(ctx, player.Creature, amount, player.Creature, card);
    }

    public static int On(Player? player) => player?.Creature?.GetPowerAmount<StaticPower>() ?? 0;

    /// <summary>Spends all of it (Arc Flash, Power Down) and says how much there was.</summary>
    public static async Task<int> SpendAll(Player player)
    {
        if (player.Creature.GetPower<StaticPower>() is not { } power) return 0;
        int amount = power.Amount;
        await PowerCmd.Remove(power);
        return amount;
    }

    private bool Touches(CardModel card) =>
        card.Owner?.Creature == Owner && (card.Type == CardType.Attack || card.Keywords.Contains(BrotherKeywords.Share));

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Touches(cardPlay.Card)) _beforeCard = Amount;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!Touches(cardPlay.Card) || _beforeCard <= 0) return;
        int zap = Math.Min(_beforeCard, Amount);
        _beforeCard = 0;
        if (zap <= 0 || Owner.CombatState is not { } combat) return;
        var enemies = combat.HittableEnemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) return;
        Flash();
        Sounds.Play(Sounds.BrandiZap);
        var targets = Owner.GetPower<StaticFieldPower>() != null || Owner.Player is not { } player
            ? enemies
            : [player.RunState.Rng.CombatTargets.NextItem(enemies)!];
        foreach (var target in targets)
        {
            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_lightning");
            await CreatureCmd.Damage(choiceContext, target, zap, ValueProp.Unpowered, Owner);
            RunStats.Add(RunStats.Zapped, Owner.Player, zap);
        }
        if (zap >= Amount) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(choiceContext, this, -zap, null, null);
    }
}