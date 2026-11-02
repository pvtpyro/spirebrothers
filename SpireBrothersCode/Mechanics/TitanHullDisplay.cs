using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// The Titan's health bar (Mechanics/Titan): a labelled steel-blue bar under Danny's own health bar and power icons while the
/// Titan stands. Drawn here because the game's bar for the Titan pet never showed up; the pet's own bar is off
/// (DannyTitan.IsHealthBarVisible). Hovering it explains the Titan and its armor. Rebuilt on every change; purely
/// visual, so it's safe to run on every client.
/// </summary>
public static class TitanHullDisplay
{
    private const string NodeName = "SpireBrothersTitanHull";
    // How far under his hitbox it starts: clear of his own health bar and the row of power icons beneath it.
    private const float Height = 26, Border = 3, Below = 100;

    private static readonly Color Frame = new("12161c"), Empty = new("2b3340"), Fill = new("5a8ee8"), Low = new("e0782b");

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
            if (Titan.Unit(player) is not { } titan) return;

            var hitbox = creatureNode.Hitbox;
            float width = Math.Max(160, hitbox.Size.X);
            var display = Build(titan.CurrentHp, titan.MaxHp, width);
            display.Name = NodeName;
            display.MouseFilter = Control.MouseFilterEnum.Stop;
            display.MouseEntered += () => NHoverTipSet.CreateAndShow(display, Tip(player));
            display.MouseExited += () => NHoverTipSet.Remove(display);
            creatureNode.AddChild(display);
            // Centred under his feet, below his own health bar and powers (which hang under his hitbox).
            var bottom = hitbox.GlobalPosition + new Vector2(hitbox.Size.X / 2, hitbox.Size.Y);
            display.GlobalPosition = bottom + new Vector2(-width / 2, Below);
            display.ZIndex = 10;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titan hull display failed: {e}");
        }
    }

    private static IHoverTip Tip(Player player)
    {
        var text = new LocString("powers", "SPIREBROTHERS-TITAN_HULL.tip");
        text.Add("Armor", Titan.ArmorThreshold);
        return new HoverTip(new LocString("powers", "SPIREBROTHERS-TITAN_HULL.name"), text.GetFormattedText());
    }

    private static Control Build(int hp, int max, float width)
    {
        var size = new Vector2(width, Height);
        var root = new Control { Size = size };
        root.AddChild(new ColorRect { Color = Frame, Size = size, MouseFilter = Control.MouseFilterEnum.Ignore });
        var inner = size - new Vector2(Border * 2, Border * 2);
        root.AddChild(new ColorRect
        {
            Color = Empty, Position = new Vector2(Border, Border), Size = inner, MouseFilter = Control.MouseFilterEnum.Ignore
        });
        float part = max <= 0 ? 0 : Math.Clamp(hp / (float)max, 0, 1);
        root.AddChild(new ColorRect
        {
            Color = part * 2 < 1 ? Low : Fill, Position = new Vector2(Border, Border), Size = new Vector2(inner.X * part, inner.Y),
            MouseFilter = Control.MouseFilterEnum.Ignore
        });

        var label = new Label
        {
            Text = $"TITAN  {hp}/{max}",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Size = size,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        label.AddThemeFontSizeOverride("font_size", 20);
        label.AddThemeColorOverride("font_color", Colors.White);
        label.AddThemeColorOverride("font_outline_color", Frame);
        label.AddThemeConstantOverride("outline_size", 6);
        root.AddChild(label);
        return root;
    }
}
