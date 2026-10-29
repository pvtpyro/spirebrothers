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
                case "john": John(c); break;
                case "huffman": Huffman(c); break;
                case "brandi": Brandi(c); break;
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
        // a stack of books by his feet (he reads everywhere he goes), one lying open on top
        (int x, int w, string col)[] books = [(266, 30, "2c4a8a"), (269, 25, "8a2a2a"), (264, 28, "2e6a3a"), (268, 24, "c89a30")];
        for (int i = 0; i < books.Length; i++)
        {
            var (bx, bw, bc) = books[i]; int by = 239 - i * 7;
            c.Draw(q => S.Box(q, bx, by, bx + bw, by + 6), C(bc), hi: C(bc).Light);
            for (int x = bx + 2; x < bx + bw - 1; x++) c.Set(x, by + 3, C("efe6cc"));   // page edges
        }
        c.Draw(q => S.Poly(q, new V(266, 210), new V(280, 206), new V(294, 210), new V(294, 213), new V(280, 210), new V(266, 213)), C("efe6cc"), hi: C("ffffff"));
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
        // a sword resting in a stone on the hill (old-school adventure games)
        c.Draw(q => S.Poly(q, new V(160, 194), new V(163, 184), new V(181, 184), new V(184, 194)), C("8a8296"), hi: C("b0a8bc"));
        c.Draw(q => S.Poly(q, new V(169.5f, 185), new V(169.5f, 160), new V(172, 156), new V(174.5f, 160), new V(174.5f, 185)), C("d8e0ec"), hi: C("ffffff"));
        c.Draw(q => S.Box(q, 164, 156, 180, 159), C("4a5ac8"));
        c.Draw(q => S.Box(q, 170.5f, 147, 173.5f, 156), C("3a2a5a"));
        c.Draw(q => S.Circle(q, new V(172, 146), 2), C("f0c040"));
        // a d20 tumbling across the sky (D&D), with a sparkly trail
        for (int i = 0; i < 46; i++) { c.Set(384 + i, 50 + i / 3, C("ffffff").Lerp(C("f6b08a"), i / 46f)); if (i % 4 == 0) c.Set(386 + i, 58 + i / 3, C("ffd860")); }
        D20(c, new V(368, 52), 16);
        // floating notes
        Note(c, new V(214, 70), C("ffd860"));
        Note(c, new V(236, 50), C("ffffff"));
        Note(c, new V(256, 78), C("ffd860"));
    }

    /// <summary>A d20 seen face-on: a hexagon outline with the front triangle face lit, showing a 20.</summary>
    static void D20(Canvas c, V at, float r)
    {
        var hex = new V[6];
        for (int i = 0; i < 6; i++) hex[i] = at + V.Dir(-90 + i * 60) * r;
        var tri = new[] { at + new V(0, -r * 0.62f), at + new V(r * 0.6f, r * 0.42f), at + new V(-r * 0.6f, r * 0.42f) };
        c.Draw(q => S.Poly(q, hex), C("b8304a"), hi: C("e05a70"));
        c.Flat(q => S.Poly(q, tri), C("e05a70"));
        // edges from the front face out to the rim
        var edge = C("6a1a2a");
        foreach (var (a, b) in new[] { (tri[0], tri[1]), (tri[1], tri[2]), (tri[2], tri[0]), (tri[0], hex[0]), (tri[0], hex[1]), (tri[0], hex[5]),
                     (tri[1], hex[1]), (tri[1], hex[2]), (tri[1], hex[3]), (tri[2], hex[3]), (tri[2], hex[4]), (tri[2], hex[5]) })
            for (int s = 0; s <= 24; s++) c.Plot(a + (b - a) * (s / 24f), edge);
        // "20" in a 3 x 5 pixel font
        string[] two = ["###", "..#", "###", "#..", "###"], zero = ["###", "#.#", "#.#", "#.#", "###"];
        int x0 = (int)at.X - 3, y0 = (int)at.Y - 1;
        for (int y = 0; y < 5; y++) for (int x = 0; x < 3; x++)
        {
            if (two[y][x] == '#') c.Set(x0 + x, y0 + y, C("ffffff"));
            if (zero[y][x] == '#') c.Set(x0 + 4 + x, y0 + y, C("ffffff"));
        }
        Sparkle(c, at + new V(r + 3, -r + 2));
        Sparkle(c, at + new V(-r - 2, r - 4));
    }

    static void Sparkle(Canvas c, V at)
    {
        for (int k = -2; k <= 2; k++) { c.Plot(at + new V(k, 0), C("fff4c0")); c.Plot(at + new V(0, k), C("fff4c0")); }
        c.Plot(at, C("ffffff"));
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

    // The friends' scenes are darker than the brothers' (night, underground), so they read as a set of their own.

    // ---------------------------------------------------------------- John: the Shroud at night, a Flame Altar, hive spires far off

    static void John(Canvas c)
    {
        Sky(c, 200, C("07060e"), C("140c22"), C("2a1634"), C("3a1e3a"));
        Stars(c, 21, 60, 90, C("b8a8d8"));
        // a pale moon behind the haze
        c.Flat(q => S.Circle(q, new V(110, 46), 15), C("8a7a98"));
        c.Flat(q => S.Circle(q, new V(106, 43), 11), C("a898b4"));
        // hive city spires on the horizon (Hive Scum's home), with a few lit windows and a smog glow
        var hive = C("120c18");
        var rng = new Random(12);
        // the hive itself: a stepped mass of city, tier on tier, with rows of lit windows
        c.Flat(q => S.Poly(q, new V(130, 178), new V(160, 150), new V(180, 150), new V(196, 128), new V(222, 128), new V(236, 108),
            new V(290, 108), new V(304, 128), new V(330, 128), new V(344, 150), new V(372, 150), new V(400, 178)), hive);
        foreach (var (y0, x0, x1) in new[] { (114, 240, 286), (134, 200, 326), (156, 164, 368) })
            for (int x = x0; x < x1; x += 3) if (rng.Next(3) > 0) c.Set(x, y0 + rng.Next(0, 3) * 4, rng.Next(5) == 0 ? C("ff5a3a") : C("e0a040"));
        // gothic spires rising out of it, with crossbars and antennas
        foreach (var (sx, top, w) in new[] { (210, 74, 7), (244, 40, 9), (263, 22, 11), (284, 46, 8), (318, 80, 7) })
        {
            int baseY = sx is > 230 and < 296 ? 110 : 130;
            c.Flat(q => S.Poly(q, new V(sx - w, baseY), new V(sx - w * 0.6f, top + 16), new V(sx, top), new V(sx + w * 0.6f, top + 16), new V(sx + w, baseY)), hive);
            for (int y = top - 12; y < top; y++) c.Set(sx, y, hive);
            c.Set(sx, top - 13, C("ff5a3a"));
            for (int k = 1; k <= 2; k++) { int by = top + 16 + k * (baseY - top - 16) / 3; for (int x = sx - w - 2; x <= sx + w + 2; x++) c.Set(x, by, hive); }
            for (int y = top + 20; y < baseY - 4; y += 7) if (rng.Next(3) > 0) c.Set(sx + rng.Next(-1, 2), y, C("e0a040"));
        }
        // smokestacks venting smog
        foreach (var sx in new[] { 186, 340 })
        {
            c.Flat(q => S.Box(q, sx, 112, sx + 5, 150), hive);
            for (int i = 0; i < 5; i++) c.Flat(q => S.Circle(q, new V(sx + 2 + i * 6, 104 - i * 8), 4 + i * 1.4f), C("2a1a2e").Lerp(C("3a1e3a"), i * 0.18f));
        }
        for (int y = 120; y < 178; y++) for (int x = 130; x < 400; x++) Blend(c, x, y, C("6a2a3a"), (y - 120) / 58f * 0.25f * (1 - MathF.Abs(x - 265) / 140f));
        // the ruined valley
        Ridge(c, 182, 9, 0.028f, 13, C("1a1220"), C("2a1e2e"));
        // dead, twisted trees in the fog
        foreach (var (tx, th) in new[] { (40, 70), (92, 52), (190, 60), (450, 66) }) DeadTree(c, tx, 204, th);
        // ground
        for (int y = 200; y < c.H; y++) for (int x = 0; x < c.W; x++) c.Set(x, y, (x * 7 + y * 13) % 11 == 0 ? C("2a2026") : C("1e171c"));
        for (int i = 0; i < 40; i++) { int x = rng.Next(c.W), y = rng.Next(204, c.H); c.Set(x, y, C("3a3036")); c.Set(x + 1, y, C("2e262c")); }
        // the Shroud: purple fog banks with glowing spores, pushed back around the altar's light
        var altar = new V(262, 196);
        for (int y = 130; y < c.H; y++) for (int x = 0; x < c.W; x++)
        {
            float wave = MathF.Sin(x * 0.03f + y * 0.11f) * 0.5f + MathF.Sin(x * 0.011f - y * 0.05f) * 0.5f;
            // thickest in a bank along the horizon, thinner toward the camera
            float bank = MathF.Exp(-MathF.Pow((y - 198) / 22f, 2));
            float fog = Math.Clamp((y - 140) / 70f, 0, 1) * (0.32f + 0.14f * wave) + bank * (0.35f + 0.2f * wave);
            float clear = Math.Clamp(1 - (new V(x, y) - altar).Len / 95f, 0, 1);
            fog *= 1 - clear * clear * 1.6f;
            if (fog > 0) Blend(c, x, y, (x + y) % 2 == 0 ? C("5a2a6a") : C("4a2258"), fog);
        }
        for (int i = 0; i < 70; i++)
        {
            var at = new V(rng.Next(c.W), rng.Next(140, 262));
            if ((at - altar).Len < 70) continue;
            c.Plot(at, rng.Next(3) == 0 ? C("7af0e0") : C("e070d0"));
            if (rng.Next(4) == 0) { c.Plot(at + new V(1, 0), C("8a3a8a")); c.Plot(at + new V(0, 1), C("8a3a8a")); }
        }
        // the Flame Altar: a stone plinth with a big flame and a warm pool of light
        for (int y = 150; y < c.H; y++) for (int x = 170; x < 360; x++)
        {
            float d = (new V(x, (y - altar.Y) * 1.8f + altar.Y) - altar).Len;
            if (d < 90) Blend(c, x, y, C("ff9a40"), (1 - d / 90) * (1 - d / 90) * 0.35f);
        }
        c.Draw(q => S.Poly(q, altar + new V(-14, 8), altar + new V(-10, -14), altar + new V(10, -14), altar + new V(14, 8)), C("5a5260"), hi: C("7a7280"));
        c.Draw(q => S.Box(q, altar.X - 16, altar.Y - 20, altar.X + 16, altar.Y - 14), C("6a6270"), hi: C("8a8290"));
        for (int i = -1; i <= 1; i++) c.Flat(q => S.Box(q, altar.X + i * 7 - 1, altar.Y - 10, altar.X + i * 7 + 1, altar.Y + 4), C("3a3440"));
        c.Flat(q => S.Taper(q, altar + new V(0, -20), altar + new V(1, -52), 11, 1), C("ff6a20"));
        c.Flat(q => S.Taper(q, altar + new V(-7, -20), altar + new V(-10, -38), 5, 0.6f), C("ff6a20"));
        c.Flat(q => S.Taper(q, altar + new V(7, -20), altar + new V(10, -36), 5, 0.6f), C("ff6a20"));
        c.Flat(q => S.Taper(q, altar + new V(0, -20), altar + new V(0, -42), 6.5f, 0.8f), C("ffb040"));
        c.Flat(q => S.Taper(q, altar + new V(0, -20), altar + new V(0, -30), 3.5f, 0.6f), C("fff0b0"));
        for (int i = 0; i < 10; i++) c.Plot(altar + new V(rng.Next(-14, 14), -56 - rng.Next(0, 20)), i % 2 == 0 ? C("ffd060") : C("ff8030"));
        // his crafting corner: an anvil with a glowing green stimm, and a loot chest (always grinding)
        var iron = C("4a4e58");
        c.Draw(q => S.U(S.Box(q, 410, 214, 444, 220), S.U(S.Box(q, 420, 220, 434, 232), S.Box(q, 414, 232, 440, 238))), iron, hi: C("7a808c"));
        c.Draw(q => S.Taper(q, new V(410, 217), new V(400, 215), 3, 1), iron);
        c.Draw(q => S.Capsule(q, new V(428, 212), new V(428, 202), 2), C("8a929c"), hi: C("d8dee8"));
        c.Flat(q => S.Capsule(q, new V(428, 210.5f), new V(428, 204), 1), C("70ff80"));
        c.Plot(428, 199, C("e8eef5"));
        Sparkle(c, new V(434, 198));
        var wood = C("6b4a2b");
        c.Draw(q => S.Box(q, 282, 222, 312, 240), wood, hi: wood.Light);
        c.Draw(q => S.U(S.Box(q, 282, 214, 312, 222), S.Ellipse(q, new V(297, 216), 15, 5)), wood.Shade, hi: wood);
        c.Draw(q => S.Box(q, 282, 221, 312, 223), C("c8a040"));
        c.Draw(q => S.Box(q, 294, 219, 300, 226), C("e0c060"));
        for (int i = 0; i < 4; i++) c.Plot(288 + i * 6, 212 - i % 2, C("ffe070"));
    }

    static void DeadTree(Canvas c, int x, int ground, int h)
    {
        var bark = C("150f18");
        c.Flat(q => S.Taper(q, new V(x, ground), new V(x + 3, ground - h), 3.5f, 1), bark);
        c.Flat(q => S.Taper(q, new V(x + 1, ground - h * 0.55f), new V(x - 14, ground - h * 0.85f), 1.8f, 0.5f), bark);
        c.Flat(q => S.Taper(q, new V(x + 2, ground - h * 0.7f), new V(x + 16, ground - h * 0.95f), 1.6f, 0.5f), bark);
        c.Flat(q => S.Taper(q, new V(x - 9, ground - h * 0.77f), new V(x - 13, ground - h * 0.98f), 1f, 0.4f), bark);
    }

    // ---------------------------------------------------------------- Huffman: an underground server room, storage drives, a jetpack

    // ---------------------------------------------------------------- Brandi: Camp Victory, Baghdad, at night
    static void Brandi(Canvas c)
    {
        Sky(c, 186, C("070a1e"), C("141838"), C("2e2448"), C("5a3048"));
        Stars(c, 33, 90, 120, C("d8dcff"));
        // a thin crescent moon
        c.Flat(q => S.I(S.Circle(q, new V(392, 38), 13), -S.Circle(q, new V(398, 34), 12)), C("f4ecc8"));
        // the palace on its lake: a long low block with domes, windows lit
        var stone = C("2a2238"); var lit = C("ffcc70");
        c.Flat(q => S.Box(q, 214, 150, 330, 178), stone);
        c.Flat(q => S.Box(q, 248, 136, 296, 150), stone);
        c.Flat(q => S.Circle(q, new V(272, 136), 14), stone);
        c.Flat(q => S.Circle(q, new V(228, 150), 7), stone);
        c.Flat(q => S.Circle(q, new V(316, 150), 7), stone);
        c.Flat(q => S.Box(q, 271, 112, 273, 124), stone);
        for (int x = 220; x < 326; x += 8) for (int y = 156; y < 174; y += 7) if ((x * 7 + y) % 3 != 0) { c.Set(x, y, lit); c.Set(x + 1, y, lit); c.Set(x, y + 1, lit.Mul(0.8f)); }
        // the lake, with the palace lights wobbling in it
        for (int y = 178; y < 192; y++) for (int x = 0; x < c.W; x++)
        {
            c.Set(x, y, C("10142c").Lerp(C("1c2040"), (y - 178) / 14f));
            int mx = x + (int)(2 * MathF.Sin(y * 1.7f));
            if (mx > 220 && mx < 326 && mx % 8 < 2 && (y % 3 != 0)) Blend(c, x, y, lit, 0.35f * (192 - y) / 14f);
        }
        // palm silhouettes along the shore
        foreach (var (px, h) in new[] { (150, 54), (176, 42), (352, 50), (446, 58) })
        {
            var trunk = C("0c0a16");
            c.Flat(q => S.Taper(q, new V(px, 186), new V(px + 6, 186 - h), 2.6f, 1.4f), trunk);
            var top = new V(px + 6, 186 - h);
            foreach (var a in new[] { -160f, -125f, -90f, -55f, -20f, 15f, 160f + 30 })
                c.Flat(q => S.Taper(q, top, top + V.Dir(a) * 18 + new V(0, 6), 2.2f, 0.4f), trunk);
        }
        // the camp ground: packed sand
        for (int y = 192; y < c.H; y++) for (int x = 0; x < c.W; x++)
            c.Set(x, y, ((x * 13 + y * 7) % 11 == 0 ? C("3a2e2a") : C("2c2422")).Lerp(C("1a1416"), (y - 192) / 78f));
        // T-walls (the tall concrete barriers), one with a smiley sticker
        foreach (var tx in new[] { 6, 40, 74, 108 })
        {
            c.Draw(q => S.Box(q, tx, 150, tx + 30, 206), C("6a6458"), hi: C("8a8474"));
            c.Flat(q => S.Box(q, tx - 4, 200, tx + 34, 206), C("5a5448"));
        }
        var yellow = C("f8d030"); var face = new V(54, 170);
        c.Draw(q => S.Circle(q, face, 6), yellow, hi: C("fff090"));
        c.Plot(52, 168, C("2a1e10")); c.Plot(56, 168, C("2a1e10"));
        foreach (var (sx, sy) in new[] { (51, 172), (52, 173), (53, 173), (54, 173), (55, 173), (56, 173), (57, 172) }) c.Plot(sx, sy, C("2a1e10"));
        // a burn barrel by her, the fire she can't stop watching
        var barrel = new V(420, 214);
        c.Draw(q => S.Box(q, barrel.X - 11, barrel.Y - 22, barrel.X + 11, barrel.Y), C("4a3a30"), hi: C("6a5444"));
        for (int i = 0; i < 2; i++) c.Flat(q => S.Box(q, barrel.X - 11, barrel.Y - 17 + i * 9, barrel.X + 11, barrel.Y - 16 + i * 9), C("2a201a"));
        c.Flat(q => S.Taper(q, barrel + new V(0, -22), barrel + new V(-1, -48), 9, 1), C("ff6a1a"));
        c.Flat(q => S.Taper(q, barrel + new V(-5, -22), barrel + new V(-8, -38), 4, 0.5f), C("ff8a2a"));
        c.Flat(q => S.Taper(q, barrel + new V(0, -22), barrel + new V(1, -38), 5, 0.6f), C("ffd050"));
        for (int y = 140; y < c.H; y++) for (int x = 330; x < c.W; x++)
        {
            float d = MathF.Sqrt((x - barrel.X) * (x - barrel.X) + (y - (barrel.Y - 30)) * (y - (barrel.Y - 30)) * 1.6f);
            if (d < 90) Blend(c, x, y, C("ff7a30"), (1 - d / 90) * 0.16f);
        }
        var sparks = new Random(9);
        for (int i = 0; i < 14; i++) c.Set((int)barrel.X + sparks.Next(-12, 13), (int)barrel.Y - 50 - sparks.Next(40), sparks.Next(2) == 0 ? C("ffd050") : C("ff8a2a"));
    }

    static void Huffman(Canvas c)
    {
        // concrete wall with panel seams
        Fill(c, (x, y) => y < 206 ? ((x % 60 == 0 || y % 46 == 0) ? C("1a1c22") : C("20232a")) : null);
        // ceiling light strips, dim blue
        for (int x = 0; x < c.W; x++) { c.Set(x, 0, C("101216")); c.Set(x, 1, C("101216")); }
        foreach (var lx in new[] { 40, 200, 360 })
            for (int x = lx; x < lx + 80; x++) { c.Set(x, 4, C("9ad8ff")); c.Set(x, 5, C("5aa0d0")); for (int y = 6; y < 60; y++) Blend(c, x + (y - 6) / 3 - 9, y, C("5aa0d0"), (60 - y) / 54f * 0.06f); }
        // cable trays along the ceiling with glowing network cables
        for (int x = 0; x < c.W; x++) { c.Set(x, 14, C("3a3e48")); c.Set(x, 19, C("2a2e36")); }
        for (int x = 0; x < c.W; x++) { c.Set(x, 16, x % 24 < 12 ? C("8a50e0") : C("6a3ab0")); c.Set(x, 17, C("2ab0c0")); }
        var rng = new Random(31);
        // the big monitor: a star wrapped in a half-built Dyson sphere
        c.Draw(q => S.Box(q, 196, 30, 300, 96), C("2a2e36"), hi: C("4a4e58"));
        c.Flat(q => S.Box(q, 200, 34, 296, 92), C("060814"));
        for (int i = 0; i < 30; i++) c.Set(rng.Next(201, 295), rng.Next(35, 91), C("8aa0d0"));
        var star = new V(248, 63);
        c.Flat(q => S.Circle(q, star, 14), C("ffb040"));
        c.Flat(q => S.Circle(q, star, 10), C("ffe090"));
        c.Flat(q => S.Circle(q, star, 5), C("ffffff"));
        // the lattice: a few rings of the shell, the left half finished
        var shell = C("5ad0ff");
        for (float a = 0; a < 360; a += 1.2f)
        {
            var d = V.Dir(a);
            bool built = d.X < 0.2f;
            foreach (var ry in new[] { 6f, 12f, 18f })
            {
                var p = star + new V(d.X * 20, d.Y * ry);
                if (built || (int)a % 18 < 6) c.Plot(p, shell);
            }
            if (built && (int)(a * 10) % 150 < 12) c.Plot(star + d * 20, C("ffffff"));
        }
        for (float y = -20; y <= 20; y++) { c.Plot(star.X, star.Y + y, shell.Mul(0.8f)); if (y < 0 || y % 4 == 0) c.Plot(star.X - MathF.Sqrt(400 - y * y) * 0.55f, star.Y + y, shell.Mul(0.8f)); }
        c.Draw(q => S.Box(q, 244, 96, 252, 104), C("2a2e36"));
        // server racks along the wall, lights blinking
        foreach (var rx in new[] { 6, 64, 122, 312, 370 }) Rack(c, rx, 78, rng);
        // the storage drive wall: rows of cells, each with a colored light
        c.Draw(q => S.Box(q, 400, 40, 474, 140), C("2e3038"), hi: C("4a4e58"));
        Col[] cell = [C("60e070"), C("5ad0ff"), C("e0a040"), C("b070f0")];
        for (int row = 0; row < 6; row++) for (int col = 0; col < 3; col++)
        {
            int x = 406 + col * 22, y = 46 + row * 15;
            c.Draw(q => S.Box(q, x, y, x + 18, y + 11), C("1a1c22"));
            c.Flat(q => S.Box(q, x + 2, y + 2, x + 13, y + 9), C("3a3e48"));
            var lit = cell[(row * 3 + col * 5) % cell.Length];
            c.Set(x + 15, y + 4, lit); c.Set(x + 15, y + 5, lit); c.Set(x + 16, y + 4, lit.Light);
        }
        for (int y = 140; y < 206; y++) { c.Set(420, y, C("8a50e0")); c.Set(421, y, C("6a3ab0")); c.Set(452, y, C("2ab0c0")); }
        // a jetpack hanging on a wall hook: twin tanks, red stripes, nozzles
        c.Draw(q => S.Capsule(q, new V(430, 152), new V(430, 156), 1.5f), C("8a8e98"));
        foreach (var tx in new[] { 422f, 438f })
        {
            c.Draw(q => S.Capsule(q, new V(tx, 164), new V(tx, 186), 6), C("b8bcc8"), hi: C("e8eef5"));
            c.Flat(q => S.Box(q, tx - 5, 172, tx + 5, 175), C("c83a3a"));
            c.Draw(q => S.Poly(q, new V(tx - 4, 190), new V(tx + 4, 190), new V(tx + 5.5f, 197), new V(tx - 5.5f, 197)), C("4a4e58"));
        }
        c.Draw(q => S.Box(q, 427, 160, 433, 188), C("4a4e58"));
        c.Plot(430, 166, C("60e070"));
        // raised data-center floor tiles, with the room's light reflected in them
        for (int y = 206; y < c.H; y++) for (int x = 0; x < c.W; x++)
        {
            bool seam = x % 32 == 0 || (y - 206) % 16 == 0;
            var tile = seam ? C("1a1c22") : C("2e323a");
            if (!seam && (x / 32 + (y - 206) / 16) % 3 == 0 && (y - 206) % 16 < 4 && x % 32 > 20) tile = C("4a5260");
            c.Set(x, y, tile);
        }
        for (int x = 0; x < c.W; x++) c.Set(x, 206, C("3a3e48"));
        // cables snaking across the floor, and the dog's bowl (for the mastiff)
        for (int x = 0; x < 300; x++) { int y = 236 + (int)(MathF.Sin(x * 0.05f) * 4); c.Set(x, y, C("2ab0c0")); c.Set(x, y + 1, C("1a7080")); }
        c.Draw(q => S.U(S.Ellipse(q, new V(268, 232), 12, 3.5f), S.Box(q, 257, 226, 279, 232)), C("8a2a24"), hi: C("b04a3a"));
        c.Flat(q => S.Ellipse(q, new V(268, 226), 10, 2.5f), C("4a1a18"));
        c.Draw(q => S.U(S.Capsule(q, new V(262, 224), new V(274, 222), 1.2f), S.U(S.Circle(q, new V(261, 223), 1.8f), S.Circle(q, new V(275, 221), 1.8f))), C("efe6cc"));
    }

    static void Rack(Canvas c, int x, int top, Random rng)
    {
        const int w = 50;
        c.Draw(q => S.Box(q, x, top, x + w, 206), C("15171c"), hi: C("2e323a"));
        for (int y = top + 6; y < 200; y += 9)
        {
            c.Flat(q => S.Box(q, x + 4, y, x + w - 4, y + 7), C("23262e"));
            for (int vx = x + 8; vx < x + 30; vx += 3) c.Set(vx, y + 3, C("15171c"));
            Col[] leds = [C("60e070"), C("60e070"), C("5ad0ff"), C("e0a040"), C("2a2e36")];
            for (int k = 0; k < 4; k++) c.Set(x + 34 + k * 3, y + 3, leds[rng.Next(leds.Length)]);
        }
    }
}
