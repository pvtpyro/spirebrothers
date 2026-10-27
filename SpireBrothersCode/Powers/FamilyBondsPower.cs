using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rooms;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// The Family Bonds icon, given to every brother on turn 1 when 2+ brothers share the party (see FamilyBonds).
/// The number is how many bonds are active; hovering lists each one. Type None so buff-stripping can't remove it.
/// Also carries the two bonds that need a power's hooks: Nobody Gets Left Behind and Family Loan's gold.
/// </summary>
public class FamilyBondsPower : BrothersPower
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _savedThisCombat;

    private List<Bond> Bonds => FamilyBonds.Active(CombatState?.Players);

    public override int DisplayAmount => Bonds.Count;

    /// <summary>FamilyBonds flashes the icon whenever a bond fires (Flash itself is protected).</summary>
    public void Pulse() => Flash();

    // One box listing the bond names, not one box per bond: with all four brothers that was 8 extra boxes, and
    // hovering a brother with a few other powers crashed the game (2026-10-26). The full descriptions are in DESIGN.md.
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var bonds = Bonds;
            if (bonds.Count == 0) return [];
            var names = bonds.Select(b => $"[gold]{new LocString("powers", FamilyBonds.Key(b, "title")).GetFormattedText()}[/gold]");
            return [new HoverTip(new LocString("powers", Id.Entry + ".active"), string.Join("\n", names))];
        }
    }

    // Nobody Gets Left Behind: with all four brothers, the first brother who would die each combat survives. Once for
    // the whole team, so it's tracked across every brother's icon.
    private bool TeamSaved =>
        CombatState?.Players.Any(p => p.Creature.GetPower<FamilyBondsPower>()?._savedThisCombat == true) == true;

    public override bool ShouldDieLate(Creature creature) =>
        creature != Owner || TeamSaved || !Bonds.Contains(Bond.NobodyLeftBehind);

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner || TeamSaved) return;
        _savedThisCombat = true;
        Flash();
        await CreatureCmd.Heal(creature, FamilyBonds.NobodyLeftBehindHeal);
    }

    // Family Loan: Tim pays David back after every win.
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        var player = Owner?.Player;
        if (player?.Character is not David || !Bonds.Contains(Bond.FamilyLoan)) return;
        Flash();
        await PlayerCmd.GainGold(FamilyBonds.FamilyLoanGold, player);
    }
}
