using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>Base class for Daniel's cards. Handles art paths plus Wired / Share / Stratagem helpers.</summary>
[Pool(typeof(DanielCardPool))]
public abstract class DanielCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>Cards with a Wired bonus glow gold while it is active.</summary>
    protected virtual bool HasWiredBonus => false;

    /// <summary>Stratagem cards list their input combo here; they can only be played once it's entered.</summary>
    protected virtual IReadOnlyList<CardKeyword>? StratagemCombo => null;

    // The game asks these constantly while cards are on screen, so they must never throw.
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (base.ShouldGlowGoldInternal) return true;
            try
            {
                if (!IsInCombat || Owner?.PlayerCombatState == null) return false;
                if (HasWiredBonus && Wired.IsActive(this)) return true;
                if (StratagemCombo != null && TurnTracker.MatchesCombo(Owner, StratagemCombo)) return true;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Glow check failed on {GetType().Name}: {e}");
            }
            return false;
        }
    }

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            if (StratagemCombo == null) return true;
            try
            {
                return IsInCombat && TurnTracker.MatchesCombo(Owner, StratagemCombo);
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Playable check failed on {GetType().Name}: {e}");
                return false;
            }
        }
    }

    /// <summary>Diligent cards name the value that grows each play (e.g. DynamicVars.Damage).</summary>
    protected virtual DynamicVar? DiligentTarget => null;

    // Total added by Diligent this combat. Combat cards are fresh copies, so this starts at 0 each fight.
    private decimal _diligentBonus;

    /// <summary>Call at the end of OnPlay: grows the Diligent value by the card's "Diligent" amount.</summary>
    protected void GrowDiligent()
    {
        if (DiligentTarget == null) return;
        var amount = DynamicVars["Diligent"].BaseValue;
        // Ten Thousand Hours: each stack adds another full helping of growth.
        var practice = Owner?.Creature?.GetPower<TenThousandHoursPower>();
        if (practice != null) amount *= 1 + practice.Amount;
        DiligentTarget.BaseValue += amount;
        _diligentBonus += amount;
    }

    // Diligent text uses {DiligentGrown} and {DiligentStart} so it can show "12 (from 6)" with 12 in green.
    // Stratagem text uses {Input}: the combo, with steps already entered this turn shown in green.
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        if (DiligentTarget != null)
        {
            description.Add("DiligentGrown", _diligentBonus > 0);
            description.Add("DiligentStart", DiligentTarget.BaseValue - _diligentBonus);
        }
        if (StratagemCombo == null) return;

        int done = 0;
        try
        {
            if (IsInCombat && Owner?.PlayerCombatState != null) done = TurnTracker.ComboProgress(Owner, StratagemCombo);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Stratagem progress failed on {GetType().Name}: {e}");
        }

        var steps = StratagemCombo.Select((kw, i) =>
        {
            var color = i < done ? "green" : "gold";
            return $"[{color}]{TurnTracker.KeywordName(kw)}[/{color}]";
        });
        description.Add("Input", string.Join(", ", steps));
    }

    /// <summary>Factorio: add Gears to this card owner's hand.</summary>
    protected async Task AddGears(int count)
    {
        if (CombatState == null) return;
        for (int i = 0; i < count; i++)
        {
            var gear = CombatState.CreateCard<Gear>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(gear, PileType.Hand, Owner);
        }
    }

    /// <summary>Share: the chosen player, or yourself if playing solo / no target was picked.</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;
}
