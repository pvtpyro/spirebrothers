using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Character;

// Danny with the other two Titanfall 1 chassis (Mechanics/Chassis). Each is its own character so the co-op lobby and
// saved runs carry the choice with no extra code; everything else (cards, relic, art, the `is Danny` checks) is
// Danny's. Their buttons are hidden on the select screen: Danny's button shows a picker instead (ChassisPicker).

/// <summary>Danny, flying a Stryder: a fast, light Titan.</summary>
public class DannyStryder : Danny
{
    public override TitanChassis Chassis => TitanChassis.Stryder;
}

/// <summary>Danny, flying an Ogre: a slow, heavy Titan.</summary>
public class DannyOgre : Danny
{
    public override TitanChassis Chassis => TitanChassis.Ogre;
}
