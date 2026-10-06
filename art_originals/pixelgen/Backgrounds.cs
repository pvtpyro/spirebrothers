// Character select backgrounds: one pixel-art scene per brother, 480 x 270 (shown 4x on a 1080p screen).
// Each is original art that nods at his favorite games without copying anything from them, with the brother
// standing big on the right. The left side is darkened so the game's name/description text stays readable.

public static class Backgrounds
{
    public const int W = 480, H = 270;

    public static void RenderAll(IEnumerable<AnimatedBrother> brothers, string outDir)
    {
        foreach (var b in brothers)
        {
            var c = new Canvas(W, H) { Origin = new V(0, 0) };
            switch (b.Name)
            {
                case "daniel": Daniel(c); break;
                case "david": David(c); break;
                case "joshua": Joshua(c); break;
                case "tim": Tim(c); break;
            }
            StandBrother(c, b, 345, 246);
            Vignette(c);
            c.SavePng(Path.Combine(outDir, b.Name, "select_bg.png"));
            c.SavePng(Path.Combine(outDir, $"select_bg_{b.Name}_preview.png"), 2);
        }
    }

    // ---------------------------------------------------------------- shared helpers

    static void Fill(Canvas c, Func<int, int, Col?> f)
    {
        for (int y = 0; y < c.H; y++) for (int x = 0; x < c.W; x++) if (f(x, y) is { } col) c.Set(x, y, col);
    }

    static void Blend(Canvas c, int x, int y, Col col, float a)
    {
        if (c.Get(x, y) is { } o) c.Set(x, y, o.Lerp(col, Math.Clamp(a, 0, 1)));
    }

    /// <summary>Vertical gradient through the given stops, in pixel-art bands with a checker dither between them.</summary>
    static void Sky(Canvas c, int bottom, params Col[] stops)
    {
        const int bands = 14;
        for (int y = 0; y < bottom; y++)
        {
            float t = (float)y / bottom * (stops.Length - 1);
            float band = MathF.Floor(t * bands) / bands;
            float next = band + 1f / bands;
            bool dither = (t * bands) % 1 > 0.6f;
            for (int x = 0; x < c.W; x++)
            {
                float u = dither && (x + y) % 2 == 0 ? next : band;
                int i = Math.Min((int)u, stops.Length - 2);
                c.Set(x, y, stops[i].Lerp(stops[i + 1], Math.Clamp(u - i, 0, 1)));
            }
        }
    }

    static void Stars(Canvas c, int seed, int count, int maxY, Col col)
    {
        var rng = new Random(seed);
        for (int i = 0; i < count; i++)
        {
            int x = rng.Next(c.W), y = rng.Next(maxY);
            c.Set(x, y, rng.Next(4) == 0 ? Col.Hex("ffffff") : col);
            if (rng.Next(9) == 0) { c.Set(x + 1, y, col.Mul(0.7f)); c.Set(x - 1, y, col.Mul(0.7f)); c.Set(x, y + 1, col.Mul(0.7f)); c.Set(x, y - 1, col.Mul(0.7f)); }
        }
    }

    /// <summary>A ridge line filled down to the bottom: height(x) = base minus a few sine bumps.</summary>
    static void Ridge(Canvas c, float baseY, float amp, float freq, int seed, Col col, Col? top = null)
    {
        var rng = new Random(seed);
        float p1 = rng.NextSingle() * 6, p2 = rng.NextSingle() * 6;
        for (int x = 0; x < c.W; x++)
        {
            float h = baseY - amp * (0.6f * MathF.Sin(x * freq + p1) + 0.4f * MathF.Sin(x * freq * 2.3f + p2));
            for (int y = (int)h; y < c.H; y++) c.Set(x, y, col);
            if (top is { } t) { c.Set(x, (int)h, t); c.Set(x, (int)h + 1, t); }
        }
    }

    /// <summary>Square block with a light top edge and dark outline, like a voxel seen from the side.</summary>
    static void Block(Canvas c, int x, int y, int s, Col body, Col? topCol = null, int topH = 3)
    {
        for (int j = 0; j < s; j++) for (int i = 0; i < s; i++)
        {
            Col col = j < topH && topCol is { } tc ? tc : body;
            if (i == 0 || j == 0) col = col.Light;
            if (i == s - 1 || j == s - 1) col = col.Mul(0.72f);
            c.Set(x + i, y + j, col);
        }
    }

    static void Note(Canvas c, V at, Col col)
    {
        c.Draw(q => S.Ellipse(q, at, 3.2f, 2.5f), col);
        c.Draw(q => S.Capsule(q, at + new V(2.6f, 0), at + new V(2.6f, -11), 0.8f), col, noShade: true);
        c.Draw(q => S.Capsule(q, at + new V(2.6f, -11), at + new V(6.5f, -7), 0.8f), col, noShade: true);
    }

    static void Ball(Canvas c, V at, float r, Col body, Col patch)
    {
        c.Draw(q => S.Circle(q, at, r), body, hi: body.Light);
        foreach (var d in new[] { new V(0, 0), new V(r * 0.62f, -r * 0.3f), new V(-r * 0.55f, -r * 0.4f), new V(-r * 0.2f, r * 0.62f), new V(r * 0.5f, r * 0.45f) })
            c.Flat(q => S.I(S.Circle(q, at + d, r * 0.22f), S.Circle(q, at, r - 1.5f)), patch);
    }

    /// <summary>The brother's first idle frame, 2x, feet at (cx, feetY), with a soft ground shadow.</summary>
    static void StandBrother(Canvas c, AnimatedBrother b, int cx, int feetY)
    {
        var frame = b.Render(b.Anims()["idle"].poses[0]);
        const int K = 2;
        for (int y = -3; y <= 3; y++) for (int x = -36; x <= 36; x++)
            if (x * x / (36f * 36f) + y * y / 9f <= 1) Blend(c, cx + x + 4, feetY + y, new Col(0, 0, 0), 0.4f);
        for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++)
        {
            if (frame.Get(x, y) is not { } px) continue;
            for (int j = 0; j < K; j++) for (int i = 0; i < K; i++)
                c.Set(cx + (x - (int)Rig.Cx) * K + i, feetY - (80 - y) * K + j, px);
        }
    }

    /// <summary>Darkens the left side (where the game prints the name and description) and the very bottom (buttons).</summary>
    static void Vignette(Canvas c)
    {
        for (int y = 0; y < c.H; y++) for (int x = 0; x < c.W; x++)
        {
            float left = Math.Clamp(1 - x / 210f, 0, 1);
            float bottom = Math.Clamp((y - 220) / 50f, 0, 1);
            float a = MathF.Max(left * left * 0.62f, bottom * 0.45f);
            if (a > 0) Blend(c, x, y, Col.Hex("0a0810"), a);
        }
    }

    static Col C(string hex) => Col.Hex(hex);

    // ---------------------------------------------------------------- Daniel: sci-fi factory world at night

    static void Daniel(Canvas c)
    {
        Sky(c, 200, C("071422"), C("0f2c3c"), C("1f5560"));
        Stars(c, 11, 140, 150, C("9fdcee"));
        // ringed planet
        var planet = new V(150, 56);
        c.Draw(q => S.Circle(q, planet, 24), C("2e7f8c"), hi: C("5fc0c8"));
        c.Flat(q => S.I(MathF.Abs(S.Ellipse(q, planet, 42, 7)) - 1.2f, q.Y - planet.Y), C("a8e6ea"));
        // far ridge
        Ridge(c, 178, 10, 0.03f, 3, C("0c2230"), C("164050"));
        // factory silhouette with lit windows and smoke
        var wall = C("0a1820");
        foreach (var (x0, w, h) in new[] { (20, 70, 46), (90, 50, 30), (140, 80, 58), (220, 40, 36) })
        {
            for (int y = 196 - h; y < 196; y++) for (int x = x0; x < x0 + w; x++) c.Set(x, y, wall);
            for (int wy = 196 - h + 6; wy < 190; wy += 9) for (int wx = x0 + 5; wx < x0 + w - 5; wx += 9)
                if ((wx * 7 + wy * 3) % 5 != 0) { c.Set(wx, wy, C("f2c84a")); c.Set(wx + 1, wy, C("f2c84a")); c.Set(wx, wy + 1, C("c89a30")); c.Set(wx + 1, wy + 1, C("c89a30")); }
        }
        foreach (var sx in new[] { 40, 60, 170, 196 })
        {
            for (int y = 120; y < 150; y++) for (int x = sx; x < sx + 6; x++) c.Set(x, y, wall);
            for (int i = 0; i < 5; i++) c.Flat(q => S.Circle(q, new V(sx + 3 + i * 5, 112 - i * 9), 4 + i * 1.3f), C("2a4450").Lerp(C("1f5560"), i * 0.15f));
        }
        // orbital strike: a red beam from the sky, glowing where it lands
        for (int y = 0; y < 214; y++) for (int x = 404; x < 436; x++)
        {
            float d = MathF.Abs(x - 420);
            if (d < 2) c.Set(x, y, C("fff2e0"));
            else if (d < 5) c.Set(x, y, C("ff5a3a"));
            else Blend(c, x, y, C("ff3020"), (1 - d / 16f) * 0.35f);
        }
        c.Flat(q => S.Ellipse(q, new V(420, 212), 30, 6), C("ff7a40"));
        c.Flat(q => S.Ellipse(q, new V(420, 212), 18, 3), C("fff0c0"));
        // a falling drop pod streak
        for (int i = 0; i < 34; i++) { c.Set(232 + i, 6 + i * 2, C("ffd080")); c.Set(233 + i, 6 + i * 2, C("ff8040")); }
        c.Draw(q => S.Circle(q, new V(268, 76), 4), C("c8d0d8"));
        // big background gears
        Gear(c, new V(60, 222), 26, C("1b4650"));
        Gear(c, new V(250, 232), 18, C("1b4650"));
        // conveyor belts with items riding them
        Belt(c, 202, 0); Belt(c, 226, 5);
        // blocky ground
        for (int x = 0; x < c.W; x += 12) { Block(c, x, 246, 12, C("6b4a2b"), C("4f9a52")); Block(c, x, 258, 12, C("5a3d24")); }
    }

    static void Gear(Canvas c, V at, float r, Col col)
    {
        c.Flat(q =>
        {
            var d = q - at; float a = MathF.Atan2(d.Y, d.X);
            float teeth = MathF.Sin(a * 10) > 0.2f ? 4 : 0;
            return S.Sub(d.Len - (r + teeth), S.Circle(q, at, r * 0.35f));
        }, col);
    }

    static void Belt(Canvas c, int y, int shift)
    {
        for (int x = 0; x < c.W; x++)
        {
            c.Set(x, y, C("2b2f36")); c.Set(x, y + 9, C("15181c"));
            for (int j = 1; j < 9; j++) c.Set(x, y + j, C("3a3f48"));
            if ((x + shift) % 10 < 2) for (int j = 3; j < 7; j++) c.Set(x + (j % 2), y + j, C("e0b020"));
        }
        Col[] items = [C("d0763a"), C("9aa4b0"), C("46c060"), C("d0763a")];
        for (int x = 8 + shift * 3, i = 0; x < c.W; x += 26, i++)
            c.Draw(q => S.Box(q, x, y - 6, x + 7, y), items[i % items.Length]);
    }

    // ---------------------------------------------------------------- David: dusk hike over a blocky world

    static void David(Canvas c)
    {
        Sky(c, 205, C("2a1e48"), C("7a3a5a"), C("e0784a"), C("f2b060"));
        c.Flat(q => S.Circle(q, new V(250, 150), 34), C("ffd890"));
        c.Flat(q => S.Circle(q, new V(250, 150), 26), C("fff0c0"));
        Stars(c, 5, 50, 60, C("e8d8ff"));
        // snowy mountains
        Ridge(c, 150, 34, 0.018f, 8, C("4a3a5e"), C("e8e4f0"));
        Ridge(c, 172, 18, 0.03f, 9, C("2f4a3a"), C("3f6a4a"));
        // a long-neck dinosaur on the ridge
        var dino = C("1a2a22");
        c.Flat(q => S.Ellipse(q, new V(400, 150), 18, 9), dino);
        c.Flat(q => S.Taper(q, new V(412, 146), new V(432, 108), 5, 2.5f), dino);
        c.Flat(q => S.Ellipse(q, new V(436, 106), 6, 3), dino);
        c.Flat(q => S.Taper(q, new V(384, 152), new V(352, 162), 5, 0.8f), dino);
        foreach (var lx in new[] { 390, 397, 405, 412 }) c.Flat(q => S.Capsule(q, new V(lx, 154), new V(lx, 168), 2.2f), dino);
        // blocky pine trees
        foreach (var tx in new[] { 18, 70, 296, 456 }) Pine(c, tx, 196);
        // surface: grass blocks, cut open below to show stone and ore
        var rng = new Random(4);
        for (int x = 0; x < c.W; x += 12)
        {
            Block(c, x, 196, 12, C("6b4a2b"), C("4f9a52"), 4);
            for (int y = 208; y < c.H; y += 12)
            {
                bool cave = x > 120 && x < 230 && y > 222 && y < 250;
                if (cave) { for (int j = 0; j < 12; j++) for (int i = 0; i < 12; i++) c.Set(x + i, y + j, C("1a1620")); continue; }
                Block(c, x, y, 12, y < 220 ? C("6b4a2b") : C("6e6a78"));
                int ore = rng.Next(9);
                if (y >= 220 && ore < 2)
                {
                    var oc = ore == 0 ? C("f0c040") : C("60e0e0");
                    foreach (var (i, j) in new[] { (3, 3), (4, 3), (7, 6), (8, 6), (4, 8), (8, 3) }) c.Set(x + i, y + j, oc);
                }
            }
        }
        // tent and campfire (he hikes and camps)
        c.Draw(q => S.Poly(q, new V(120, 196), new V(146, 160), new V(172, 196)), C("d06a3a"), hi: C("f09060"));
        c.Flat(q => S.Poly(q, new V(140, 196), new V(146, 176), new V(152, 196)), C("3a2018"));
        c.Draw(q => S.Capsule(q, new V(186, 194), new V(200, 192), 1.6f), C("6b4a2b"));
        c.Flat(q => S.Taper(q, new V(193, 191), new V(193, 178), 5, 0.5f), C("ff8a20"));
        c.Flat(q => S.Taper(q, new V(193, 191), new V(193, 183), 2.6f, 0.3f), C("ffe060"));
        // a loot bag and a stack of gold (he never spends it)
        c.Draw(q => S.U(S.Circle(q, new V(232, 188), 8), S.Box(q, 229, 176, 235, 181)), C("c8b48a"), hi: C("e8d8b0"));
        c.Plot(231, 188, C("3a3020")); c.Plot(232, 187, C("3a3020")); c.Plot(232, 189, C("3a3020")); c.Plot(233, 188, C("3a3020"));
        for (int i = 0; i < 4; i++) c.Draw(q => S.Ellipse(q, new V(252, 192 - i * 3), 6, 2), C("f0c040"), hi: C("fff0a0"));
    }

    static void Pine(Canvas c, int x, int ground)
    {
        for (int y = ground - 10; y < ground; y++) { c.Set(x + 4, y, C("4a2e1c")); c.Set(x + 5, y, C("4a2e1c")); }
        for (int k = 0; k < 4; k++)
        {
            int w = 18 - k * 4, top = ground - 16 - k * 9;
            for (int y = top; y < top + 9; y++) for (int i = 0; i < w; i++) c.Set(x + 5 - w / 2 + i, y, k % 2 == 0 ? C("2e6a3a") : C("3a7e44"));
        }
    }

    // ---------------------------------------------------------------- Joshua: sunset farm, a mountain, music

    static void Joshua(Canvas c)
    {
        Sky(c, 200, C("2a1a4a"), C("6a3a8a"), C("d86a8a"), C("f6b08a"));
        c.Flat(q => S.Circle(q, new V(300, 168), 40), C("ffc89a"));
        Stars(c, 9, 40, 50, C("f0d8ff"));
        // a snowcapped volcano
        c.Flat(q => S.Poly(q, new V(60, 190), new V(170, 92), new V(196, 92), new V(320, 190)), C("4a3a7a"));
        c.Flat(q => S.Poly(q, new V(150, 112), new V(170, 92), new V(196, 92), new V(218, 112), new V(204, 108), new V(192, 116), new V(178, 106), new V(164, 116)), C("f4eefc"));
        Ridge(c, 190, 8, 0.025f, 2, C("3a5a3a"), C("4a7a44"));
        // a red gate on the hill
        var red = C("d23a3a");
        c.Draw(q => S.U(S.Box(q, 92, 160, 96, 192), S.Box(q, 122, 160, 126, 192)), red);
        c.Draw(q => S.Box(q, 84, 154, 134, 158), red);
        c.Draw(q => S.Box(q, 88, 164, 130, 167), red);
        c.Flat(q => S.Box(q, 82, 151, 136, 154), C("2a2030"));
        // cherry blossom tree
        c.Draw(q => S.Capsule(q, new V(236, 194), new V(238, 150), 3), C("4a2e1c"));
        c.Draw(q => S.Capsule(q, new V(238, 162), new V(252, 148), 1.6f), C("4a2e1c"));
        var rng = new Random(3);
        for (int i = 0; i < 26; i++)
        {
            var at = new V(240 + rng.Next(-26, 26), 140 + rng.Next(-16, 12));
            c.Flat(q => S.Circle(q, at, 6 + rng.Next(4)), i % 3 == 0 ? C("f8c8e0") : C("f0a0c8"));
        }
        for (int i = 0; i < 30; i++) c.Set(rng.Next(190, 300), rng.Next(120, 240), C("f8c8e0"));
        // farm rows with sprouts
        for (int y = 200; y < c.H; y++) for (int x = 0; x < c.W; x++)
            c.Set(x, y, ((y - 200) / 6) % 2 == 0 ? C("6b4228") : C("54321e"));
        for (int row = 0; row < 10; row++) for (int x = 6 + row * 3 % 12; x < c.W; x += 16)
        {
            int y = 203 + row * 6;
            c.Set(x, y, C("6ad050")); c.Set(x + 1, y - 1, C("6ad050")); c.Set(x - 1, y - 1, C("4aa040")); c.Set(x, y - 2, C("8ae070"));
        }
        // a giant ball flying across the sky
        for (int i = 0; i < 46; i++) { c.Set(380 + i, 52 + i / 3, C("ffffff").Lerp(C("f6b08a"), i / 46f)); c.Set(380 + i, 58 + i / 3, C("ffffff").Lerp(C("f6b08a"), i / 46f)); }
        Ball(c, new V(368, 52), 16, C("e8e8ee"), C("3a3a48"));
        // floating notes
        Note(c, new V(214, 70), C("ffd860"));
        Note(c, new V(236, 50), C("ffffff"));
        Note(c, new V(256, 78), C("ffd860"));
    }

    // ---------------------------------------------------------------- Tim: castle at dusk, blueprints, bricks

    static void Tim(Canvas c)
    {
        Sky(c, 205, C("101a3a"), C("2a3a6a"), C("c86a4a"), C("f0a060"));
        Stars(c, 6, 60, 70, C("dce8ff"));
        // blueprint grid across the sky, with a dimension line
        for (int y = 0; y < 150; y++) for (int x = 0; x < c.W; x++)
            if (x % 16 == 0 || y % 16 == 0) Blend(c, x, y, C("9ac8ff"), 0.13f);
        for (int x = 230; x < 330; x++) c.Set(x, 24, C("cfe4ff"));
        for (int y = 20; y < 29; y++) { c.Set(230, y, C("cfe4ff")); c.Set(329, y, C("cfe4ff")); }
        for (int i = 0; i < 3; i++) { c.Set(231 + i, 23 - i, C("cfe4ff")); c.Set(231 + i, 25 + i, C("cfe4ff")); c.Set(328 - i, 23 - i, C("cfe4ff")); c.Set(328 - i, 25 + i, C("cfe4ff")); }
        // hill with a castle
        Ridge(c, 182, 22, 0.012f, 6, C("3a5a3a"), C("5a8a4a"));
        var stone = C("8a8a96");
        c.Draw(q => S.Box(q, 110, 120, 220, 168), stone, hi: stone.Light);
        foreach (var tx in new[] { 100, 152, 210 })
        {
            int top = tx == 152 ? 84 : 104;
            c.Draw(q => S.Box(q, tx, top, tx + 20, 168), stone, hi: stone.Light);
            for (int k = 0; k < 3; k++) c.Draw(q => S.Box(q, tx + k * 8, top - 6, tx + k * 8 + 4, top), stone);
            c.Flat(q => S.Box(q, tx + 8, top + 12, tx + 12, top + 20), C("2a2030"));
        }
        for (int k = 0; k < 7; k++) c.Draw(q => S.Box(q, 122 + k * 14, 114, 128 + k * 14, 120), stone);
        c.Flat(q => S.Poly(q, new V(150, 168), new V(150, 150), new V(162, 144), new V(174, 150), new V(174, 168)), C("3a2418"));
        c.Draw(q => S.Capsule(q, new V(162, 84), new V(162, 62), 0.8f), C("4a3020"), noShade: true);
        c.Draw(q => S.Poly(q, new V(163, 62), new V(180, 66), new V(163, 71)), C("d8643c"));
        // little houses below the castle
        foreach (var hx in new[] { 60, 240, 280 })
        {
            c.Draw(q => S.Box(q, hx, 176, hx + 18, 192), C("c8a070"));
            c.Draw(q => S.Poly(q, new V(hx - 3, 177), new V(hx + 9, 166), new V(hx + 21, 177)), C("8a3a2a"));
        }
        // a giant ball rocketing by with a boost trail
        for (int i = 0; i < 38; i++)
        {
            float t = i / 38f; int x = 424 - i, y = 44 + i;
            for (int w = -3; w <= 3; w++) if (Math.Abs(w) <= 3.5f - t * 3.5f) c.Set(x, y + w, t < 0.4f ? C("ffe060") : C("ff7a30"));
        }
        Ball(c, new V(436, 32), 15, C("e8e8ee"), C("3a3a48"));
        // ground and scattered bricks
        for (int y = 200; y < c.H; y++) for (int x = 0; x < c.W; x++) c.Set(x, y, (x / 3 + y / 2) % 7 == 0 ? C("4a7a3e") : C("3e6a34"));
        Brick(c, 40, 228, 4, C("d83a3a")); Brick(c, 98, 238, 2, C("3a7ad8")); Brick(c, 150, 222, 3, C("f0c030"));
        Brick(c, 200, 244, 2, C("3ab05a")); Brick(c, 250, 230, 4, C("f0f0f0")); Brick(c, 430, 240, 3, C("d83a3a"));
    }

    static void Brick(Canvas c, int x, int y, int studs, Col col)
    {
        int w = studs * 7;
        c.Draw(q => S.Box(q, x, y, x + w, y + 8), col, hi: col.Light);
        for (int i = 0; i < studs; i++) c.Draw(q => S.Box(q, x + 1.5f + i * 7, y - 3, x + 5.5f + i * 7, y), col, hi: col.Light);
    }
}
