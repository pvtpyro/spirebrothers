using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpireBrothers.SpireBrothersCode;

/// <summary>Custom keywords used across the Spire Brothers characters.</summary>
public static class BrotherKeywords
{
    // Daniel's two halves. Shown at the top of the card text.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Logic;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Hands;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Stratagem;

    // Written into card text by hand; the keyword just supplies the hover tip.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Wired;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Share;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Diligent;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Comeback;

    // David. Also written into card text by hand.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Exact;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Hoard;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Rant;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Delegate;

    // Joshua. Song shows at the top of the card; Chorus, Check, and Roll (D&D dice) are written into the card text by hand.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Song;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Chorus;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Check;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Roll;

    // Tim. Script shows at the end of the card (so Xref / Snap to Grid can add it); Kids and Age are written by hand.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)] public static CardKeyword Script;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Kids;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Age;

    // John (a friend). Chem shows at the top of the card; Grind, Toxin, and the Shroud are written by hand.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Chem;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Grind;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Toxin;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Shroud;

    // Huffman (a friend). Law shows at the top of the card; the Mastiff, Build, and Metal are written by hand.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Law;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Mastiff;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Build;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Metal;

    // Duo cards (Mechanics/DuoCards): only appear when the brother they help is in the party. Written by hand
    // ("Duo: David.") so the card names its partner.
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Duo;
}
