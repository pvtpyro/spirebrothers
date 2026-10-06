using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Shows Tim's current Age above his head. For now it's a number (1 Dark, 2 Feudal, 3 Castle, 4 Imperial).
/// To swap in art, add images/ages/age0.png .. age3.png (Dark .. Imperial); if a file exists it's used instead.
/// Hovering it names the Age, its card bonus, and the cost of the next one.
/// Purely visual, so it's safe to run on every client.
/// </summary>
public static class AgeDisplay
{
    private const string NodeName = "SpireBrothersAge";
    private static readonly Vector2 Size = new(96, 72);
    private const float GapAboveHead = 16;

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

            int age = Ages.Get(player);
            Control display = Build(age);
            display.Name = NodeName;
            display.MouseFilter = Control.MouseFilterEnum.Stop;
            display.Size = Size;
            display.MouseEntered += () => NHoverTipSet.CreateAndShow(display, Tip(player));
            display.MouseExited += () => NHoverTipSet.Remove(display);
            creatureNode.AddChild(display);
            display.GlobalPosition = creatureNode.GetTopOfHitbox() - new Vector2(Size.X / 2, Size.Y + GapAboveHead);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Age display failed: {e}");
        }
    }

    private static IHoverTip Tip(Player player)
    {
        int age = Ages.Get(player);
        var text = new LocString("powers", "SPIREBROTHERS-AGES.tip");
        text.Add("Bonus", Ages.Bonus(player) > 0);
        text.Add("Amount", Ages.Bonus(player));
        text.Add("Maxed", age >= Ages.Imperial);
        text.Add("NextAge", Ages.Name(Math.Min(age + 1, Ages.Imperial)));
        text.Add("Cost", Ages.GoldCost(player));
        return new HoverTip(new LocString("powers", $"SPIREBROTHERS-AGES.name{age}"), text.GetFormattedText());
    }

    private static Control Build(int age)
    {
        var art = $"{MainFile.ResPath}/images/ages/age{age}.png";
        if (ResourceLoader.Exists(art))
        {
            return new TextureRect
            {
                Texture = GD.Load<Texture2D>(art),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };
        }

        var label = new Label
        {
            Text = (age + 1).ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        label.AddThemeFontSizeOverride("font_size", 48);
        label.AddThemeColorOverride("font_color", new Color("f5d27a"));
        label.AddThemeColorOverride("font_outline_color", new Color("1a1208"));
        label.AddThemeConstantOverride("outline_size", 12);
        return label;
    }
}
