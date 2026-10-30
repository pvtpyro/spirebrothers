using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace SpireBrothers.SpireBrothersCode.Monsters;

/// <summary>
/// The golden Sarcophagus behind Apophis (ApophisBoss). A minion that never attacks: every turn it heals him. While it
/// stands, Apophis falling only sends him into it (SarcophagusPower), and he rises with 180 HP; his rising spends it.
/// No Personal Shield on it, so big hits land in full.
/// </summary>
public class Sarcophagus : PixelMonster
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 80, 70);
    public override int MaxInitialHp => MinInitialHp;
    private int RestoreHeal => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 6);

    protected override string ArtFolder => "sarcophagus";
    protected override float BodyHeight => 200f;

    public override string? CustomDeathSfx => "event:/sfx/enemy/enemy_attacks/mechaknight/mechaknight_die";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1, null, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var restore = new MoveState("RESTORE", Restore, new HealIntent());
        restore.FollowUpState = restore;
        return new MonsterMoveStateMachine([restore], restore);
    }

    private async Task Restore(IReadOnlyList<Creature> targets)
    {
        var apophis = Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive && e.Monster is Apophis);
        if (apophis == null || apophis.CurrentHp >= apophis.MaxHp) return;
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.4f);
        await CreatureCmd.Heal(apophis, RestoreHeal);
    }
}
