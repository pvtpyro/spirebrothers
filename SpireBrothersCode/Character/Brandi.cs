using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Brandi;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Brandi, Private Pyro. Daniel's wife, so she's family: the Sister-in-Law. Army vet (Signal in an MI unit), sniper at
/// heart, a pyro, and "Sparky". She never talks; her face does it for her. Borrows the Silent's vanilla scenes (the
/// best fit for a sniper) where the game needs a Spine one. Card plan: BRANDI_CARDS.md.
/// </summary>
public class Brandi : BrotherCharacter
{
    public const string CharacterId = "Brandi";
    // Her favorite color.
    public static readonly Color Color = new("d8343a");

    protected override string ArtFolder => "brandi";
    public override string PlaceholderID => "silent";
    public override Color NameColor => Color;
    public override Color MapDrawingColor => new("A3262B");
    public override Color RemoteTargetingLineColor => new("E04A50FF");
    public override Color RemoteTargetingLineOutline => new("3A0C0EFF");
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_silent";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeBrandi>(), ModelDb.Card<StrikeBrandi>(), ModelDb.Card<StrikeBrandi>(), ModelDb.Card<StrikeBrandi>(),
        ModelDb.Card<DefendBrandi>(), ModelDb.Card<DefendBrandi>(), ModelDb.Card<DefendBrandi>(), ModelDb.Card<DefendBrandi>(),
        ModelDb.Card<Spark>(),
        ModelDb.Card<SteadyBreath>(),
        ModelDb.Card<TheLook>()
    ];

    // The Zippo, and her late husband's dog tags (stamped 33), which she always carries.
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<EngravedZippo>(), ModelDb.Relic<DogTags>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<BrandiCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BrandiRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BrandiPotionPool>();
}
