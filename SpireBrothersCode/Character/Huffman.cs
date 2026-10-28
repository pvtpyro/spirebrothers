using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.Huffman;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Huffman, the Arbitrator. A friend the brothers always play with (an "Honorary Brother"), not a brother himself.
/// Borrows the Necrobinder's vanilla scenes (the one with a pet, like his dog) where the game needs a Spine one.
/// </summary>
public class Huffman : BrotherCharacter
{
    public const string CharacterId = "Huffman";
    // Friends wear silver and charcoal; the brothers have their own colors.
    public static readonly Color Color = new("9aa6b8");

    protected override string ArtFolder => "huffman";
    public override string PlaceholderID => "necrobinder";
    public override Color NameColor => Color;
    public override Color MapDrawingColor => new("2F63C4");
    public override Color RemoteTargetingLineColor => new("6FA0F0FF");
    public override Color RemoteTargetingLineOutline => new("14306AFF");
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 80;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeHuffman>(), ModelDb.Card<StrikeHuffman>(), ModelDb.Card<StrikeHuffman>(), ModelDb.Card<StrikeHuffman>(),
        ModelDb.Card<DefendHuffman>(), ModelDb.Card<DefendHuffman>(), ModelDb.Card<DefendHuffman>(), ModelDb.Card<DefendHuffman>(),
        ModelDb.Card<SicEm>(),
        ModelDb.Card<HaveYouTriedRebooting>(),
        ModelDb.Card<Nanolathe>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<ArbitesBadge>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<HuffmanCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HuffmanRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HuffmanPotionPool>();
}
