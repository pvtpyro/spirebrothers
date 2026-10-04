using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;

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

    protected override bool ShouldGlowGoldInternal =>
        (HasWiredBonus && Wired.IsActive(this)) ||
        (StratagemCombo != null && TurnTracker.MatchesCombo(Owner, StratagemCombo));

    protected override bool IsPlayable =>
        StratagemCombo == null || TurnTracker.MatchesCombo(Owner, StratagemCombo);

    /// <summary>Share: the chosen player, or yourself if playing solo / no target was picked.</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;
}
