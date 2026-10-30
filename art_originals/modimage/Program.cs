// The Steam Workshop image (600 x 600), no names: six equal bust tiles, 3 across and 2 rows. The four brothers first
// (Daniel, David, Joshua on top; Tim bottom left), then the two friends (Honorary Brothers) on their charcoal
// select-button colors with steel rings. Brandi is left out on purpose (2026-10-29, her call), though she's still in the
// mod. Each tile's colors are sampled from the character's select button in SpireBrothers/images/characters.
// Run: dotnet run -c Release, then copy out/mod_image.png to the Workshop uploader's image.png.
var Chars = "D:/projects/csharp/SpireBrothers/SpireBrothers/images/characters/";
const int Size = 600, Gap = 4, BustScale = 6;
AnimatedBrother[] people = [new DanielArt(), new DavidArt(), new JoshuaArt(), new TimArt(), new JohnArt(), new HuffmanArt()];
// Each tile: who, where (x, y, w, h on the 600 x 600 image), and the portrait's pixel scale.
var tiles = people.Select((b, i) => (who: b, x: i % 3 * 200, y: i / 3 * 300, w: 200, h: 300, scale: BustScale)).ToArray();
var img = new Canvas(Size, Size);
for (int y = 0; y < Size; y++) for (int x = 0; x < Size; x++) img.Set(x, y, Col.Hex("16141c"));

foreach (var (b, ox, oy, TileW, TileH, S) in tiles)
{
    bool friend = b is JohnArt or HuffmanArt;
    b.Portrait = true;
    var frame = b.Render(b.Anims()["idle"].poses[0]);
    var button = PngRead.Load(Chars + $"{b.Name}/select.png");
    Col top = button.Get(0, 0)!.Value, low = button.Get(0, 150)!.Value;
    // friends: a touch lighter at the top so the charcoal doesn't read as an empty tile
    if (friend) top = top.Lerp(Col.Hex("5a606c"), 0.35f);
    var ringCol = friend ? Col.Hex("bec4ce") : new Col(255, 255, 255);
    float ringAmt = friend ? 0.16f : 0.12f;

    // half a gap on every side that touches another tile
    int x0 = ox + (ox == 0 ? 0 : Gap / 2), x1 = ox + TileW - (ox + TileW == Size ? 0 : Gap / 2);
    int y0 = oy + (oy == 0 ? 0 : Gap / 2), y1 = oy + TileH - (oy + TileH == Size ? 0 : Gap / 2);
    float cx = ox + TileW / 2f, cy = oy + TileH * 0.38f;

    // gradient plus soft rings around the face, like the select buttons
    for (int y = y0; y < y1; y++) for (int x = x0; x < x1; x++)
    {
        var g = top.Lerp(low, (y - y0) / (float)(y1 - y0));
        float r = MathF.Sqrt((x + 0.5f - cx) * (x + 0.5f - cx) + (y + 0.5f - cy) * (y + 0.5f - cy));
        float m = r % 26f;
        float ring = Math.Clamp(Math.Min(m - 20f, 26f - m + 0.5f), 0, 1);
        img.Set(x, y, g.Lerp(ringCol, ring * ringAmt));
    }
    // friends get a thin steel frame, like their select buttons' steel rim
    if (friend)
        for (int y = y0; y < y1; y++) for (int x = x0; x < x1; x++)
            if (x - x0 < 2 || x1 - 1 - x < 2 || y - y0 < 2 || y1 - 1 - y < 2) img.Set(x, y, Col.Hex("8a909c"));

    // portrait: head down to the waist (frame x 8..51, y 2..71) at the tile's scale, centered, with a drop shadow, standing
    // on the tile bottom; in a short tile the head stays near the top and the body is cut off instead
    const int X0 = 8, Y0 = 2, CW = 44, CH = 70;
    int px = (int)(cx - CW * S / 2f), py = Math.Max(y1 - CH * S + 6, y0 + 4 * S);
    Col? Port(int x, int y) => frame.Get(X0 + x / S, Y0 + y / S);
    for (int pass = 0; pass < 2; pass++)
        for (int y = 0; y < CH * S; y++) for (int x = 0; x < CW * S; x++)
        {
            if (Port(x, y) is not { } c) continue;
            int tx = px + x + (pass == 0 ? 5 : 0), ty = py + y + (pass == 0 ? 5 : 0);
            if (tx < x0 || tx >= x1 || ty < y0 || ty >= y1) continue;
            img.Set(tx, ty, pass == 0 ? img.Get(tx, ty)!.Value.Lerp(new Col(0, 0, 0), 0.35f) : c);
        }
}
Directory.CreateDirectory("out");
img.SavePng("out/mod_image.png");
Console.WriteLine("out/mod_image.png");
