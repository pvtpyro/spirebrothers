using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// Base for the family events (? rooms). They only show up when at least one Spire Brother (or Brandi) is in the party.
/// Picture: images/events/&lt;event_id&gt;.png, wide like vanilla's (3440x1613; the placeholders are half that).
/// Text: events.json, "&lt;ID&gt;.pages.&lt;PAGE&gt;.description" and "&lt;ID&gt;.pages.&lt;PAGE&gt;.options.&lt;METHOD_NAME&gt;.title/description".
/// </summary>
public abstract class BrotherEvent : CustomEventModel
{
    public override string? CustomInitialPortraitPath => $"events/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ImagePath();

    public override bool IsAllowed(IRunState runState) =>
        runState.Players.Any(p => BrothersInArms.IsBrother(p) || p.Character is Character.Brandi);

    /// <summary>
    /// Events about the brothers' parents set this. John, Huffman, and Brandi then read FRIEND_&lt;PAGE&gt; pages instead
    /// (same options, same effects): it's not their Mom and Dad, so they're the guest who's been coming over for years.
    /// </summary>
    protected virtual bool HasFriendText => false;

    private bool ForFriend => HasFriendText && Owner?.Character is Character.John or Character.Huffman or Character.Brandi;

    /// <summary>A page key, swapped for its FRIEND_ copy when one of the friends is playing.</summary>
    protected string Page(string pageKey) => ForFriend ? "FRIEND_" + pageKey : pageKey;

    public override LocString InitialDescription => ForFriend ? PageDescription(Page("INITIAL")) : base.InitialDescription;

    /// <summary>Ends the event, showing the text of page <paramref name="pageKey"/> (or its FRIEND_ copy).</summary>
    protected void Finish(string pageKey) => SetEventFinished(PageDescription(Page(pageKey)));

    protected async Task HealPercent(int percent)
    {
        var creature = Owner!.Creature;
        await CreatureCmd.Heal(creature, Math.Max(1, creature.MaxHp * percent / 100));
    }

    protected async Task LoseHp(int amount) =>
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, amount, ValueProp.Unblockable | ValueProp.Unpowered, (Creature?)null);

    protected async Task GainMaxHp(int amount) => await CreatureCmd.GainMaxHp(Owner!.Creature, amount);

    protected async Task GainGold(int amount) => await PlayerCmd.GainGold(amount, Owner!);

    /// <summary>A random potion, offered as a reward so a full belt can still swap.</summary>
    protected async Task RandomPotion()
    {
        var potion = PotionFactory.CreateRandomPotionOutOfCombat(Owner!, Rng).ToMutable();
        await RewardsCmd.OfferCustom(Owner!, [new PotionReward(potion, Owner!)]);
    }

    protected async Task RandomRelic()
    {
        var relic = RelicFactory.PullNextRelicFromFront(Owner!).ToMutable();
        await RelicCmd.Obtain(relic, Owner!);
    }

    protected Task UpgradeRandomCards(int count)
    {
        var cards = PileType.Deck.GetPile(Owner!).Cards.Where(c => c?.IsUpgradable ?? false).ToList().StableShuffle(Rng).Take(count);
        foreach (var card in cards) CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }

    protected async Task UpgradeChosenCards(int count)
    {
        var cards = await CardSelectCmd.FromDeckForUpgrade(Owner!, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, count));
        foreach (var card in cards) CardCmd.Upgrade(card);
    }

    protected async Task RemoveCards(int count)
    {
        var cards = (await CardSelectCmd.FromDeckForRemoval(prefs: new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, count), player: Owner!)).ToList();
        await CardPileCmd.RemoveFromDeck(cards);
    }

    protected async Task TransformCard()
    {
        var card = (await CardSelectCmd.FromDeckForTransformation(Owner!, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1))).FirstOrDefault();
        if (card != null) await CardCmd.TransformToRandom(card, Rng, CardPreviewStyle.EventLayout);
    }

    protected async Task AddToDeck<T>() where T : CardModel
    {
        var card = Owner!.RunState.CreateCard<T>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
    }

    /// <summary>Shows a card's hover tip when hovering the option, like vanilla events that add a card.</summary>
    protected static EventOption ShowingCard<T>(EventOption option) where T : CardModel
    {
        option.HoverTips = HoverTipFactory.FromCardWithCardHoverTips<T>();
        return option;
    }
}
