using MegaCrit.Sts2.Core.HoverTips;
using SpireBrothers.SpireBrothersCode.Cards.David;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>On Goliath. David starts with a Smooth Stone and gets the opening line (FoePower, GoliathElite).</summary>
public class ChampionOfGathPower : FoePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<SmoothStone>();
}
