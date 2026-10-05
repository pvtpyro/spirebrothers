using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.David;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>David, the Min-Maxer. Uses the Silent's visuals until custom art exists.</summary>
public class David : BrotherCharacter
{
    public const string CharacterId = "David";
    public static readonly Color Color = new("6abf69");

    protected override string ArtFolder => "david";
    public override string PlaceholderID => "silent";
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 72;
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_silent";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeDavid>(), ModelDb.Card<StrikeDavid>(), ModelDb.Card<StrikeDavid>(), ModelDb.Card<StrikeDavid>(),
        ModelDb.Card<DefendDavid>(), ModelDb.Card<DefendDavid>(), ModelDb.Card<DefendDavid>(), ModelDb.Card<DefendDavid>(),
        ModelDb.Card<CollectingDust>(),
        ModelDb.Card<DoTheMath>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<OldWallet>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<DavidCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DavidRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DavidPotionPool>();
}
