using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Daniel;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>Daniel, the Nerd Who Nerds Wrong. Uses the Defect's visuals until custom art exists.</summary>
public class Daniel : PlaceholderCharacterModel
{
    public const string CharacterId = "Daniel";
    public static readonly Color Color = new("4fb3d9");

    public override string PlaceholderID => "defect";
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 75;

    // The Defect has no screen-wipe sound, so borrow the Ironclad's.
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeDaniel>(), ModelDb.Card<StrikeDaniel>(), ModelDb.Card<StrikeDaniel>(), ModelDb.Card<StrikeDaniel>(),
        ModelDb.Card<DefendDaniel>(), ModelDb.Card<DefendDaniel>(), ModelDb.Card<DefendDaniel>(), ModelDb.Card<DefendDaniel>(),
        ModelDb.Card<RubberDuckDebugging>(),
        ModelDb.Card<PercussiveMaintenance>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<AndYouKnowWhat>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<DanielCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DanielRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DanielPotionPool>();
}
