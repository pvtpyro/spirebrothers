using MegaCrit.Sts2.Core.Events;

namespace SpireBrothers.SpireBrothersCode.Events;

/// <summary>
/// Each player picks: chat, ask for advice, or vent for an hour. Every brother vents to Mom except Daniel, so his
/// Vent option is locked.
/// </summary>
public class CallingMom : BrotherEvent
{
    public const int ChatHealPercent = 20, VentRemovals = 2;

    protected override bool HasFriendText => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(JustChat, Page("INITIAL")),
        Option(AskForAdvice, Page("INITIAL")),
        Owner?.Character is Character.Daniel ? LockedOption("VENT_FOR_AN_HOUR_LOCKED") : Option(VentForAnHour, Page("INITIAL")),
    ];

    private async Task JustChat()
    {
        await HealPercent(ChatHealPercent);
        Finish("JUST_CHAT");
    }

    private async Task AskForAdvice()
    {
        await TransformCard();
        Finish("ASK_FOR_ADVICE");
    }

    private async Task VentForAnHour()
    {
        await RemoveCards(VentRemovals);
        Finish("VENT_FOR_AN_HOUR");
    }
}
