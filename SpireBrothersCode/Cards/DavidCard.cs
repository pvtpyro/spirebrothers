using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>Base class for David's cards. Handles art paths plus Exact / Hoard / Rant / Share helpers.</summary>
[Pool(typeof(DavidCardPool))]
public abstract class DavidCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // Glow gold when an Exact card would trigger if played now, or a Rant card already has Rants to build on.
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (base.ShouldGlowGoldInternal) return true;
            try
            {
                if (!IsInCombat || Owner?.PlayerCombatState == null) return false;
                if (Keywords.Contains(BrotherKeywords.Exact) && Exact.WouldTrigger(this)) return true;
                if (Keywords.Contains(BrotherKeywords.Rant) && DavidTracker.RantsThisTurn(Owner) > 0) return true;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Glow check failed on {GetType().Name}: {e}");
            }
            return false;
        }
    }

    // Cards whose number grows (Hoard, Rant, Summit Push) show "9 (from 6)" with the current number in green,
    // the same way Daniel's Diligent cards do. Text uses {CalcGrown:cond:...|...} and {CalcStart}.
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        foreach (var key in new[] { "CalculatedDamage", "CalculatedBlock" })
        {
            if (!DynamicVars.TryGetValue(key, out var v) || v is not CalculatedVar calc) continue;
            decimal start = DynamicVars["CalculationBase"].BaseValue;
            decimal now = start;
            try { now = calc.Calculate(null); }
            catch (Exception e) { MainFile.Logger.Error($"Growth preview failed on {GetType().Name}: {e}"); }
            description.Add("CalcGrown", now > start);
            description.Add("CalcStart", start);
            return;
        }
    }

    /// <summary>Hoard: +1 per <paramref name="perGold"/> Gold the owner is holding.</summary>
    protected int Hoard(int perGold = 50) => (Owner?.Gold ?? 0) / perGold;

    /// <summary>Rant: Rant cards already played this turn (not counting this one).</summary>
    protected int Rants => DavidTracker.RantsThisTurn(Owner);

    /// <summary>"The enemy loses X Strength this turn": its attacks deal X less damage per hit until its turn ends.</summary>
    protected async Task RantAt(PlayerChoiceContext ctx, Creature target, decimal amount)
    {
        if (amount > 0) await PowerCmd.Apply<RantedAtPower>(ctx, target, amount, Owner.Creature, this);
    }

    /// <summary>Lose up to <paramref name="amount"/> Gold (never below 0). Returns how much was actually lost.</summary>
    protected async Task<int> LoseGold(int amount)
    {
        int lost = Math.Min(amount, Owner.Gold);
        if (lost > 0) await PlayerCmd.SetGold(Owner.Gold - lost, Owner);
        return lost;
    }

    /// <summary>Share: the chosen player, or yourself if playing solo / no target was picked.</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;
}
