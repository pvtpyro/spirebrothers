using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>
/// Base class for Brandi's cards (Daniel's wife). Handles art paths, Shot attacks (bonus damage per Aim, then the Aim is
/// spent; see Mechanics/Aim), Look (reading the enemy's intent), glow, and the Share / ALL players helpers.
/// </summary>
[Pool(typeof(BrandiCardPool))]
public abstract class BrandiCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760).
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // ---- Shot

    /// <summary>Shot attacks deal CalculatedDamage (printed damage + ExtraDamage per Aim), then spend her Aim.</summary>
    public virtual bool IsShot => false;

    /// <summary>The vars for a Shot: <paramref name="damage"/> printed, +<paramref name="perAim"/> per Aim.</summary>
    protected static DynamicVar[] ShotVars(int damage, int perAim = 3) =>
    [
        new CalculationBaseVar(damage),
        new ExtraDamageVar(perAim),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(AimMultiplier)
    ];

    private static decimal AimMultiplier(CardModel card, Creature? _) => Aim.Count(card.Owner);

    /// <summary>Fires the Shot at the card's target(s), then spends her Aim. Returns the Aim spent.</summary>
    protected async Task<int> Shoot(PlayerChoiceContext ctx, CardPlay play, string vfx = "vfx/vfx_dramatic_stab", bool keepHalf = false)
    {
        Sounds.Play(Sounds.BrandiShot);
        await CommonActions.CardAttack(this, play, vfx: vfx).Execute(ctx);
        RunStats.Add(RunStats.Shots, Owner);
        return await Aim.Spend(ctx, Owner, keepHalf);
    }

    // ---- Look

    /// <summary>Look cards read the enemy's intent (like vanilla Go for the Eyes).</summary>
    protected static bool Attacking(Creature? enemy) => enemy?.Monster?.IntendsToAttack ?? false;

    // ---- glow and playability

    /// <summary>Extra glow conditions for individual cards.</summary>
    protected virtual bool ExtraGlow => false;

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
                if (IsShot && Aim.Count(Owner) > 0) return true;
                if (ExtraGlow) return true;
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
            // One Shot, One Kill: one Shot attack a turn.
            if (!IsShot || !IsInCombat || Owner?.Creature?.GetPower<OneShotOneKillPower>() == null) return true;
            return Aim.ShotsThisTurn(Owner, CombatState) == 0;
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        Growth.AddCalcArgs(this, description);
    }

    // ---- targets

    /// <summary>The player a Share card targets, or the owner in solo (where the target is null).</summary>
    protected Creature ShareTarget(CardPlay play) =>
        play.Target is { IsPlayer: true, IsAlive: true } t ? t : Owner.Creature;

    /// <summary>Living player creatures in this combat (just you in solo).</summary>
    protected List<Creature> AllPlayers =>
        CombatState?.Players.Where(p => p.Creature.IsAlive).Select(p => p.Creature).ToList() ?? [Owner.Creature];

    /// <summary>Living enemies in this combat.</summary>
    protected List<Creature> Enemies => Ember.Enemies(CombatState);

    /// <summary>A random living teammate (run RNG), or herself in solo.</summary>
    protected Creature RandomTeammate()
    {
        var others = AllPlayers.Where(c => c != Owner.Creature).ToList();
        return others.Count == 0 ? Owner.Creature : Owner.RunState.Rng.CombatTargets.NextItem(others)!;
    }

    /// <summary>Gives the player a Block amount outside the card's own Block var (no Dexterity, like a relic).</summary>
    protected static Task GiveBlock(Creature who, decimal amount) =>
        amount <= 0 ? Task.CompletedTask : CreatureCmd.GainBlock(who, amount, ValueProp.Unpowered, null);
}
