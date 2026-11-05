using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Danny;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Danny, The Pilot (a working name). Daniel's idea, from his love of Titanfall: damage he deals fills the Titan Meter,
/// and when it's full he calls in his Titan. While he's in it, every card flips to its Titan side (Mechanics/Titan,
/// Cards/DannyCard). Borrows the Defect's vanilla scenes (a robot, the closest thing to a mech). Card plan: DANNY_CARDS.md.
/// </summary>
public class Danny : BrotherCharacter
{
    public const string CharacterId = "Danny";
    // Vanguard blue.
    public static readonly Color Color = new("3d74d6");

    protected override string ArtFolder => "danny";
    public override string PlaceholderID => "defect";
    public override Color NameColor => Color;
    public override Color MapDrawingColor => new("2C5AAE");
    public override Color RemoteTargetingLineColor => new("5A8EE8FF");
    public override Color RemoteTargetingLineOutline => new("0E1E3AFF");
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;

    /// <summary>His Titan (Mechanics/Chassis). Danny himself is the Atlas; DannyStryder and DannyOgre pick the others.</summary>
    public virtual TitanChassis Chassis => TitanChassis.Atlas;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeDanny>(), ModelDb.Card<StrikeDanny>(), ModelDb.Card<StrikeDanny>(), ModelDb.Card<StrikeDanny>(),
        ModelDb.Card<DefendDanny>(), ModelDb.Card<DefendDanny>(), ModelDb.Card<DefendDanny>(), ModelDb.Card<DefendDanny>(),
        ModelDb.Card<SmartPistol>(),
        ModelDb.Card<Grapple>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<NeuralLink>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<DannyCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DannyRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DannyPotionPool>();
}
