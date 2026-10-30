using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Cards.David;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// On every Ba'al, and it looks the same on all of them. Underneath:
/// - Clones are secondary enemies (like vanilla MinionPower), so the real one falling ends the fight and the game kills
///   the rest of them.
/// - At half HP a Ba'al is revealed: a clone gets JustAClonePower, the real one RealBaalPower and Strength.
/// - A clone that falls is replaced by a fresh one from the real one's reserve, and gives back the Gold he stole.
/// - It's also the fight's FoePower (David starts with Due Diligence), run by the first Ba'al only so nobody gets three.
/// Kept after its owner dies (like vanilla AdaptablePower) so AfterDeath still runs for him.
/// </summary>
public class WhichOneIsRealPower : FoePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<DueDiligence>();

    private Baal? Self => Owner.Monster as Baal;

    public override bool OwnerIsSecondaryEnemy => Self is not { IsReal: true };

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    private bool _revealed;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner != Owner.CombatState?.Enemies.FirstOrDefault(e => e.Monster is Baal)) return;
        await base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || _revealed || Owner.IsDead || Owner.CurrentHp * 2 > Owner.MaxHp || Self is not { } baal) return;
        _revealed = true;
        var boss = Owner.CombatState?.Encounter as BaalBoss;
        if (baal.IsReal)
        {
            await PowerCmd.Apply<RealBaalPower>(choiceContext, Owner, 1, Owner, null);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, baal.FoundStrength, Owner, null);
            boss?.Line(Owner, "found");
        }
        else
        {
            await PowerCmd.Apply<JustAClonePower>(choiceContext, Owner, 1, Owner, null);
            boss?.Line(Owner, "notMe");
        }
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner || wasRemovalPrevented || Self is not { } baal) return;
        await baal.ReturnGold();
        if (baal.IsReal) return;
        // Next clone, please. (Not once the real one is down: the fight's over.)
        var combat = Owner.CombatState;
        var real = combat?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Baal { IsReal: true })?.Monster as Baal;
        if (combat == null || real == null || real.ClonesLeft <= 0) return;
        real.ClonesLeft--;
        await Alexander.Summon(real.Creature, ModelDb.Monster<Baal>(), minion: false, order: _ => 0);
        if (combat.Enemies.LastOrDefault(e => e.IsAlive && e.Monster is Baal) is { } fresh)
            (combat.Encounter as BaalBoss)?.Line(fresh, "arrive");
    }
}
