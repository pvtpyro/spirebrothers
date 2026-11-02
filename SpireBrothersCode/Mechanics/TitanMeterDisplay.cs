using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Danny's Titan Meter (Mechanics/Titan): a tall bar on his left that fills from the bottom as he deals damage, turns
/// gold when the Titan is ready, and goes away while the Titan stands. Hovering it explains the meter. Rebuilt on every
/// change; purely visual, so it's safe to run on every client.
/// </summary>
public static class TitanMeterDisplay
{
    private const string NodeName = "SpireBrothersTitanMeter";
    private static readonly Vector2 BarSize = new(26, 190);
    private const float GapLeft = 26, Border = 4;

    private static readonly Color Frame = new("12161c"), Empty = new("2b3340"), Fill = new("3d74d6"), Ready = new("f5c542");

    public static void Update(Player player)
    {
        try
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode == null) return;

            if (creatureNode.GetNodeOrNull<Control>(NodeName) is { } old)
            {
                NHoverTipSet.Remove(old);
                creatureNode.RemoveChild(old);
                old.QueueFree();
            }
            if (Titan.IsIn(player) || !player.Creature.IsAlive) return;

            int meter = Titan.MeterOf(player);
            bool ready = meter >= Titan.MeterSize;
            var display = Build(meter, ready);
            display.Name = NodeName;
            display.MouseFilter = Control.MouseFilterEnum.Stop;
            display.MouseEntered += () => NHoverTipSet.CreateAndShow(display, Tip(player));
            display.MouseExited += () => NHoverTipSet.Remove(display);
            creatureNode.AddChild(display);

            var hitbox = creatureNode.Hitbox;
            var left = hitbox.GlobalPosition + new Vector2(0, hitbox.Size.Y);
            display.GlobalPosition = left - new Vector2(BarSize.X + GapLeft, BarSize.Y + 10);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titan meter display failed: {e}");
        }
    }

    private static IHoverTip Tip(Player player)
    {
        var text = new LocString("powers", "SPIREBROTHERS-TITAN_METER.tip");
        text.Add("Amount", Titan.MeterOf(player));
        text.Add("Size", Titan.MeterSize);
        text.Add("Hull", Titan.MaxHull(player));
        return new HoverTip(new LocString("powers", "SPIREBROTHERS-TITAN_METER.name"), text.GetFormattedText());
    }

    private static Control Build(int meter, bool ready)
    {
        var root = new Control { Size = BarSize + new Vector2(0, 34) };
        var frame = new ColorRect { Color = Frame, Size = BarSize, MouseFilter = Control.MouseFilterEnum.Ignore };
        var empty = new ColorRect
        {
            Color = Empty, Position = new Vector2(Border, Border), Size = BarSize - new Vector2(Border * 2, Border * 2),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        float inner = BarSize.Y - Border * 2;
        float filled = inner * Math.Clamp(meter / (float)Titan.MeterSize, 0, 1);
        var fill = new ColorRect
        {
            Color = ready ? Ready : Fill, Position = new Vector2(Border, Border + inner - filled),
            Size = new Vector2(BarSize.X - Border * 2, filled), MouseFilter = Control.MouseFilterEnum.Ignore
        };
        root.AddChild(frame);
        root.AddChild(empty);
        root.AddChild(fill);
        // Tick marks every quarter, like a gauge.
        for (int i = 1; i < 4; i++)
            root.AddChild(new ColorRect
            {
                Color = Frame, Position = new Vector2(Border, Border + inner * i / 4f - 1), Size = new Vector2(8, 2),
                MouseFilter = Control.MouseFilterEnum.Ignore
            });
        if (ready)
            fill.TreeEntered += () =>
            {
                var pulse = fill.CreateTween().SetLoops();
                pulse.TweenProperty(fill, "modulate", new Color(1.35f, 1.35f, 1.35f), 0.45).SetTrans(Tween.TransitionType.Sine);
                pulse.TweenProperty(fill, "modulate", Colors.White, 0.45).SetTrans(Tween.TransitionType.Sine);
            };

        var label = new Label
        {
            Text = ready ? "!" : meter.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            Position = new Vector2(-20, BarSize.Y + 2),
            Size = new Vector2(BarSize.X + 40, 30),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        label.AddThemeFontSizeOverride("font_size", 26);
        label.AddThemeColorOverride("font_color", ready ? Ready : new Color("cfe0ff"));
        label.AddThemeColorOverride("font_outline_color", Frame);
        label.AddThemeConstantOverride("outline_size", 8);
        root.AddChild(label);
        return root;
    }
}
