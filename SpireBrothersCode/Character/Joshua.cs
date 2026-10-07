using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Joshua;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>Joshua, the Musician. Uses the Regent's visuals until custom art exists.</summary>
public class Joshua : BrotherCharacter
{
    public const string CharacterId = "Joshua";
    public static readonly Color Color = new("e0a84f");

    protected override string ArtFolder => "joshua";
    public override string PlaceholderID => "regent";
    public override Color NameColor => Color;
    // Co-op: his pen color when drawing on the map, and the arrow teammates see when he targets something.
    public override Color MapDrawingColor => new("7B3FB8");
    public override Color RemoteTargetingLineColor => new("B98AF0FF");
    public override Color RemoteTargetingLineOutline => new("43206BFF");
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;

    // Not every character has a screen-wipe sound, so borrow the Ironclad's (known to exist).
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeJoshua>(), ModelDb.Card<StrikeJoshua>(), ModelDb.Card<StrikeJoshua>(), ModelDb.Card<StrikeJoshua>(),
        ModelDb.Card<DefendJoshua>(), ModelDb.Card<DefendJoshua>(), ModelDb.Card<DefendJoshua>(), ModelDb.Card<DefendJoshua>(),
        ModelDb.Card<HumATune>(),
        ModelDb.Card<SingAlong>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<WellWornGuitar>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<JoshuaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<JoshuaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<JoshuaPotionPool>();
}
