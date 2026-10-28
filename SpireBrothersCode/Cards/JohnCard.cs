using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>
/// Base class for John's cards (a friend of the brothers). Handles art paths, Grind (run-long growth, see
/// Mechanics/Grind), the "In the Shroud" glow, and the Share / ALL players helpers.
/// </summary>
[Pool(typeof(JohnCardPool))]
public abstract class JohnCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // ---- Grind

    /// <summary>Grind cards name the value that grows (e.g. DynamicVars.Damage) and carry a "Grind" var for how much.</summary>
    protected virtual DynamicVar? GrindTarget => null;

    // How much of the run's growth is already on GrindTarget. Clones copy it along with the number, so the two stay in step.
    private int _grindApplied;
    // Grind happens the first time the card is played each combat. Combat copies start fresh from the deck card.
    private bool _groundThisCombat;

    public bool HasGrind => GrindTarget != null;

    /// <summary>Puts the run's growth onto the number (after a load, or when another copy of the card has Ground).</summary>
    protected void SyncGrind()
    {
        var target = GrindTarget;
        if (target == null) return;
        int want = Grind.Of(this);
        if (want == _grindApplied) return;
        target.BaseValue += want - _grindApplied;
        _grindApplied = want;
    }

    /// <summary>Call at the end of OnPlay: Grinds the first time the card is played this combat.</summary>
    protected async Task GrindAfterPlay(PlayerChoiceContext ctx)
    {
        // Realm First! lifts the once-per-combat rule.
        if (GrindTarget == null || (_groundThisCombat && Owner?.Creature?.GetPower<RealmFirstPower>() == null)) return;
        _groundThisCombat = true;
        await GrindTimes(ctx, 1);
    }

    /// <summary>Grinds right now, ignoring the once-per-combat rule (Grindstone, Best in Slot).</summary>
    public async Task GrindTimes(PlayerChoiceContext ctx, int times)
    {
        if (GrindTarget == null || times <= 0 || Owner == null) return;
        SyncGrind();
        // Rested XP: the next Grind this turn counts twice (3 times upgraded).
        if (Owner.Creature.GetPower<RestedXpPower>() is { } rested)
        {
            times *= Math.Max(1, rested.Amount);
            await PowerCmd.Remove(rested);
        }
        // Efficient Grind (Stimm Lab talent): the first Grind each combat counts twice.
        if (JohnTracker.TakeFirstGrind(Owner) && StimmLab.Rank(Owner, StimmLab.Talent.EfficientGrind) > 0) times *= 2;
        int per = DynamicVars["Grind"].IntValue;
        if (Owner.Creature.GetPower<JustOneMoreRunPower>() != null) per *= 2;
        Grind.Add(this, per * times);
        SyncGrind();
        Grind.Times.Set(Owner, Grind.Times.Get(Owner) + times);
        if (Owner.Creature.GetPower<ChemLabPower>() is { } lab) await lab.OnGrind(times);
        if (Owner.GetRelic<Relics.LootChest>() is { } chest) await chest.OnGrind(times);
    }

    // ---- glow and text

    /// <summary>Cards with an "In the Shroud:" bonus glow gold while John is in it.</summary>
    protected virtual bool ShroudBonus => false;

    /// <summary>Extra glow conditions for individual cards.</summary>
    protected virtual bool ExtraGlow => false;

    protected bool InShroud => Shroud.In(Owner);

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (base.ShouldGlowGoldInternal) return true;
            try
            {
                if (!IsInCombat || Owner?.PlayerCombatState == null) return false;
                // Captain Blockbeard is waiting for an answer: light up the cards that can give him one.
                if (InsultDuelPower.WantsRetort(this)) return true;
                if (ShroudBonus && InShroud) return true;
                if (ExtraGlow) return true;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Glow check failed on {GetType().Name}: {e}");
            }
            return false;
        }
    }

    // Grind text uses {GrindGrown} and {GrindStart}: the printed number dimmed, then the ground-up one in green.
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        try { SyncGrind(); }
        catch (Exception e) { MainFile.Logger.Error($"Grind sync failed on {GetType().Name}: {e}"); }
        Growth.AddCalcArgs(this, description);
        if (GrindTarget != null)
        {
            description.Add("GrindGrown", _grindApplied > 0);
            description.Add("GrindStart", GrindTarget.BaseValue - _grindApplied);
        }
    }

    // ---- co-op

    /// <summary>The player a Share card targets, or the owner in solo (where the target is null).</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;

    /// <summary>Living player creatures in this combat (just you in solo).</summary>
    protected List<Creature> AllPlayers =>
        CombatState?.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList() ?? [Owner.Creature];
}
