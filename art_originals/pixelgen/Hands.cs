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
        var c = new Canvas(GW, GH) { K = 2, Origin = new V(0, 0) };
        bool rolled = b.Forearm != null;          // Tim rolls his sleeves up
        bool ring = b.Name is "daniel" or "tim";  // the married brothers
        var skin = b.Skin;

        // forearm (bare for Tim; under the sleeve for everyone else, so only the wrist shows)
        c.Draw(q => S.Taper(q, new V(21, 104), new V(21, 40), 7.5f, 4.6f), skin, hi: skin.Light);
        DrawHand(c, move, skin, ring);

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
        FadeBottom(c);
        return c;
    }

    // Finger roots along the knuckles (index nearest the thumb, on the left) and how long each finger is when straight.
    static readonly V[] Root = [new(16.6f, 28), new(20.2f, 27), new(23.6f, 27.5f), new(26.8f, 29)];
    static readonly float[] Len = [15f, 17f, 15.5f, 11.5f];

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
        const int Start = 196, End = 236;
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
