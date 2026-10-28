using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.John;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// John, the Grinder. A friend the brothers always play with (an "Honorary Brother"), not a brother himself.
/// Borrows the Ironclad's vanilla scenes where the game needs a Spine one.
/// </summary>
public class John : BrotherCharacter
{
    public const string CharacterId = "John";
    // Friends wear silver and charcoal; the brothers have their own colors.
    public static readonly Color Color = new("a8b0bc");

    protected override string ArtFolder => "john";
    public override string PlaceholderID => "ironclad";
    public override Color NameColor => Color;
    public override Color MapDrawingColor => new("6E8A3A");
    public override Color RemoteTargetingLineColor => new("9CD86AFF");
    public override Color RemoteTargetingLineOutline => new("2E4A14FF");
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 78;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeJohn>(), ModelDb.Card<StrikeJohn>(), ModelDb.Card<StrikeJohn>(), ModelDb.Card<StrikeJohn>(),
        ModelDb.Card<DefendJohn>(), ModelDb.Card<DefendJohn>(), ModelDb.Card<DefendJohn>(), ModelDb.Card<DefendJohn>(),
        ModelDb.Card<CartelSpecial>(),
        ModelDb.Card<ChemGrenade>(),
        ModelDb.Card<CookOff>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<FlameAltar>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<JohnCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<JohnRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<JohnPotionPool>();
}
