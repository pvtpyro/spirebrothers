using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// The Family Album screen: a leather scrapbook with the snapshots taped in as polaroids, three to a page, newest first,
/// with All / Victories / Elites / Defeats filters. Built from plain Godot controls over the main menu (laid out on a
/// 1920 x 1080 stage); Back or a right-click closes it. Drawn photos stand the bodies on a floor in the act's colors,
/// to scale, foes mirrored to face the party, the beaten ones tipped over.
/// </summary>
public static class AlbumScreen
{
    private static readonly Vector2 Stage = new(1920, 1080);
    private const int PerSpread = 6;

    private static readonly Color Background = new("161a20");
    private static readonly Color Gold = new("e8b64c");
    private static readonly Color Leather = new("5a2e1f");
    private static readonly Color Spine = new("3f1f15");
    private static readonly Color Paper = new("efe4cc");
    private static readonly Color PhotoPaper = new("fbf8f1");
    private static readonly Color Ink = new("2a2420");
    private static readonly Color FadedInk = new("6d6253");
    private static readonly Color PageInk = new("6b4a33");
    private static readonly Color Tape = new(0.92f, 0.86f, 0.67f, 0.75f);
    private static readonly Color ButtonFill = new("2b313b");
    private static readonly Color ButtonEdge = new("4a5260");

    // Where each polaroid sits on its page, and its tilt in degrees.
    private static readonly (Vector2 Pos, float Tilt)[] Slots =
    [
        (new(40, 80), -3f), (new(450, 110), 2.5f), (new(210, 410), -1.5f),
    ];

    private static readonly string[] Filters = ["ALL", "VICTORIES", "ELITES", "DEFEATS"];

    private static Font? _hand;
    /// <summary>Caveat (SIL Open Font License, fonts/Caveat-OFL.txt), for the handwritten captions.</summary>
    private static Font? Hand => _hand ??= ResourceLoader.Exists(MainFile.ResPath + "/fonts/Caveat.ttf")
        ? GD.Load<Font>(MainFile.ResPath + "/fonts/Caveat.ttf") : null;

    private static string Text(string key) => new LocString("powers", AlbumTracker.Loc + key).GetFormattedText();

    // ---------- Opening and closing ----------

    private sealed class State
    {
        public required Control Root;
        public required Control Book;
        public required Label PageLabel;
        public required List<Button> FilterButtons;
        public string Filter = "ALL";
        public int Spread;
    }

    public static void Open(Control parent)
    {
        var root = new Control { Name = "SpireBrothersAlbum", MouseFilter = Control.MouseFilterEnum.Stop };
        root.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        root.AddChild(new ColorRect { Color = Background, MouseFilter = Control.MouseFilterEnum.Ignore }.Tap(r => r.SetAnchorsPreset(Control.LayoutPreset.FullRect)));
        parent.AddChild(root);

        // Everything is laid out on a 1920 x 1080 stage, scaled to fit the window and centered.
        var stage = new Control { Size = Stage, MouseFilter = Control.MouseFilterEnum.Ignore };
        root.AddChild(stage);
        void Fit()
        {
            float scale = Math.Min(root.Size.X / Stage.X, root.Size.Y / Stage.Y);
            stage.Scale = Vector2.One * scale;
            stage.Position = (root.Size - Stage * scale) / 2;
        }
        root.Resized += Fit;
        Fit();

        var back = MakeButton(Text("BACK"), false);
        back.Position = new Vector2(40, 22);
        back.Size = new Vector2(130, 56);
        stage.AddChild(back);

        // Esc (and a controller's back button) closes the album. The game's hotkeys go to the newest binding, so ours
        // wins over the profile button behind it while the album is open; the do-nothing release keeps that button
        // from hearing the key come back up.
        string esc = MegaInput.pauseAndBack;
        Action ignore = () => { };
        bool closed = false;
        void Close()
        {
            if (closed) return;
            closed = true;
            NHotkeyManager.Instance?.RemoveHotkeyPressedBinding(esc, Close);
            NHotkeyManager.Instance?.RemoveHotkeyReleasedBinding(esc, ignore);
            root.QueueFree();
        }
        NHotkeyManager.Instance?.PushHotkeyPressedBinding(esc, Close);
        NHotkeyManager.Instance?.PushHotkeyReleasedBinding(esc, ignore);
        root.TreeExiting += Close;
        back.Pressed += Close;

        var title = new Label { Text = Text("TITLE"), Position = new Vector2(200, 14) };
        title.AddThemeFontSizeOverride("font_size", 56);
        title.AddThemeColorOverride("font_color", Gold);
        stage.AddChild(title);

        var book = new Control { Position = new Vector2(80, 120), Size = new Vector2(1760, 840) };
        stage.AddChild(book);

        var pageLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center, Position = new Vector2(760, 990), Size = new Vector2(400, 40) };
        pageLabel.AddThemeFontSizeOverride("font_size", 26);
        pageLabel.AddThemeColorOverride("font_color", new Color("d9cfbb"));
        stage.AddChild(pageLabel);

        var state = new State { Root = root, Book = book, PageLabel = pageLabel, FilterButtons = new() };

        float x = 1880;
        foreach (var filter in Filters.Reverse())
        {
            var button = MakeButton(Text("FILTER." + filter), filter == state.Filter);
            button.CustomMinimumSize = new Vector2(0, 52);
            stage.AddChild(button);
            button.ResetSize();
            x -= button.Size.X;
            button.Position = new Vector2(x, 24);
            x -= 12;
            button.Pressed += () => { state.Filter = filter; state.Spread = 0; Refresh(state); };
            state.FilterButtons.Insert(0, button);
        }

        var prev = MakeButton("<", false);
        prev.Position = new Vector2(690, 984);
        prev.Size = new Vector2(56, 52);
        prev.Pressed += () => { state.Spread--; Refresh(state); };
        stage.AddChild(prev);
        var next = MakeButton(">", false);
        next.Position = new Vector2(1174, 984);
        next.Size = new Vector2(56, 52);
        next.Pressed += () => { state.Spread++; Refresh(state); };
        stage.AddChild(next);

        root.GuiInput += input =>
        {
            if (input is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true }) Close();
        };

        Refresh(state);
        back.GrabFocus();
    }

    private static List<Snapshot> Shown(string filter) => Album.All.Reverse().Where(s => filter switch
    {
        "VICTORIES" => s.Kind is "VICTORY" or "BOSS",
        "ELITES" => s.Kind == "ELITE",
        "DEFEATS" => s.Kind == "DEFEAT",
        _ => true,
    }).ToList();

    private static void Refresh(State state)
    {
        var shots = Shown(state.Filter);
        int spreads = Math.Max(1, (shots.Count + PerSpread - 1) / PerSpread);
        state.Spread = Math.Clamp(state.Spread, 0, spreads - 1);
        for (int i = 0; i < Filters.Length; i++) Style(state.FilterButtons[i], Filters[i] == state.Filter);

        var pageText = new LocString("powers", AlbumTracker.Loc + "PAGE");
        pageText.Add("Page", state.Spread + 1);
        pageText.Add("Pages", spreads);
        state.PageLabel.Text = pageText.GetFormattedText();

        foreach (var child in state.Book.GetChildren()) child.QueueFree();
        BuildBook(state.Book, shots.Skip(state.Spread * PerSpread).Take(PerSpread).ToList(), shots.Count == 0);
    }

    // ---------- The book ----------

    private static void BuildBook(Control book, List<Snapshot> shots, bool empty)
    {
        book.AddChild(Box(Vector2.Zero, book.Size, Leather, 18, shadow: 24));
        const float pad = 22, spine = 14;
        float pageWidth = (book.Size.X - pad * 2 - spine) / 2, pageHeight = book.Size.Y - pad * 2;
        var left = Box(new Vector2(pad, pad), new Vector2(pageWidth, pageHeight), Paper, 6);
        var right = Box(new Vector2(pad + pageWidth + spine, pad), new Vector2(pageWidth, pageHeight), Paper, 6);
        book.AddChild(Box(new Vector2(pad + pageWidth, pad), new Vector2(spine, pageHeight), Spine, 3));
        book.AddChild(left);
        book.AddChild(right);

        if (empty)
        {
            var note = HandLabel(Text("EMPTY"), 40, PageInk);
            note.Position = new Vector2(60, 300);
            note.Size = new Vector2(pageWidth - 120, 200);
            note.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            note.HorizontalAlignment = HorizontalAlignment.Center;
            left.AddChild(note);
            return;
        }

        // A handwritten date at the top of each page, from its newest photo.
        for (int p = 0; p < 2; p++)
        {
            var onPage = shots.Skip(p * 3).Take(3).ToList();
            if (onPage.Count == 0) continue;
            var page = p == 0 ? left : right;
            var date = HandLabel(DateTimeOffset.FromUnixTimeSeconds(onPage[0].Time).LocalDateTime.ToString("MMMM d, yyyy"), 38, PageInk);
            date.Position = p == 0 ? new Vector2(40, 12) : new Vector2(pageWidth - 440, 12);
            date.Size = new Vector2(400, 50);
            date.HorizontalAlignment = p == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            page.AddChild(date);
            for (int i = 0; i < onPage.Count; i++) page.AddChild(Polaroid(onPage[i], Slots[i].Pos, Slots[i].Tilt));
        }
    }

    // ---------- One polaroid ----------

    private static readonly Vector2 PolaroidSize = new(360, 350);
    private static readonly Vector2 PhotoSize = new(332, 236);
    private const float Border = 14;
    private static readonly int[] CaptionSizes = [32, 30, 28, 26, 24];

    private static Control Polaroid(Snapshot shot, Vector2 position, float tilt)
    {
        var card = Box(position, PolaroidSize, PhotoPaper, 2, shadow: 10);
        card.PivotOffset = PolaroidSize / 2;
        card.RotationDegrees = tilt;

        // ClipChildren, not ClipContents: ClipContents clips to a screen-aligned box, which cuts a tilted photo crooked.
        var photo = new ColorRect
        {
            Position = new Vector2(Border, Border), Size = PhotoSize, Color = Colors.Black,
            ClipChildren = CanvasItem.ClipChildrenMode.AndDraw, MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        card.AddChild(photo);
        if (shot.Shot != null && LoadShot(shot.Shot) is { } texture)
        {
            photo.AddChild(new TextureRect
            {
                // ExpandMode before Size: until it's IgnoreSize, the texture's own size is the minimum and Size can't shrink it.
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                Texture = texture, Size = PhotoSize,
            });
        }
        else
        {
            DrawScene(photo, shot);
        }

        var (tagText, tagFill, tagInk) = shot.Kind switch
        {
            "DEFEAT" => (Text("TAG.DEFEAT"), new Color("b8432f"), Colors.White),
            "VICTORY" => (Text("TAG.VICTORY"), Gold, Ink),
            "BOSS" => (Text("TAG.BOSS"), Gold, Ink),
            _ => (Text("TAG.ELITE"), Gold, Ink),
        };
        var tag = new Label { Text = tagText };
        tag.AddThemeFontSizeOverride("font_size", 16);
        tag.AddThemeColorOverride("font_color", tagInk);
        tag.AddThemeStyleboxOverride("normal", new StyleBoxFlat { BgColor = tagFill, ContentMarginLeft = 8, ContentMarginRight = 8, ContentMarginTop = 2, ContentMarginBottom = 2 });
        photo.AddChild(tag);
        tag.ResetSize();
        tag.Position = new Vector2(PhotoSize.X - tag.Size.X - 8, 8);

        // Tape across the top edge.
        var tape = new ColorRect { Color = Tape, Size = new Vector2(96, 30), Position = new Vector2(PolaroidSize.X / 2 - 48, -14), MouseFilter = Control.MouseFilterEnum.Ignore };
        tape.PivotOffset = tape.Size / 2;
        tape.RotationDegrees = tilt * -1.4f;
        card.AddChild(tape);

        // Shrinks a long caption to fit on one line; one still too long at the smallest size wraps onto two.
        float captionWidth = PhotoSize.X - 4;
        int captionSize = CaptionSizes.FirstOrDefault(s => Hand == null || Hand.GetStringSize(shot.Caption, HorizontalAlignment.Left, -1, s).X <= captionWidth, CaptionSizes[^1]);
        var caption = HandLabel(shot.Caption, captionSize, Ink);
        caption.Position = new Vector2(Border + 2, Border + PhotoSize.Y + 4);
        caption.Size = new Vector2(captionWidth, 62);
        caption.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        caption.VerticalAlignment = VerticalAlignment.Center;
        caption.AddThemeConstantOverride("line_spacing", -8);
        card.AddChild(caption);

        var where = new LocString("powers", AlbumTracker.Loc + "DETAILS");
        where.Add("Act", shot.Act);
        where.Add("Floor", shot.Floor);
        var details = new Label { Text = where.GetFormattedText(), Position = new Vector2(Border + 2, PolaroidSize.Y - 32), Size = new Vector2(PhotoSize.X, 24) };
        details.AddThemeFontSizeOverride("font_size", 17);
        details.AddThemeColorOverride("font_color", FadedInk);
        card.AddChild(details);
        return card;
    }

    private static Texture2D? LoadShot(string file)
    {
        try
        {
            var image = Image.LoadFromFile($"{Album.ShotsDir}/{file}");
            return image == null ? null : ImageTexture.CreateFromImage(image);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Family Album: couldn't load screenshot {file}: {e}");
            return null;
        }
    }

    // ---------- Drawn scenes ----------

    // Sky and floor for each act (the overgrowth, the hive, the glory), and a dusky purple for victories.
    private static (Color Sky, Color Floor) Backdrop(Snapshot shot) => shot.Kind == "VICTORY"
        ? (new Color("5b3f6e"), new Color("3d2a4b"))
        : shot.Act switch
        {
            1 => (new Color("2f4a33"), new Color("23361f")),
            2 => (new Color("4a3a2a"), new Color("33281c")),
            _ => (new Color("2b3a55"), new Color("1d2840")),
        };

    private const float FloorHeight = 34, Margin = 12;
    /// <summary>Game units to photo pixels: a 400-tall brother is 144 px, unless someone tall needs it smaller.</summary>
    private const float MaxScale = 0.36f;
    /// <summary>How much of each body's width the next one starts after, so a party stands shoulder to shoulder.</summary>
    private const float Overlap = 0.62f;

    private static void DrawScene(Control photo, Snapshot shot)
    {
        var (sky, floor) = Backdrop(shot);
        photo.AddChild(new ColorRect { Color = sky, Size = PhotoSize, MouseFilter = Control.MouseFilterEnum.Ignore });
        photo.AddChild(new ColorRect { Color = floor, Position = new Vector2(0, PhotoSize.Y - FloorHeight), Size = new Vector2(PhotoSize.X, FloorHeight), MouseFilter = Control.MouseFilterEnum.Ignore });

        var party = Bodies(shot.Party);
        var foes = Bodies(shot.Foes);
        float tallest = party.Concat(foes).Select(b => b.Figure.Height).DefaultIfEmpty(400).Max();
        float scale = Math.Min(MaxScale, (PhotoSize.Y - FloorHeight - 30) / tallest);
        float floorY = PhotoSize.Y - FloorHeight + 12;

        // The party from the left, the foes from the right; squeezed closer if they don't fit.
        float Room(List<Body> bodies) => bodies.Sum(b => b.Footprint(scale) * Overlap) + (bodies.Count > 0 ? bodies[^1].Footprint(scale) * (1 - Overlap) : 0);
        float available = PhotoSize.X - Margin * 2;
        float squeeze = Math.Min(1, available / Math.Max(1, Room(party) + Room(foes) + (foes.Count > 0 ? 16 : 0)));

        float x = foes.Count == 0 ? (PhotoSize.X - Room(party) * squeeze) / 2 : Margin;
        foreach (var body in party)
        {
            photo.AddChild(body.Place(x, floorY, scale, mirrored: false));
            x += body.Footprint(scale) * Overlap * squeeze;
        }
        x = PhotoSize.X - Margin;
        foreach (var body in foes)
        {
            x -= body.Footprint(scale);
            photo.AddChild(body.Place(x, floorY, scale, mirrored: true));
            x += body.Footprint(scale) * (1 - Overlap * squeeze);
        }
    }

    private static List<Body> Bodies(List<PhotoFigure> figures) => figures
        .Where(f => ResourceLoader.Exists(f.Art))
        .Select(f => new Body(f, GD.Load<Texture2D>(f.Art)))
        .Where(b => b.Texture != null)
        .ToList();

    private sealed record Body(PhotoFigure Figure, Texture2D Texture)
    {
        /// <summary>On-photo size: the art scaled to its combat height, then to the photo.</summary>
        public Vector2 Size(float scale) => Texture.GetSize() * (Figure.Height / Texture.GetHeight()) * scale;

        private bool Lying => Figure.Fallen && Figure.TipOver;

        /// <summary>How much floor it takes up: its width standing, its height lying down.</summary>
        public float Footprint(float scale) => Lying ? Size(scale).Y : Size(scale).X;

        /// <summary>Stands it on the floor with its footprint starting at x. Fallen bodies lie with their head away from the fight.</summary>
        public Control Place(float x, float floorY, float scale, bool mirrored)
        {
            var size = Size(scale);
            var sprite = new TextureRect
            {
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.Scale,
                Texture = Texture, Size = size, FlipH = mirrored,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                PivotOffset = new Vector2(size.X / 2, size.Y),
            };
            if (Lying)
            {
                // Pivot at the feet; a quarter turn lays the body along the floor, head toward the photo's edge.
                sprite.RotationDegrees = mirrored ? 90 : -90;
                var feet = new Vector2(mirrored ? x : x + size.Y, floorY - size.X / 2);
                sprite.Position = feet - sprite.PivotOffset;
            }
            else
            {
                sprite.Position = new Vector2(x, floorY - size.Y);
                if (Figure.Fallen) sprite.Modulate = new Color(0.45f, 0.45f, 0.45f);
            }
            return sprite;
        }
    }

    // ---------- Small builders ----------

    private static Panel Box(Vector2 position, Vector2 size, Color fill, int radius, int shadow = 0)
    {
        var panel = new Panel { Position = position, Size = size, MouseFilter = Control.MouseFilterEnum.Ignore };
        var style = new StyleBoxFlat { BgColor = fill };
        style.SetCornerRadiusAll(radius);
        if (shadow > 0)
        {
            style.ShadowColor = new Color(0, 0, 0, 0.45f);
            style.ShadowSize = shadow;
            style.ShadowOffset = new Vector2(0, shadow / 2f);
        }
        panel.AddThemeStyleboxOverride("panel", style);
        return panel;
    }

    private static Label HandLabel(string text, int size, Color color)
    {
        var label = new Label { Text = text, MouseFilter = Control.MouseFilterEnum.Ignore };
        if (Hand != null) label.AddThemeFontOverride("font", Hand);
        label.AddThemeFontSizeOverride("font_size", size);
        label.AddThemeColorOverride("font_color", color);
        return label;
    }

    private static Button MakeButton(string text, bool active)
    {
        var button = new Button { Text = text, FocusMode = Control.FocusModeEnum.All };
        button.AddThemeFontSizeOverride("font_size", 24);
        Style(button, active);
        return button;
    }

    private static void Style(Button button, bool active)
    {
        StyleBoxFlat Make(Color fill, Color edge)
        {
            var style = new StyleBoxFlat { BgColor = fill, BorderColor = edge, ContentMarginLeft = 20, ContentMarginRight = 20, ContentMarginTop = 6, ContentMarginBottom = 6 };
            style.SetBorderWidthAll(2);
            style.SetCornerRadiusAll(26);
            return style;
        }
        var fill = active ? Gold : ButtonFill;
        var ink = active ? new Color("1b1f26") : new Color("f3ead6");
        button.AddThemeStyleboxOverride("normal", Make(fill, active ? Gold : ButtonEdge));
        button.AddThemeStyleboxOverride("hover", Make(active ? Gold : new Color("3a4250"), Gold));
        button.AddThemeStyleboxOverride("pressed", Make(Gold.Darkened(0.2f), Gold));
        button.AddThemeStyleboxOverride("focus", Make(Colors.Transparent, Gold));
        foreach (var name in new[] { "font_color", "font_hover_color", "font_focus_color" })
            button.AddThemeColorOverride(name, ink);
        button.AddThemeColorOverride("font_pressed_color", new Color("1b1f26"));
    }

    private static T Tap<T>(this T node, Action<T> action) where T : Node
    {
        action(node);
        return node;
    }
}

/// <summary>Adds "Family Album" to the main menu, right under Compendium (a copy of that button, wired to AlbumScreen).</summary>
[HarmonyPatch(typeof(NMainMenu), nameof(NMainMenu._Ready))]
public static class AlbumMenuButton
{
    // Duplicate without the copy keeping Compendium's click handler (Godot's Signals flag).
    private const int CopyFlags = (int)(Node.DuplicateFlags.Groups | Node.DuplicateFlags.Scripts | Node.DuplicateFlags.UseInstantiation);

    [HarmonyPostfix]
    private static void Postfix(NMainMenu __instance)
    {
        try
        {
            var compendium = __instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/CompendiumButton");
            var album = (NMainMenuTextButton)compendium.Duplicate(CopyFlags);
            album.Name = "SpireBrothersAlbumButton";
            compendium.AddSibling(album);
            album.GetChild<Label>(0).Text = new LocString("powers", AlbumTracker.Loc + "BUTTON").GetFormattedText();
            album.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => AlbumScreen.Open(__instance)));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Couldn't add the Family Album button: {e}");
        }
    }
}

/// <summary>"Saved to the Family Album" in the corner for a few seconds after a snapshot is taken.</summary>
public static class SnapshotToast
{
    private const float Hold = 2.5f, Fade = 0.4f;

    public static void Show()
    {
        if (Engine.GetMainLoop() is not SceneTree tree) return;
        var layer = new CanvasLayer { Layer = 120 };
        var label = new Label { Text = new LocString("powers", AlbumTracker.Loc + "SAVED").GetFormattedText(), Modulate = new Color(1, 1, 1, 0) };
        label.AddThemeFontSizeOverride("font_size", 26);
        label.AddThemeColorOverride("font_color", new Color("2a2420"));
        var style = new StyleBoxFlat { BgColor = new Color("fbf8f1"), ContentMarginLeft = 18, ContentMarginRight = 18, ContentMarginTop = 8, ContentMarginBottom = 8 };
        style.SetCornerRadiusAll(4);
        style.ShadowColor = new Color(0, 0, 0, 0.4f);
        style.ShadowSize = 8;
        label.AddThemeStyleboxOverride("normal", style);
        layer.AddChild(label);
        tree.Root.AddChild(layer);
        label.ResetSize();
        var screen = tree.Root.GetVisibleRect().Size;
        label.Position = new Vector2(screen.X - label.Size.X - 30, 110);
        label.RotationDegrees = -2;

        var tween = label.CreateTween();
        tween.TweenProperty(label, "modulate:a", 1f, Fade);
        tween.TweenInterval(Hold);
        tween.TweenProperty(label, "modulate:a", 0f, Fade);
        tween.TweenCallback(Callable.From(layer.QueueFree));
    }
}
