using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Brandi's numbers that last the whole run, saved with it (BaseLib SavedSpireFields, like Ages). The fields register
/// when this class is first touched, so MainFile.Initialize touches it.
/// </summary>
public static class BrandiSaves
{
    /// <summary>Smiley Shelf: smileys collected this run.</summary>
    public static readonly SavedSpireField<Player, int> Smileys = new(() => 0, "SpireBrothersSmileys");

    /// <summary>Passport: stamps collected this run.</summary>
    public static readonly SavedSpireField<Player, int> Stamps = new(() => 0, "SpireBrothersStamps");
}
