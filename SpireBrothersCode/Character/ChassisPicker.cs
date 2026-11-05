using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Monsters;

namespace SpireBrothers.SpireBrothersCode.Character;

/// <summary>
/// Character select: pick Danny's Titan. DannyStryder and DannyOgre are real characters (Mechanics/Chassis) whose
/// buttons are hidden; Danny's button stands for all three. While it's selected, a "Choose your Titan" row sits at the
/// bottom of the info panel, and picking one calls the game's own SelectCharacter with that character on Danny's
/// button, so the name, description, co-op lobby sync, and Embark all stay vanilla. A teammate on any chassis lights
/// up Danny's button. Plan: DANNY_CHASSIS.md.
/// </summary>
[HarmonyPatch]
public static class ChassisPicker
{
    private const string NodeName = "SpireBrothersChassisPicker";
    private static readonly Color Fill = new("1c2433");
    private static readonly Color Edge = new("4a5260");
    private static readonly Color Picked = new("2c4f8f");
    private static readonly Color Gold = new("e8b64c");

    private static readonly AccessTools.FieldRef<NCharacterSelectScreen, Control> Buttons =
        AccessTools.FieldRefAccess<NCharacterSelectScreen, Control>("_charButtonContainer");

    private static readonly (ChassisKind Kind, Func<CharacterModel> Model)[] Choices =
    [
        (ChassisKind.Atlas, () => ModelDb.Character<Danny>()),
        (ChassisKind.Stryder, () => ModelDb.Character<DannyStryder>()),
        (ChassisKind.Ogre, () => ModelDb.Character<DannyOgre>()),
    ];

    /// <summary>A chassis character other than Danny himself: no button of its own.</summary>
    private static bool IsHidden(CharacterModel? character) => character is Danny && character.GetType() != typeof(Danny);

    private static string Text(string key) => new LocString("powers", "SPIREBROTHERS-CHASSIS." + key).GetFormattedText();

    // Hide the Stryder and Ogre buttons, then re-link controller focus around the buttons that are left (the game
    // links them in a loop at the end of InitCharacterButtons, counting the hidden ones).
    [HarmonyPatch(typeof(NCharacterSelectScreen), "InitCharacterButtons")]
    [HarmonyPostfix]
    private static void AfterInitCharacterButtons(NCharacterSelectScreen __instance)
    {
        try
        {
            var all = Buttons(__instance).GetChildren().OfType<NCharacterSelectButton>().ToList();
            foreach (var button in all.Where(b => IsHidden(b.Character))) button.Visible = false;
            var shown = all.Where(b => b.Visible).ToList();
            for (int i = 0; i < shown.Count; i++)
            {
                shown[i].FocusNeighborLeft = shown[(i + shown.Count - 1) % shown.Count].GetPath();
                shown[i].FocusNeighborRight = shown[(i + 1) % shown.Count].GetPath();
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Hiding the chassis buttons failed: {e}");
        }
    }

    [HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen.SelectCharacter))]
    [HarmonyPostfix]
    private static void AfterSelectCharacter(NCharacterSelectScreen __instance, NCharacterSelectButton charSelectButton, CharacterModel characterModel)
    {
        try
        {
            var picker = Find(__instance) ?? Build(__instance);
            if (picker == null) return;
            bool show = characterModel is Danny && !charSelectButton.IsRandom && !charSelectButton.IsLocked;
            picker.Visible = show;
            if (show) Refresh(__instance, picker, charSelectButton, characterModel);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Chassis picker failed: {e}");
        }
    }

    // Ready (Embark pressed) locks the choice like the character buttons; Unready opens it again.
    [HarmonyPatch(typeof(NCharacterSelectScreen), "OnEmbarkPressed")]
    [HarmonyPostfix]
    private static void AfterEmbark(NCharacterSelectScreen __instance) => SetLocked(__instance);

    [HarmonyPatch(typeof(NCharacterSelectScreen), "OnUnreadyPressed")]
    [HarmonyPostfix]
    private static void AfterUnready(NCharacterSelectScreen __instance) => SetLocked(__instance);

    // The game lights up the button whose character a teammate picked; a teammate on the Stryder or Ogre should light
    // up Danny's (theirs is hidden). Same as the game's loop otherwise.
    [HarmonyPatch(typeof(NCharacterSelectScreen), "RefreshButtonSelectionForPlayer")]
    [HarmonyPrefix]
    private static bool RefreshButtonSelectionForPlayer(NCharacterSelectScreen __instance, LobbyPlayer player)
    {
        try
        {
            if (player.id == __instance.Lobby.LocalPlayer.id) return false;
            foreach (var item in Buttons(__instance).GetChildren().OfType<NCharacterSelectButton>())
            {
                bool match = player.character == item.Character
                             || (item.Character?.GetType() == typeof(Danny) && player.character is Danny);
                if (item.RemoteSelectedPlayers.Contains(player.id) && !match) item.OnRemotePlayerDeselected(player.id);
                else if (match) item.OnRemotePlayerSelected(player.id);
            }
            return false;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Chassis lobby highlight failed: {e}");
            return true;
        }
    }

    // ---------- The row ----------

    private static Control? Find(NCharacterSelectScreen screen) => screen.GetNodeOrNull<Control>("InfoPanel/VBoxContainer/" + NodeName);

    private static Control? Build(NCharacterSelectScreen screen)
    {
        if (screen.GetNodeOrNull<Control>("InfoPanel/VBoxContainer") is not { } panel) return null;
        var root = new VBoxContainer { Name = NodeName, Visible = false };
        root.AddThemeConstantOverride("separation", 6);

        var title = new Label { Text = Text("title") };
        title.AddThemeFontSizeOverride("font_size", 22);
        title.AddThemeColorOverride("font_color", Gold);
        title.AddThemeColorOverride("font_outline_color", Colors.Black);
        title.AddThemeConstantOverride("outline_size", 6);
        root.AddChild(title);

        var row = new HBoxContainer { Name = "Row" };
        row.AddThemeConstantOverride("separation", 10);
        root.AddChild(row);
        foreach (var (kind, model) in Choices)
        {
            var button = new Button { Name = kind.ToString(), CustomMinimumSize = new Vector2(120, 132), FocusMode = Control.FocusModeEnum.None };
            var stack = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore, Alignment = BoxContainer.AlignmentMode.Center };
            stack.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            stack.AddChild(new TextureRect
            {
                Texture = DannyTitan.ArtFor(kind), CustomMinimumSize = new Vector2(69, 90),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest, MouseFilter = Control.MouseFilterEnum.Ignore
            });
            var name = new Label { Text = Text(kind.ToString().ToUpperInvariant()), HorizontalAlignment = HorizontalAlignment.Center, MouseFilter = Control.MouseFilterEnum.Ignore };
            name.AddThemeFontSizeOverride("font_size", 20);
            stack.AddChild(name);
            button.AddChild(stack);
            button.Pressed += () => Pick(screen, kind, model());
            row.AddChild(button);
        }
        panel.AddChild(root);
        return root;
    }

    // Danny's button and the chassis on it right now (one select screen is open at a time).
    private static WeakReference<NCharacterSelectButton>? _dannyButton;
    private static ChassisKind _current;

    private static void Refresh(NCharacterSelectScreen screen, Control picker, NCharacterSelectButton dannyButton, CharacterModel current)
    {
        _dannyButton = new WeakReference<NCharacterSelectButton>(dannyButton);
        _current = ((Danny)current).Chassis.Kind;
        foreach (var (kind, _) in Choices)
            if (picker.GetNodeOrNull<Button>("Row/" + kind) is { } button) Style(button, kind == _current);
        SetLocked(screen);
    }

    private static void Pick(NCharacterSelectScreen screen, ChassisKind kind, CharacterModel chassis)
    {
        try
        {
            if (kind == _current || screen.Lobby.LocalPlayer.isReady) return;
            if (_dannyButton == null || !_dannyButton.TryGetTarget(out var dannyButton) || !GodotObject.IsInstanceValid(dannyButton)) return;
            screen.SelectCharacter(dannyButton, chassis);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Picking a chassis failed: {e}");
        }
    }

    private static void SetLocked(NCharacterSelectScreen screen)
    {
        try
        {
            if (Find(screen)?.GetNodeOrNull<Control>("Row") is not { } row) return;
            bool locked = screen.Lobby?.LocalPlayer.isReady ?? false;
            foreach (var button in row.GetChildren().OfType<Button>()) button.Disabled = locked;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Locking the chassis picker failed: {e}");
        }
    }

    private static void Style(Button button, bool picked)
    {
        StyleBoxFlat Make(Color fill, Color edge, int border) => new()
        {
            BgColor = fill, BorderColor = edge, BorderWidthLeft = border, BorderWidthRight = border, BorderWidthTop = border,
            BorderWidthBottom = border, CornerRadiusTopLeft = 6, CornerRadiusTopRight = 6, CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6
        };
        var normal = picked ? Make(Picked, Gold, 3) : Make(Fill, Edge, 2);
        button.AddThemeStyleboxOverride("normal", normal);
        button.AddThemeStyleboxOverride("hover", picked ? normal : Make(Fill.Lightened(0.15f), Gold, 2));
        button.AddThemeStyleboxOverride("pressed", normal);
        button.AddThemeStyleboxOverride("disabled", picked ? normal : Make(Fill.Darkened(0.3f), Edge, 2));
    }
}
