// Select button mockups for the two friends, next to the brothers' real buttons.
// Two looks: "plain" (charcoal with steel rings, silver name) and "darktide" (the same plus a gunmetal frame, rivets,
// a dull red pinstripe, and a purity seal). The names are drawn in a chunky block font shaped like the brothers' gold
// lettering (thick strokes, a dark outline the letters share), in silver.
// Run: "C:\Program Files\dotnet\dotnet.exe" run -c Release   (writes to out/)
const string Root = "D:/projects/csharp/SpireBrothers/";
var outDir = "out";
const int W = 132, H = 195, Cx = 66, Cy = 80;

// ---- the buttons
AnimatedBrother[] friends = [new JohnArt(), new HuffmanArt()];
var made = new Dictionary<string, Canvas>();
foreach (var look in new[] { "plain", "darktide" })
    foreach (var f in friends)
        foreach (var locked in new[] { false, true })
        {
            var card = FriendBackground(look == "darktide");
            Portrait(card, f, locked);
            if (look == "darktide") Seal(card, locked);
            BlockFont.Name(card, f.Name.ToUpperInvariant(), locked);
            var file = locked ? "select_locked.png" : "select.png";
            card.SavePng(Path.Combine(outDir, look, f.Name, file));
            made[$"{look}/{f.Name}/{file}"] = card;
        }

// ---- comparison sheets: the four real buttons, a gap, then the two friends
var real = new[] { "daniel", "david", "joshua", "tim" }.Select(n => PngRead.Load(Root + $"SpireBrothers/images/characters/{n}/select.png")).ToList();
foreach (var look in new[] { "plain", "darktide" })
{
    var row = real.Concat(friends.Select(f => made[$"{look}/{f.Name}/select.png"])).ToList();
    var sheet = new Canvas(row.Count * (W + 6) + 6 + 10, H + 12);
    Fill(sheet, Col.Hex("101014"));
    for (int i = 0; i < row.Count; i++) Paste(sheet, row[i], 6 + i * (W + 6) + (i >= 4 ? 10 : 0), 6);
    sheet.SavePng(Path.Combine(outDir, $"compare_{look}.png"), 2);
}
// both looks side by side, unlocked and locked
var both = new Canvas(8 * (W + 6) + 6 + 16, H + 12);
Fill(both, Col.Hex("101014"));
int bx = 6;
foreach (var look in new[] { "plain", "darktide" })
{
    foreach (var f in friends) foreach (var file in new[] { "select.png", "select_locked.png" }) { Paste(both, made[$"{look}/{f.Name}/{file}"], bx, 6); bx += W + 6; }
    bx += 16;
}
both.SavePng(Path.Combine(outDir, "looks.png"), 2);
// bodies at game scale next to the brothers, and zoomed heads
AnimatedBrother[] everyone = [new DanielArt(), new DavidArt(), new JoshuaArt(), new TimArt(), new JohnArt(), new HuffmanArt()];
var gray = Col.Hex("5a5f6a");
var lineup = new Canvas(66 * everyone.Length - 2, 80);
var heads = new Canvas(34 * everyone.Length, 30);
for (int i = 0; i < everyone.Length; i++)
{
    var fr = everyone[i].Render(everyone[i].Anims()["idle"].poses[0]);
    for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) lineup.Set(i * 66 + x, y, fr.Get(x, y) ?? gray);
    for (int y = 0; y < 30; y++) for (int x = 0; x < 34; x++) heads.Set(i * 34 + x, y, fr.Get(x + 14, y + 2) ?? gray);
}
lineup.SavePng(Path.Combine(outDir, "lineup.png"), 5);
heads.SavePng(Path.Combine(outDir, "heads.png"), 8);
// every animation frame of the friends (first drafts)
foreach (var f in friends)
{
    var anims = f.Anims();
    int maxF = anims.Values.Max(a => a.poses.Count);
    var sheet = new Canvas(66 * maxF - 2, anims.Count * 82 - 2);
    int r = 0;
    foreach (var (_, (poses, _, _)) in anims)
    {
        for (int i = 0; i < poses.Count; i++) { var fr = f.Render(poses[i]); for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) sheet.Set(i * 66 + x, r * 82 + y, fr.Get(x, y) ?? gray); }
        r++;
    }
    sheet.SavePng(Path.Combine(outDir, $"preview_{f.Name}.png"), 3);
}
// John's Stimm Lab rest site button (vanilla's option icons are 256 x 169): a glowing Hive Scum stimm, drawn at half size
{
    var icon = new Canvas(128, 85) { Origin = new V(0, 0) };
    var steel = Col.Hex("b8c0cc"); var dark = Col.Hex("4a4e58"); var chem = Col.Hex("70ff80");
    V a = new(26, 64), b = new(94, 22);                       // plunger end to needle base, tilted up to the right
    var d = (b - a).Norm; var n = d.Perp;
    // soft green glow behind it
    for (int y = 0; y < 85; y++) for (int x = 0; x < 128; x++)
    {
        float dist = S.Capsule(new V(x, y), a, b, 14);
        if (dist < 12) icon.Set(x, y, new Col(112, 255, 128, (byte)(70 * (1 - dist / 12))));
    }
    icon.Draw(q => S.Capsule(q, b, b + d * 22, 1.2f), steel, hi: Col.Hex("ffffff"));                       // needle
    icon.Draw(q => S.Capsule(q, a + d * 8, b, 9), steel, hi: Col.Hex("e8eef5"));                            // barrel
    icon.Flat(q => S.Capsule(q, a + d * 13 + n * 1.5f, b - d * 6 + n * 1.5f, 5), chem);                     // the chem inside
    icon.Flat(q => S.Capsule(q, a + d * 15 + n * 3.5f, b - d * 12 + n * 3.5f, 1.2f), Col.Hex("d8ffe0"));   // glint
    for (int k = 0; k < 4; k++) { var m = a + d * (20 + k * 11); icon.Draw(q => S.Capsule(q, m - n * 9, m - n * 5, 0.8f), dark, outline: false, noShade: true); }
    icon.Draw(q => S.Capsule(q, a + d * 6 + n * 13, a + d * 6 - n * 13, 2.6f), dark, hi: steel);            // finger grip
    icon.Draw(q => S.Capsule(q, a - d * 8, a + d * 6, 2.4f), dark, hi: steel);                               // plunger rod
    icon.Draw(q => S.Capsule(q, a - d * 9 + n * 8, a - d * 9 - n * 8, 2.6f), dark, hi: steel);              // thumb rest
    icon.SavePng(Path.Combine(outDir, "john", "stimm_lab.png"), 2);
}
// Huffman's cyber-mastiff (40 x 28, facing right, shown 3.2x in combat): a big grey dog with metal plates and a red eye
{
    var dog = new Canvas(40, 28) { Origin = new V(0, 0) };
    var fur = Col.Hex("5a5650"); var plate = Col.Hex("8a929c"); var dark = Col.Hex("2e2c2a");
    foreach (var (hip, foot) in new[] { (new V(9, 18), new V(8, 27)), (new V(13, 18), new V(14, 27)), (new V(24, 18), new V(23, 27)), (new V(28, 18), new V(29, 27)) })
        dog.Draw(q => S.Capsule(q, hip, foot, 1.7f), hip.X is 13 or 28 ? fur : fur.Mul(0.8f));
    dog.Draw(q => S.Taper(q, new V(7, 13), new V(1, 7), 1.6f, 0.8f), fur);                                     // tail, up
    dog.Draw(q => S.Ellipse(q, new V(18, 15), 12, 6), fur, hi: fur.Light);                                     // body
    dog.Draw(q => S.Box(q, 13, 9.5f, 24, 13), plate, hi: Col.Hex("c8d0da"));                                  // armour plate on the back
    dog.Plot(15, 11, dark); dog.Plot(19, 11, dark); dog.Plot(23, 11, dark);                                    // rivets
    dog.Draw(q => S.Capsule(q, new V(27, 12), new V(31, 8), 3.6f), fur);                                       // neck
    dog.Draw(q => S.U(S.Ellipse(q, new V(32, 7), 4.5f, 3.8f), S.Box(q, 33, 6, 39, 10.5f)), fur, hi: fur.Light); // head and snout
    dog.Draw(q => S.Poly(q, new V(29, 4), new V(30, 0), new V(32, 3.5f)), fur.Mul(0.8f));                      // ear
    dog.Draw(q => S.Box(q, 31, 4.5f, 36, 6.5f), plate);                                                        // cyber visor
    dog.Plot(35, 5, Col.Hex("ff3a2a")); dog.Plot(34, 5, Col.Hex("ff8a7a"));                                    // red eye
    dog.Plot(39, 7, dark);                                                                                    // nose
    dog.Draw(q => S.Box(q, 26, 10, 31, 11.5f), Col.Hex("8a2a24"), outline: false);                             // collar
    dog.Plot(28, 12, Col.Hex("d4af37"));                                                                      // tag
    dog.SavePng(Path.Combine(outDir, "huffman", "mastiff.png"));
    dog.SavePng(Path.Combine(outDir, "mastiff_preview.png"), 8);
}
// their character select backgrounds (pixelgen's Backgrounds.cs): out/<name>/select_bg.png
Backgrounds.RenderAll(friends, outDir);
// combat frames (out/<name>/frames/<anim>_<n>.png) and co-op treasure room hands, like pixelgen does for the brothers
foreach (var f in friends)
{
    f.Portrait = false;
    foreach (var (name, (poses, _, _)) in f.Anims())
        for (int i = 0; i < poses.Count; i++) f.Render(poses[i]).SavePng(Path.Combine(outDir, f.Name, "frames", $"{name}_{i}.png"));
}
Hands.RenderAll(friends, outDir);
// map marker (128 x 128): his pixel face on a charcoal disc with a steel rim, where the brothers have an emoji on their color;
// and icon_outline.png, the flat white disc the game shows behind it on co-op map votes
foreach (var f in friends)
{
    f.Portrait = true;
    var frame = f.Render(f.Anims()["idle"].poses[0]);
    var marker = new Canvas(128, 128);
    var outline = new Canvas(128, 128);
    for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
    {
        float d = MathF.Sqrt((x - 63.5f) * (x - 63.5f) + (y - 63.5f) * (y - 63.5f));
        if (d <= 63.5f) outline.Set(x, y, Col.Hex("ffffff"));
        if (d <= 60) marker.Set(x, y, Col.Hex("1b1b1b"));
        if (d <= 55) marker.Set(x, y, Col.Hex("6a707c"));
        if (d <= 52) marker.Set(x, y, Col.Hex("3a3d44").Lerp(Col.Hex("16171b"), y / 128f));
    }
    // his head and shoulders, 3x, centered in the disc
    const int HX = 12, HY = 2, HW = 38, HH = 34, K = 3;
    for (int y = 0; y < HH * K; y++) for (int x = 0; x < HW * K; x++)
    {
        int mx = 64 - HW * K / 2 + x, my = 64 - HH * K / 2 + y + 6;
        float d = MathF.Sqrt((mx - 63.5f) * (mx - 63.5f) + (my - 63.5f) * (my - 63.5f));
        if (d <= 52 && frame.Get(HX + x / K, HY + y / K) is { } px) marker.Set(mx, my, px);
    }
    marker.SavePng(Path.Combine(outDir, f.Name, "map_marker.png"));
    outline.SavePng(Path.Combine(outDir, f.Name, "icon_outline.png"));
}
// ---- Brandi (Daniel's wife: family, not a friend). Her own look: a crimson button with red rings and a warm gold name,
// a red map marker, and the same frames / hands / select background as everyone else. Written to out/brandi/.
{
    var brandi = new BrandiArt();
    foreach (var locked in new[] { false, true })
    {
        var card = BrandiBackground();
        Portrait(card, brandi, locked);
        BlockFont.Name(card, "BRANDI", locked, "ffe9a8", "d89a3a");
        card.SavePng(Path.Combine(outDir, "brandi", locked ? "select_locked.png" : "select.png"));
    }
    Backgrounds.RenderAll([brandi], outDir);
    brandi.Portrait = false;
    foreach (var (name, (poses, _, _)) in brandi.Anims())
        for (int i = 0; i < poses.Count; i++) brandi.Render(poses[i]).SavePng(Path.Combine(outDir, "brandi", "frames", $"{name}_{i}.png"));
    Hands.RenderAll([brandi], outDir);
    brandi.Portrait = true;
    var frame = brandi.Render(brandi.Anims()["idle"].poses[0]);
    var marker = new Canvas(128, 128);
    var outline = new Canvas(128, 128);
    for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
    {
        float d = MathF.Sqrt((x - 63.5f) * (x - 63.5f) + (y - 63.5f) * (y - 63.5f));
        if (d <= 63.5f) outline.Set(x, y, Col.Hex("ffffff"));
        if (d <= 60) marker.Set(x, y, Col.Hex("1b0a0c"));
        if (d <= 55) marker.Set(x, y, Col.Hex("c84a50"));
        if (d <= 52) marker.Set(x, y, Col.Hex("a3262b").Lerp(Col.Hex("3a0c0e"), y / 128f));
    }
    const int HX = 12, HY = 2, HW = 38, HH = 34, K = 3;
    for (int y = 0; y < HH * K; y++) for (int x = 0; x < HW * K; x++)
    {
        int mx = 64 - HW * K / 2 + x, my = 64 - HH * K / 2 + y + 6;
        float d = MathF.Sqrt((mx - 63.5f) * (mx - 63.5f) + (my - 63.5f) * (my - 63.5f));
        if (d <= 52 && frame.Get(HX + x / K, HY + y / K) is { } px) marker.Set(mx, my, px);
    }
    marker.SavePng(Path.Combine(outDir, "brandi", "map_marker.png"));
    outline.SavePng(Path.Combine(outDir, "brandi", "icon_outline.png"));
}
Console.WriteLine("done");

// ---------------------------------------------------------------- helpers

static void Fill(Canvas c, Col col) { for (int y = 0; y < c.H; y++) for (int x = 0; x < c.W; x++) c.Set(x, y, col); }
static void Paste(Canvas dst, Canvas src, int ox, int oy) { for (int y = 0; y < src.H; y++) for (int x = 0; x < src.W; x++) if (src.Get(x, y) is { } c) dst.Set(ox + x, oy + y, c); }
static void Blend(Canvas c, int x, int y, Col col, float a) { if (c.Get(x, y) is { } o) c.Set(x, y, o.Lerp(col, a)); }
static Col Dim(Col c) { var l = (byte)((0.3f * c.R + 0.59f * c.G + 0.11f * c.B) * 0.32f); return new Col(l, l, l); }

static float RingCover(int x, int y)
{
    float r = MathF.Sqrt((x + 0.5f - Cx) * (x + 0.5f - Cx) + (y + 0.5f - Cy) * (y + 0.5f - Cy));
    float m = r % 22f;
    return Math.Clamp(Math.Min(m - 16.5f, 22f - m + 0.5f), 0, 1);
}

// Charcoal gradient with steel rings (the brothers' buttons have the same rings in their own colors); the darktide look
// adds a gunmetal frame with rivets and a red pinstripe.
static Canvas FriendBackground(bool darktide)
{
    var top = Col.Hex("3a3d44"); var low = Col.Hex("16171b"); var ring = Col.Hex("5c616c");
    var c = new Canvas(W, H);
    for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
    {
        var g = top.Lerp(low, Math.Min(1f, y / 150f));
        c.Set(x, y, g.Lerp(ring, RingCover(x, y) * 0.55f));
    }
    if (!darktide) return c;
    var metal = Col.Hex("4a4e57"); var hi = Col.Hex("7a808c"); var dark = Col.Hex("22242a"); var red = Col.Hex("8a2a24");
    for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
    {
        int d = Math.Min(Math.Min(x, y), Math.Min(W - 1 - x, H - 1 - y));
        if (d == 0 || d == 3) c.Set(x, y, dark);
        else if (d == 1 && (x == 1 || y == 1)) c.Set(x, y, hi);   // light catches the top and left edges
        else if (d < 3) c.Set(x, y, metal);
        else if (d == 5) c.Set(x, y, red);
    }
    // rivets in the corners and along the sides
    foreach (var (x, y) in new[] { (2, 2), (W - 3, 2), (2, H - 3), (W - 3, H - 3), (2, H / 2), (W - 3, H / 2) })
    {
        c.Set(x, y, Col.Hex("a8aeba")); c.Set(x + 1, y, metal); c.Set(x, y + 1, metal); c.Set(x + 1, y + 1, dark);
    }
    return c;
}

// Brandi's button: a crimson gradient with red rings (the brothers' buttons have the same rings in their own colors).
static Canvas BrandiBackground()
{
    var top = Col.Hex("8a1e26"); var low = Col.Hex("2a070a"); var ring = Col.Hex("c8444c");
    var c = new Canvas(W, H);
    for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
    {
        var g = top.Lerp(low, Math.Min(1f, y / 150f));
        c.Set(x, y, g.Lerp(ring, RingCover(x, y) * 0.5f));
    }
    return c;
}

// The pixel portrait from the first idle frame, 3x, with a soft drop shadow (same crop as pixelgen's SelectButtons).
static void Portrait(Canvas card, AnimatedBrother b, bool locked)
{
    b.Portrait = true;
    var frame = b.Render(b.Anims()["idle"].poses[0]);
    const int S = 3, X0 = 8, Y0 = 2, CW = 44, CH = 50, OY = 12;
    Col? Port(int x, int y) => frame.Get(X0 + x / S, Y0 + y / S);
    for (int y = 0; y < CH * S; y++) for (int x = 0; x < CW * S; x++)
        if (Port(x, y) != null) Blend(card, x + 4, y + OY + 4, new Col(0, 0, 0), 0.35f);
    for (int y = 0; y < CH * S; y++) for (int x = 0; x < CW * S; x++)
        if (Port(x, y) is { } c) card.Set(x, y + OY, locked ? Dim(c) : c);
}

// A red wax purity seal with a parchment strip, pinned to the top-left corner.
static void Seal(Canvas card, bool locked)
{
    var wax = Col.Hex("9a2622"); var paper = Col.Hex("d8c8a0");
    Col L(Col c) => locked ? Dim(c) : c;
    var at = new V(16, 16);
    card.Draw(q => S.Poly(q, at + new V(-3, 2), at + new V(2.5f, 2), at + new V(2, 26), at + new V(-0.5f, 23.5f), at + new V(-3.5f, 26)), L(paper), hi: L(paper.Light));
    for (int y = 8; y < 22; y += 3) { card.Set((int)at.X - 1, (int)at.Y + y, L(Col.Hex("6a5a40"))); card.Set((int)at.X, (int)at.Y + y, L(Col.Hex("6a5a40"))); }
    card.Draw(q => S.Circle(q, at, 6.5f) - MathF.Abs(MathF.Sin(MathF.Atan2(q.Y - at.Y, q.X - at.X) * 5)) * 0.8f, L(wax), hi: L(wax.Light));
    card.Draw(q => S.Circle(q, at, 3.2f), L(wax.Shade), outline: false, noShade: true);
    card.Set((int)at.X - 1, (int)at.Y - 1, L(wax.Light));
}

// ---- the name: a 5 x 7 block font. Outer columns are thick strokes, inner ones thin, like the brothers' lettering.
static class BlockFont
{
    public static readonly Dictionary<char, string[]> Font = new()
    {
        ['B'] = ["####.", "#...#", "#...#", "####.", "#...#", "#...#", "####."],
        ['D'] = ["####.", "#...#", "#...#", "#...#", "#...#", "#...#", "####."],
        ['I'] = ["#####", "..#..", "..#..", "..#..", "..#..", "..#..", "#####"],
        ['R'] = ["####.", "#...#", "#...#", "####.", "#.#..", "#..#.", "#...#"],
        ['A'] = [".###.", "#...#", "#...#", "#####", "#...#", "#...#", "#...#"],
        ['F'] = ["#####", "#....", "#....", "####.", "#....", "#....", "#...."],
        ['H'] = ["#...#", "#...#", "#...#", "#####", "#...#", "#...#", "#...#"],
        ['J'] = ["..###", "....#", "....#", "....#", "#...#", "#...#", ".###."],
        ['M'] = ["#...#", "##.##", "#.#.#", "#.#.#", "#...#", "#...#", "#...#"],
        ['N'] = ["#...#", "##..#", "##..#", "#.#.#", "#..##", "#..##", "#...#"],
        ['O'] = [".###.", "#...#", "#...#", "#...#", "#...#", "#...#", ".###."],
        ['U'] = ["#...#", "#...#", "#...#", "#...#", "#...#", "#...#", ".###."],
    };
    public const int W = 132, H = 195, Cx = 66;

    public static void Name(Canvas card, string text, bool locked, string top = "eef1f6", string bottom = "9ba3b0")
    {
        // short names get big letters and long ones smaller, like the brothers' (DAVID and TIM are bigger than DANIEL and JOSHUA)
        bool big = text.Length <= 5;
        int[] ColW = big ? [7, 3, 3, 3, 7] : [4, 2, 2, 2, 4];   // HUFFMAN has to fit inside the darktide frame
        int[] RowH = big ? [4, 4, 4, 4, 4, 4, 4] : [3, 3, 3, 4, 3, 3, 3];
        int TextTop = big ? 154 : 158, gap = big ? 3 : 2;
        int letterW = ColW.Sum(), width = text.Length * letterW + (text.Length - 1) * gap;
        var fill = new bool[W, H];
        int x0 = Cx - width / 2;
        foreach (var ch in text)
        {
            var rows = Font[ch];
            int y = TextTop;
            for (int r = 0; r < 7; r++)
            {
                int x = x0;
                for (int k = 0; k < 5; k++)
                {
                    if (rows[r][k] == '#') for (int dy = 0; dy < RowH[r]; dy++) for (int dx = 0; dx < ColW[k]; dx++) fill[x + dx, y + dy] = true;
                    x += ColW[k];
                }
                y += RowH[r];
            }
            x0 += letterW + gap;
        }
        int textH = RowH.Sum();
        var outline = Col.Hex("1a161e");
        static Col Dim(Col c) { var l = (byte)((0.3f * c.R + 0.59f * c.G + 0.11f * c.B) * 0.32f); return new Col(l, l, l); }
        for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
        {
            if (fill[x, y])
            {
                // silver, bright at the top and cooler toward the bottom, with a light first row
                float t = (y - TextTop) / (float)(textH - 1);
                var c = y == TextTop ? Col.Hex("ffffff") : Col.Hex(top).Lerp(Col.Hex(bottom), t);
                card.Set(x, y, locked ? Dim(c) : c);
                continue;
            }
            // a 2px outline the letters share, 3px along the bottom
            bool near = false;
            for (int dy = -3; dy <= 2 && !near; dy++) for (int dx = -2; dx <= 2 && !near; dx++)
                near = x + dx >= 0 && y + dy >= 0 && x + dx < W && y + dy < H && fill[x + dx, y + dy];
            if (near) card.Set(x, y, outline);
        }
    }
}
