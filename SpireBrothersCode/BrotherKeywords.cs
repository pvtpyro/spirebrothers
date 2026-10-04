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
}
