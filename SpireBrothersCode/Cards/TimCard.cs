using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>Base class for Tim's cards. Handles art paths plus Kids / Age / Share helpers.</summary>
[Pool(typeof(TimCardPool))]
public abstract class TimCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>Cards with an "&lt;Age&gt; or later:" bonus name that Age here; they glow gold once it's reached.</summary>
    protected virtual int? AgeBonusAt => null;

    /// <summary>Cards with an "If you have N+ Kids" bonus glow gold once you have that many.</summary>
    protected virtual int KidsThreshold => 0;

    /// <summary>Extra glow conditions for individual cards (Takedown, Musty Flick).</summary>
    protected virtual bool ExtraGlow => false;

    protected bool AgeBonusActive => AgeBonusAt is int age && Ages.Get(Owner) >= age;

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (base.ShouldGlowGoldInternal) return true;
            try
            {
                if (!IsInCombat || Owner?.PlayerCombatState == null) return false;
                if (AgeBonusActive) return true;
                if (KidsThreshold > 0 && Kids.Count(Owner) >= KidsThreshold) return true;
                if (ExtraGlow) return true;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Glow check failed on {GetType().Name}: {e}");
            }
            return false;
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        Growth.AddCalcArgs(this, description);
    }

    /// <summary>Kids-scaling multiplier for CalculatedDamage / CalculatedBlock vars.</summary>
    protected static decimal KidCount(CardModel card, Creature? _) => Kids.Count(card.Owner);

    /// <summary>Living player creatures in this combat (just you in solo).</summary>
    protected List<Creature> AllPlayers =>
        CombatState?.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList() ?? [Owner.Creature];
}
