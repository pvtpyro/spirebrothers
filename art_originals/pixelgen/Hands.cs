/// <summary>
/// The co-op treasure room hands: each player's arm reaches in from the edge of the screen, points while picking, then
/// throws rock, paper, or scissors. Vanilla's are 422 x 1200 with the hand at the top and the sleeve fading out at the
/// bottom, so these match: drawn on an 84 x 240 grid (a right hand, back toward the viewer, thumb on the left), scaled
/// 5x, and padded to 422 wide. Saved as out/&lt;name&gt;/hands/&lt;move&gt;.png, preview in out/hands_preview.png.
/// </summary>
public static class Hands
{
    const int GW = 84, GH = 240, Up = 5, OutW = 422;
    public static readonly string[] Moves = ["point", "rock", "paper", "scissors"];

    static readonly Col Nail = Col.Hex("f8dcc8");
    static readonly Col RingGold = Col.Hex("f0c040");

    public static void RenderAll(AnimatedBrother[] brothers, string outDir)
    {
        var bg = Col.Hex("5a5f6a");
        var sheet = new Canvas((GW + 4) * Moves.Length * brothers.Length, GH);
        for (int i = 0; i < sheet.Px.Length; i++) sheet.Px[i] = bg;
        for (int b = 0; b < brothers.Length; b++)
            for (int m = 0; m < Moves.Length; m++)
            {
                var c = Render(brothers[b], Moves[m]);
                Save(c, Path.Combine(outDir, brothers[b].Name, "hands", $"{Moves[m]}.png"));
                int ox = (b * Moves.Length + m) * (GW + 4);
                for (int y = 0; y < GH; y++) for (int x = 0; x < GW; x++) if (c.Get(x, y) is { } px) sheet.Set(ox + x, y, px);
            }
        sheet.SavePng(Path.Combine(outDir, "hands_preview.png"), 2);
    }

    /// <summary>Model space is 42 x 120 (K = 2 doubles it onto the 84 x 240 grid, keeping 1-pixel outlines).</summary>
    public static Canvas Render(Brother b, string move)
    {
        // The game puts the cursor at about (179, 11) of the 422 x 1200 image (scene hand_image.tscn: the TextureRect sits at
        // (-163, -10), 383 x 1072, keep-aspect-centered), so the drawing is nudged until the pointing fingertip lands there.
        var c = new Canvas(GW, GH) { K = 2, Origin = new V(0, 0), Offset = new V(3, -20) };
        bool rolled = b.Forearm != null;          // Tim rolls his sleeves up
        bool ring = b.Name is "daniel" or "tim";  // the married brothers
        var skin = b.Skin;

        // the friends: John's fingerless Hive Scum glove, Huffman's armored Arbites gauntlet
        Col? glove = b switch { JohnArt => Col.Hex("2a2a2e"), HuffmanArt => Col.Hex("33363e"), _ => null };
        bool gauntlet = b is HuffmanArt;

        // forearm (bare for Tim; under the sleeve for everyone else, so only the wrist shows)
        c.Draw(q => S.Taper(q, new V(21, 104), new V(21, 40), 7.5f, 4.6f), skin, hi: skin.Light);
        DrawHand(c, move, skin, ring, glove, gauntlet);

        if (rolled)
        {
            // short sleeve rolled above the elbow, with a thick rolled band
            c.Draw(q => S.Poly(q, new V(11, 122), new V(12.5f, 80), new V(29.5f, 80), new V(31, 122)), b.Sleeve, hi: b.Sleeve.Light);
            c.Draw(q => S.Box(q, 11.5f, 76, 30.5f, 83) - 1, b.Sleeve.Light, shade: b.Sleeve);
            c.Flat(q => S.Box(q, 12.5f, 79.2f, 29.5f, 79.8f), b.Sleeve.Shade);
        }
        else
        {
            // full sleeve up to the wrist, with a cuff
            c.Draw(q => S.Poly(q, new V(11, 122), new V(15.5f, 50), new V(26.5f, 50), new V(31, 122)), b.Sleeve, hi: b.Sleeve.Light);
            if (b is JoshuaArt)
                c.Pattern(q => S.Poly(q, new V(11, 122), new V(15.5f, 50), new V(26.5f, 50), new V(31, 122)),
                    (x, y) => (x % 8 == 0) ^ (y % 8 == 0) ? b.Sleeve.Shade : null);
            c.Draw(q => S.Box(q, 15, 44, 27, 52) - 0.6f, b.Sleeve.Shade, shade: b.Sleeve.Shade.Shade);
            if (b is DavidArt) for (float y = 45.5f; y < 51; y += 1.5f) c.Flat(q => S.Box(q, 15.8f, y, 26.2f, y + 0.4f), b.Sleeve.Shade.Shade);
        }
        if (b is JohnArt) StimmInjector(c);
        if (gauntlet) GauntletCuff(c, glove!.Value);
        FadeBottom(c);
        return c;
    }

    // Finger roots along the knuckles (index nearest the thumb, on the left) and how long each finger is when straight.
    static readonly V[] Root = [new(16.6f, 28), new(20.2f, 27), new(23.6f, 27.5f), new(26.8f, 29)];
    static readonly float[] Len = [15f, 14.2f, 13.8f, 10.5f];  // middle kept short enough to fit under the top edge

    static readonly Col Steel = Col.Hex("9aa2ae");

    /// <summary>
    /// A glove over the hand: <paramref name="gauntlet"/> covers the whole hand (armor plates, no nails); otherwise it's
    /// fingerless, so the fingertips show. Drawn over the bare hand so the skin underneath shapes the fingertips.
    /// </summary>
    static void DrawHand(Canvas c, string move, Col skin, bool ring, Col? glove, bool gauntlet)
    {
        DrawHand(c, move, skin, ring && glove == null);
        if (glove is not { } g) return;
        (bool up, float lean)[] f = Fingers(move);
        bool open = move == "paper";

        // thumb: the whole thing in a gauntlet, just the base in a fingerless glove
        float thumbLen = gauntlet ? 1f : 0.55f;
        if (open) c.Draw(q => S.Taper(q, new V(15, 38), new V(15, 38) + (new V(8.5f, 27.5f) - new V(15, 38)) * thumbLen, 2.7f, 2.0f), g, hi: g.Light);
        else c.Draw(q => S.Taper(q, new V(15.5f, 39), new V(15.5f, 39) + (new V(13, 29.5f) - new V(15.5f, 39)) * thumbLen, 2.7f, 2.2f), g, hi: g.Light);

        // back of the hand, with a steel plate on the gauntlet and a bare knuckle strip for the Hive Scum glove
        c.Draw(q => S.RBox(q, new V(21.5f, 34.5f), 6.8f, 7.6f, 0) - 1.2f, g, hi: g.Light);
        if (gauntlet)
        {
            c.Draw(q => S.RBox(q, new V(21.5f, 35.5f), 4.6f, 4.4f, 0) - 0.8f, Steel, hi: Steel.Light);
            c.Plot(18.5f, 33, g.Shade); c.Plot(24.5f, 33, g.Shade); c.Plot(18.5f, 38, g.Shade); c.Plot(24.5f, 38, g.Shade);   // rivets
        }

        for (int i = 0; i < 4; i++)
        {
            var root = Root[i];
            if (f[i].up)
            {
                var tip = root + new V(0, -Len[i]).Rot(f[i].lean);
                var d = (tip - root).Norm;
                float r = i == 3 ? 1.55f : 1.8f;
                var end = gauntlet ? tip : root + d * (Len[i] * 0.45f);
                c.Draw(q => S.Capsule(q, root, end, r), g, hi: g.Light);
                if (gauntlet)
                {
                    // segmented plates on each finger
                    for (float t = 0.33f; t < 0.95f; t += 0.33f) c.Flat(q => S.Capsule(q, root + d * (Len[i] * t) - d.Perp * 1.2f, root + d * (Len[i] * t) + d.Perp * 1.2f, 0.3f), Steel);
                }
            }
            else
            {
                c.Draw(q => S.Circle(q, new V(root.X, 26.2f + (i == 3 ? 1 : 0)), 2.05f), g, hi: g.Light);
                if (gauntlet) c.Plot(root.X, 25.5f + (i == 3 ? 1 : 0), Steel);
            }
        }
        // knuckle line
        c.Flat(q => S.Box(q, 16, 30.5f, 27.5f, 31), g.Shade);
    }

    static (bool up, float lean)[] Fingers(string move) => move switch
    {
        "point" => [(true, -2), (false, 0), (false, 0), (false, 0)],
        "scissors" => [(true, -11), (true, 9), (false, 0), (false, 0)],
        "paper" => [(true, -10), (true, -1), (true, 8), (true, 18)],
        _ => [(false, 0), (false, 0), (false, 0), (false, 0)],
    };

    /// <summary>John: a Hive Scum stimm injector strapped to his bare forearm, with a glowing green chem window.</summary>
    static void StimmInjector(Canvas c)
    {
        var strap = Col.Hex("3a2a20");
        c.Draw(q => S.Box(q, 13.5f, 54, 28.5f, 56.5f), strap, hi: strap.Light);
        c.Draw(q => S.Box(q, 14, 66, 28, 68.5f), strap, hi: strap.Light);
        c.Draw(q => S.Capsule(q, new V(24, 51), new V(24, 71), 2.6f), Steel, hi: Steel.Light);           // the injector
        c.Flat(q => S.Capsule(q, new V(24, 57.5f), new V(24, 64.5f), 1.2f), Col.Hex("70ff80"));            // the chem
        c.Plot(23.5f, 59, Col.Hex("d8ffe0"));
        c.Draw(q => S.Box(q, 22.5f, 48.5f, 25.5f, 51), Col.Hex("5a5e68"));                                // plunger cap
    }

    /// <summary>Huffman: the gauntlet's flared armored cuff, worn over his shirt sleeve.</summary>
    static void GauntletCuff(Canvas c, Col g)
    {
        c.Draw(q => S.Poly(q, new V(14.5f, 41), new V(27.5f, 41), new V(30, 56), new V(12, 56)), g, hi: g.Light);
        c.Draw(q => S.Box(q, 13, 47, 29, 50), Steel, hi: Steel.Light);
        c.Plot(15, 48.5f, g.Shade); c.Plot(21, 48.5f, g.Shade); c.Plot(27, 48.5f, g.Shade);
        c.Plot(21, 44, Col.Hex("d4af37")); c.Plot(20, 44, Col.Hex("d4af37")); c.Plot(22, 44, Col.Hex("d4af37"));   // Arbites eagle, a gold glint
    }

    static void DrawHand(Canvas c, string move, Col skin, bool ring)
    {
        // which fingers are straight, and the angle each one leans (degrees from straight up; negative leans left)
        (bool up, float lean)[] f = move switch
        {
            "point" => [(true, -2), (false, 0), (false, 0), (false, 0)],
            "scissors" => [(true, -11), (true, 9), (false, 0), (false, 0)],
            "paper" => [(true, -10), (true, -1), (true, 8), (true, 18)],
            _ => [(false, 0), (false, 0), (false, 0), (false, 0)],
        };
        bool open = move == "paper";

        // thumb: sticks out to the side when the hand is open, tucked along the side of the fist otherwise
        if (open) c.Draw(q => S.Taper(q, new V(15, 38), new V(8.5f, 27.5f), 2.6f, 1.9f), skin, hi: skin.Light);
        else c.Draw(q => S.Taper(q, new V(15.5f, 39), new V(13, 29.5f), 2.6f, 2.1f), skin, hi: skin.Light);

        // back of the hand
        c.Draw(q => S.RBox(q, new V(21.5f, 34.5f), 6.6f, 7.4f, 0) - 1.2f, skin, hi: skin.Light);

        for (int i = 0; i < 4; i++)
        {
            var root = Root[i];
            if (f[i].up)
            {
                var tip = root + new V(0, -Len[i]).Rot(f[i].lean);
                float r = i == 3 ? 1.45f : 1.7f;
                c.Draw(q => S.Capsule(q, root, tip, r), skin, hi: skin.Light);
                // knuckle creases and the nail at the tip
                var d = (tip - root).Norm;
                c.Plot(root + d * (Len[i] * 0.45f), skin.Shade);
                c.Plot(root + d * (Len[i] * 0.45f) + d.Perp * 0.5f, skin.Shade);
                c.Flat(q => S.Capsule(q, tip - d * 2.6f, tip - d * 1.1f, 0.75f), Nail);
            }
            else
            {
                // curled finger: a rounded knuckle bump on top of the fist
                c.Draw(q => S.Circle(q, new V(root.X, 26.2f + (i == 3 ? 1 : 0)), 1.95f), skin, hi: skin.Light);
            }
        }
        // creases between curled fingers
        for (int i = 0; i < 3; i++)
            if (!f[i].up && !f[i + 1].up)
                c.Flat(q => S.Box(q, (Root[i].X + Root[i + 1].X) / 2 - 0.25f, 26.5f, (Root[i].X + Root[i + 1].X) / 2 + 0.25f, 29.5f), skin.Shade);
        // knuckle line across the back of the hand
        c.Flat(q => S.Box(q, 16, 30.5f, 27.5f, 31), skin.Shade);

        // wedding ring on the ring finger
        if (ring)
        {
            var root = Root[2];
            var at = f[2].up ? root + new V(0, -3.2f).Rot(f[2].lean) : new V(root.X, 28.6f);
            c.Draw(q => S.Box(q, at.X - 1.7f, at.Y - 0.6f, at.X + 1.7f, at.Y + 0.6f), RingGold, outline: false, hi: Col.Hex("fff0a0"));
        }
    }

    /// <summary>The bottom of the sleeve dissolves into the background with an ordered dither, like vanilla's soft fade.</summary>
    static void FadeBottom(Canvas c)
    {
        int[,] bayer = { { 0, 8, 2, 10 }, { 12, 4, 14, 6 }, { 3, 11, 1, 9 }, { 15, 7, 13, 5 } };
        const int Start = 176, End = 216;  // shifted up with the drawing (see Offset)
        for (int y = Start; y < GH; y++)
        {
            float t = Math.Clamp((y - Start) / (float)(End - Start), 0, 1);
            for (int x = 0; x < GW; x++)
                if (t * 16 > bayer[y % 4, x % 4]) c.Px[y * GW + x] = null;
        }
    }

    static void Save(Canvas c, string path)
    {
        // 84 x 240 scaled 5x is 420 x 1200; pad a transparent column on each side to match vanilla's 422.
        int pad = (OutW - GW * Up) / 2, h = GH * Up;
        var raw = new byte[h * (OutW * 4 + 1)];
        for (int y = 0; y < h; y++)
        {
            int row = y * (OutW * 4 + 1);
            for (int x = 0; x < GW * Up; x++)
            {
                if (c.Px[(y / Up) * GW + x / Up] is not { } px) continue;
                int o = row + 1 + (x + pad) * 4;
                raw[o] = px.R; raw[o + 1] = px.G; raw[o + 2] = px.B; raw[o + 3] = px.A;
            }
        }
        Png.Write(path, OutW, h, raw);
    }
}
