using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>
/// Base class for Huffman's cards (a friend of the brothers). Handles art paths, Build (spend Metal for a bonus, see
/// Mechanics/Metal), the glow for Build and Law bonuses, and the Share / ALL players helpers.
/// </summary>
[Pool(typeof(HuffmanCardPool))]
public abstract class HuffmanCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>Build cards name their Metal cost; they glow gold once he has enough.</summary>
    protected virtual int BuildCost => 0;

    /// <summary>Cards with an "If you have a Law" bonus glow gold while he has one.</summary>
    protected virtual bool LawBonus => false;

    /// <summary>Extra glow conditions for individual cards.</summary>
    protected virtual bool ExtraGlow => false;

    /// <summary>How many Laws he has in play.</summary>
    protected int Laws => Owner?.Creature?.Powers.Count(p => p is LawPower) ?? 0;

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
                if (BuildCost > 0 && Metal.CanBuild(Owner, BuildCost)) return true;
                if (LawBonus && Laws > 0) return true;
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

    /// <summary>Spends the Build cost in Metal if he has it; true means the Build bonus happens.</summary>
    protected Task<bool> Build(PlayerChoiceContext ctx) => Metal.Build(ctx, Owner, BuildCost, this);

    /// <summary>The player a Share card targets, or the owner in solo (where the target is null).</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;

    /// <summary>Living player creatures in this combat (just you in solo).</summary>
    protected List<Creature> AllPlayers =>
        CombatState?.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList() ?? [Owner.Creature];
}
