using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Joshua;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>Joshua, the Musician. Uses the Regent's visuals until custom art exists.</summary>
public class Joshua : PlaceholderCharacterModel
{
    public const string CharacterId = "Joshua";
    public static readonly Color Color = new("e0a84f");

    public override string PlaceholderID => "regent";
    public override Color NameColor => Color;
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
