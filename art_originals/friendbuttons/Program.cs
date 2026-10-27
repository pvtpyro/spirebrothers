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
// their character select backgrounds (pixelgen's Backgrounds.cs): out/<name>/select_bg.png
Backgrounds.RenderAll(friends, outDir);
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

    public static void Name(Canvas card, string text, bool locked)
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
                var c = y == TextTop ? Col.Hex("ffffff") : Col.Hex("eef1f6").Lerp(Col.Hex("9ba3b0"), t);
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
