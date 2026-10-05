using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Tim;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>Tim (Timothy), the Draftsman. The oldest brother. Uses the Ironclad's visuals until custom art exists.</summary>
public class Tim : PlaceholderCharacterModel
{
    public const string CharacterId = "Tim";
    public static readonly Color Color = new("d9734f");

    public override string PlaceholderID => "ironclad";
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 76;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeTim>(), ModelDb.Card<StrikeTim>(), ModelDb.Card<StrikeTim>(), ModelDb.Card<StrikeTim>(),
        ModelDb.Card<DefendTim>(), ModelDb.Card<DefendTim>(), ModelDb.Card<DefendTim>(), ModelDb.Card<DefendTim>(),
        ModelDb.Card<DadJoke>(),
        ModelDb.Card<LuaScript>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<FamilyMinivan>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<TimCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TimRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TimPotionPool>();
}
