using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>Base class for Joshua's cards. Handles art paths plus Verse / Chorus / Share / team helpers.</summary>
[Pool(typeof(JoshuaCardPool))]
public abstract class JoshuaCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>Cards with an "If you have N+ Verses" bonus glow gold once you have that many.</summary>
    protected virtual int VerseThreshold => 0;

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (base.ShouldGlowGoldInternal) return true;
            try
            {
                if (!IsInCombat || Owner?.PlayerCombatState == null) return false;
                // Captain Blockbeard is waiting for an answer: light up the cards that can give him one.
                if (Powers.InsultDuelPower.WantsRetort(this)) return true;
                if (VerseThreshold > 0 && Verses.Count(Owner) >= VerseThreshold) return true;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Glow check failed on {GetType().Name}: {e}");
            }
            return false;
        }
    }

    // {Verses} is what a Chorus would count right now, for "(X Verses)" previews in combat.
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        Growth.AddCalcArgs(this, description);
        int verses = 0;
        try
        {
            if (IsInCombat && Owner?.PlayerCombatState != null) verses = Verses.ForChorus(Owner);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Verse preview failed on {GetType().Name}: {e}");
        }
        description.Add("Verses", verses);
    }

    // While a Chorus card resolves, its damage uses the Verses it just spent (they're already gone from the counter).
    private int? _spentVerses;

    /// <summary>Spend all Verses for this Chorus. Returns how many it counts.</summary>
    protected async Task<int> Chorus(PlayerChoiceContext ctx) => await Verses.SpendForChorus(ctx, this);

    /// <summary>CalculatedDamage multiplier for Chorus attacks: Verses spent, or what it would spend if played now.</summary>
    protected static decimal ChorusVerses(CardModel card, Creature? _) =>
        card is JoshuaCard { _spentVerses: int spent } ? spent : Verses.ForChorus(card.Owner);

    /// <summary>A Chorus attack: spend Verses, then attack with CalculatedDamage scaled by them. Returns the Verses counted.</summary>
    protected async Task<int> ChorusAttack(PlayerChoiceContext ctx, CardPlay play, string vfx)
    {
        try
        {
            int verses = await Chorus(ctx);
            _spentVerses = verses;
            await CommonActions.CardAttack(this, play, vfx: vfx).Execute(ctx);
            return verses;
        }
        finally
        {
            _spentVerses = null;
        }
    }

    /// <summary>
    /// A Chorus attack on ALL enemies whose damage depends on each enemy (Diss Track's Insulted). One hit per enemy, since
    /// the game works out a normal ALL-enemies hit once with no target, which would skip per-enemy bonuses.
    /// </summary>
    protected async Task ChorusAttackEach(PlayerChoiceContext ctx, string vfx, Func<Creature, Task>? afterHit = null)
    {
        try
        {
            _spentVerses = await Chorus(ctx);
            foreach (var enemy in CombatState?.HittableEnemies.ToList() ?? [])
            {
                if (!enemy.IsAlive) continue;
                await DamageCmd.Attack(DynamicVars.CalculatedDamage.Calculate(enemy)).FromCard(this).Targeting(enemy).WithHitFx(vfx).Execute(ctx);
                if (afterHit != null) await afterHit(enemy);
            }
        }
        finally
        {
            _spentVerses = null;
        }
    }

    /// <summary>Living player creatures in this combat (just you in solo).</summary>
    protected List<Creature> AllPlayers =>
        CombatState?.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList() ?? [Owner.Creature];

    /// <summary>Share: the chosen player, or yourself if playing solo / no target was picked.</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;

    protected async Task DrawNextTurn(PlayerChoiceContext ctx, Creature who, decimal amount)
    {
        if (amount > 0) await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, who, amount, Owner.Creature, this);
    }

    protected async Task EnergyNextTurn(PlayerChoiceContext ctx, Creature who, decimal amount)
    {
        if (amount > 0) await PowerCmd.Apply<EnergyNextTurnPower>(ctx, who, amount, Owner.Creature, this);
    }
}
