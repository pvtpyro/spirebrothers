using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Captain Blockbeard's insult duel. When he insults the party (Cutting Remark), the players have until the end of
/// their next turn to retort: play a Comeback or a Rant at him, or leave him Insulted. Retort and he's Speechless
/// (stunned, his next move skipped). No retort and he gloats: from the players' next turn, Strength until his next
/// attack (GloatingPower).
/// While he waits he shows AwaitingRetortPower, taunts again at the start of the players' turn, and their retort
/// cards glow gold (<see cref="WantsRetort"/>, read by each brother's card base class).
/// </summary>
public class InsultDuelPower : BrothersPower
{
    public const int GloatStrength = 3;

    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _waiting, _retorted, _gloatNext;

    /// <summary>
    /// True if <paramref name="card"/> would answer a Blockbeard who's waiting for a retort right now: a Comeback,
    /// a Rant, or an Insult card. Used for the gold glow.
    /// </summary>
    public static bool WantsRetort(CardModel card)
    {
        var keywords = card.Keywords;
        if (!keywords.Contains(BrotherKeywords.Comeback) && !keywords.Contains(BrotherKeywords.Rant) && card is not IInsultCard) return false;
        return card.CombatState?.Enemies.Any(e => e.IsAlive && e.GetPower<InsultDuelPower>() is { _waiting: true, _retorted: false }) ?? false;
    }

    /// <summary>Cutting Remark: throw an insult and start waiting for a retort.</summary>
    public async Task Insult()
    {
        _waiting = true;
        _retorted = false;
        Flash();
        Say("insults");
        SfxCmd.Play(FmodSfx.fakeMerchantLaugh);
        await PowerCmd.Apply<AwaitingRetortPower>(new ThrowingPlayerChoiceContext(), Owner, 1, Owner, null);
    }

    // Start of the players' turn: an unanswered insult turns into Gloating now (so his attack intent shows the extra
    // damage before they plan their Block), and if a new insult is waiting he reminds them.
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || Owner.IsDead) return;
        if (_gloatNext)
        {
            _gloatNext = false;
            await GloatingPower.Gloat(new ThrowingPlayerChoiceContext(), Owner, GloatStrength);
        }
        if (_waiting)
        {
            Flash();
            Say("waiting");
        }
    }

    // A Comeback or a Rant aimed at him answers the insult on the spot (David answers an insult the only way he knows how).
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var keywords = cardPlay.Card.Keywords;
        if (cardPlay.Target == Owner && (keywords.Contains(BrotherKeywords.Comeback) || keywords.Contains(BrotherKeywords.Rant)))
            await Retorted();
    }

    // So does giving him Insulted (an Insult card, or a Rant with the Heated Debate bond).
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is InsultedPower && power.Owner == Owner && amount > 0) await Retorted();
    }

    /// <summary>
    /// Someone answered him: right away he's speechless, the "Awaiting a Retort" icon goes, and he's stunned so his
    /// intent shows he'll skip the move he was about to make (he does it next turn instead).
    /// </summary>
    private async Task Retorted()
    {
        if (!_waiting || _retorted || Owner.IsDead) return;
        _retorted = true;
        _waiting = false;
        if (Owner.GetPower<AwaitingRetortPower>() is { } awaiting) await PowerCmd.Remove(awaiting);
        Flash();
        Say("speechless");
        await CreatureCmd.Stun(Owner, Owner.Monster?.NextMove.Id);
    }

    // End of the players' turn with the insult still unanswered: he gloats. (Insulted he already had counts as an
    // answer.)
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!_waiting || side != CombatSide.Player || Owner.IsDead) return;
        if (Owner.HasPower<InsultedPower>())
        {
            await Retorted();
            return;
        }
        _waiting = false;
        if (Owner.GetPower<AwaitingRetortPower>() is { } awaiting) await PowerCmd.Remove(awaiting);
        // He gloats now, but the Strength waits for the players' next turn (see AfterSideTurnStart).
        Say("gloats");
        _gloatNext = true;
    }

    // His lines: SPIREBROTHERS-BLOCKBEARD.<kind>.N in monsters.json. Picked from the round number so every co-op
    // client shows the same one, without touching the run's RNG.
    private void Say(string kind)
    {
        try
        {
            int count = 0;
            while (LocString.Exists("monsters", $"SPIREBROTHERS-BLOCKBEARD.{kind}.{count}")) count++;
            if (count == 0) return;
            int round = CombatState?.RoundNumber ?? 0;
            int pick = new Random(round * 7919 + kind.Length * 31).Next(count);
            Banter.Bubble(Owner, new LocString("monsters", $"SPIREBROTHERS-BLOCKBEARD.{kind}.{pick}").GetFormattedText());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Blockbeard line failed: {e}");
        }
    }
}
