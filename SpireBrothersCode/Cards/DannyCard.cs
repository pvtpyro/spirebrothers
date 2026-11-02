using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Monsters;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>
/// Base class for Danny's cards. Every one has two sides: the Pilot side (normal) and the Titan side, used while his
/// Titan stands (Mechanics/Titan). A card does OnPilot or OnTitan, can target differently on each side
/// (<paramref name="titanTarget"/>), and takes its Titan name from the "titanTitle" loc key. Its description is one
/// "{InTitan:cond:titan text|pilot text}"; the other side shows in a hover tip, so either can be read anywhere. Titan
/// art (card_portraits/&lt;id&gt;_titan.png) is used when it exists.
/// </summary>
[Pool(typeof(DannyCardPool))]
public abstract class DannyCard(int cost, CardType type, CardRarity rarity, TargetType target, TargetType? titanTarget = null) :
    CustomCardModel(cost, type, rarity, target)
{
    private readonly TargetType _titanTarget = titanTarget ?? target;

    private string FileName => Id.Entry.RemovePrefix().ToLowerInvariant();

    // Art: card_portraits/<card_id>.png (250x190) and card_portraits/big/<card_id>.png (1000x760). One picture for both
    // sides for now (a Titan-side lookup flooded the log with "Could not find big card image path").
    public override string CustomPortraitPath => $"{FileName}.png".BigCardImagePath();
    public override string PortraitPath => $"{FileName}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{FileName}.png".CardImagePath();

    // ---- the two sides

    /// <summary>True while this card's owner is in his Titan, so the card shows and plays its Titan side.</summary>
    public bool InTitan
    {
        get
        {
            try
            {
                // The Compendium's Titan Side tickbox (TitanLibraryToggle): only cards with no owner, never a run's deck.
                if (ShowTitanInLibrary && (!IsMutable || Owner == null)) return true;
                return IsMutable && IsInCombat && Titan.IsIn(Owner);
            }
            catch { return false; }
        }
    }

    /// <summary>Set by the Compendium's Titan Side tickbox.</summary>
    public static bool ShowTitanInLibrary;

    public override TargetType TargetType => InTitan ? _titanTarget : base.TargetType;

    public override string Title
    {
        get
        {
            var title = base.Title;
            if (!InTitan) return title;
            var titan = new LocString("cards", Id.Entry + ".titanTitle");
            return titan.Exists() ? titan.GetFormattedText() + (IsUpgraded ? "+" : "") : title;
        }
    }

    protected sealed override Task OnPlay(PlayerChoiceContext ctx, CardPlay play) =>
        InTitan ? OnTitan(ctx, play) : OnPilot(ctx, play);

    protected abstract Task OnPilot(PlayerChoiceContext ctx, CardPlay play);
    protected abstract Task OnTitan(PlayerChoiceContext ctx, CardPlay play);

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        Growth.AddCalcArgs(this, description);
        description.Add("InTitan", InTitan);
    }

    /// <summary>Hover tips this card needs besides the other side and the Titan keyword.</summary>
    protected virtual IEnumerable<IHoverTip> MoreHoverTips => [];

    protected sealed override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { OtherSide(), HoverTipFactory.FromKeyword(BrotherKeywords.Titan) }.OfType<IHoverTip>().Concat(MoreHoverTips);

    // The side this card isn't showing, built from the same description with InTitan flipped.
    private IHoverTip? OtherSide()
    {
        try
        {
            bool titan = !InTitan;
            var text = Description;
            DynamicVars.AddTo(text);
            AddExtraArgsToDescription(text);
            text.Add("InTitan", titan);
            text.Add(new IfUpgradedVar(IsUpgraded ? UpgradeDisplay.Upgraded : UpgradeDisplay.Normal));
            text.Add("OnTable", false);
            text.Add("InCombat", IsInCombat);
            text.Add("IsTargeting", false);
            text.Add("energyPrefix", EnergyIconHelper.GetPrefix(this));
            var name = new LocString("cards", Id.Entry + (titan ? ".titanTitle" : ".title"));
            var header = new LocString("card_keywords", titan ? "SPIREBROTHERS-TITAN.sideTitan" : "SPIREBROTHERS-TITAN.sidePilot");
            header.Add("Name", name.Exists() ? name.GetFormattedText() : "");
            return new HoverTip(header, text.GetFormattedText());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Other-side tip failed on {GetType().Name}: {e}");
            return null;
        }
    }

    // ---- glow

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
                if (InsultDuelPower.WantsRetort(this)) return true;
                if (ExtraGlow) return true;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Glow check failed on {GetType().Name}: {e}");
            }
            return false;
        }
    }

    // ---- helpers

    protected Creature? MyTitan => Titan.Unit(Owner);

    protected Task Repair(decimal amount) => Titan.Repair(Owner, amount);

    protected Task GainMeter(decimal amount) => Titan.GainMeter(Owner, (int)amount);

    protected List<Creature> Enemies => CombatState?.HittableEnemies.Where(e => e.IsAlive).ToList() ?? [];

    /// <summary>An attack from a named damage var (the Titan side's own number), at the card's target(s).</summary>
    protected async Task<AttackCommand> Hit(PlayerChoiceContext ctx, CardPlay play, string damageVar, int hits = 1, string? vfx = null)
    {
        vfx ??= InTitan ? "vfx/vfx_heavy_blunt" : "vfx/vfx_attack_blunt";
        var attack = DamageCmd.Attack(DynamicVars[damageVar].BaseValue).FromCard(this).WithHitCount(hits).WithHitFx(vfx);
        attack = TargetType switch
        {
            TargetType.AllEnemies => attack.TargetingAllOpponents(CombatState!),
            TargetType.RandomEnemy => attack.TargetingRandomOpponents(CombatState!),
            _ => attack.Targeting(play.Target!)
        };
        if (InTitan)
        {
            Sounds.Play(Sounds.DannyTitanPunch);
            if (MyTitan is { } titan) KidMotion.Hop(titan);
        }
        await attack.Execute(ctx);
        return attack;
    }

    protected Task Apply<T>(PlayerChoiceContext ctx, Creature target, decimal amount) where T : PowerModel =>
        amount <= 0 ? Task.CompletedTask : PowerCmd.Apply<T>(ctx, target, amount, Owner.Creature, this);

    protected async Task ApplyAll<T>(PlayerChoiceContext ctx, decimal amount) where T : PowerModel
    {
        foreach (var enemy in Enemies) await Apply<T>(ctx, enemy, amount);
    }

    protected Task Draw(PlayerChoiceContext ctx, decimal count) =>
        count <= 0 ? Task.CompletedTask : CardPileCmd.Draw(ctx, count, Owner);

    /// <summary>Gives Block outside the card's own Block var (no Dexterity, like a relic).</summary>
    protected Task GiveBlock(decimal amount) =>
        amount <= 0 ? Task.CompletedTask : CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);

    /// <summary>Redraws every card in his hand with a quick flip, after he gets into or out of his Titan.</summary>
    public static void FlipHand(Player player)
    {
        try
        {
            if (player.PlayerCombatState == null) return;
            foreach (var card in PileType.Hand.GetPile(player).Cards.ToList())
            {
                if (NCard.FindOnTable(card) is not { } node) continue;
                var body = node.Body;
                if (body == null || !body.IsInsideTree())
                {
                    node.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
                    continue;
                }
                body.PivotOffset = body.Size / 2;
                var tween = body.CreateTween();
                tween.TweenProperty(body, "scale:x", 0.02f, 0.12).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
                tween.TweenCallback(Callable.From(() => node.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal)));
                tween.TweenProperty(body, "scale:x", 1f, 0.12).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Hand flip failed: {e}");
        }
    }
}
