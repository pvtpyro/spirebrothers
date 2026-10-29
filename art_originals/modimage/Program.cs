// The Steam Workshop image (600 x 600): all seven characters, no names. The four brothers on the top row; Brandi (Daniel's
// wife) and the two friends (Honorary Brothers) on the bottom row, the friends on their charcoal select-button colors
// with steel rings. Each tile's colors are sampled from the character's select button in SpireBrothers/images/characters.
// Run: dotnet run -c Release, then copy out/mod_image.png to the Workshop uploader's image.png.
var Chars = "D:/projects/csharp/SpireBrothers/SpireBrothers/images/characters/";
AnimatedBrother[] people = [new DanielArt(), new DavidArt(), new JoshuaArt(), new TimArt(), new BrandiArt(), new JohnArt(), new HuffmanArt()];
const int Size = 600, TileH = 300, Gap = 4;
int[] rowCols = [4, 3];
var img = new Canvas(Size, Size);
for (int y = 0; y < Size; y++) for (int x = 0; x < Size; x++) img.Set(x, y, Col.Hex("16141c"));

for (int i = 0; i < people.Length; i++)
{
    var b = people[i];
    bool friend = b is JohnArt or HuffmanArt;
    b.Portrait = true;
    var frame = b.Render(b.Anims()["idle"].poses[0]);
    var button = PngRead.Load(Chars + $"{b.Name}/select.png");
    Col top = button.Get(0, 0)!.Value, low = button.Get(0, 150)!.Value;
    // friends: a touch lighter at the top so the charcoal doesn't read as an empty tile
    if (friend) top = top.Lerp(Col.Hex("5a606c"), 0.35f);
    var ringCol = friend ? Col.Hex("bec4ce") : new Col(255, 255, 255);
    float ringAmt = friend ? 0.16f : 0.12f;

    int row = i < rowCols[0] ? 0 : 1, col = row == 0 ? i : i - rowCols[0], Cols = rowCols[row], TileW = Size / Cols;
    int ox = col * TileW, oy = row * TileH;
    int x0 = ox + (col == 0 ? 0 : Gap / 2), x1 = ox + TileW - (col == Cols - 1 ? 0 : Gap / 2);
    int y0 = oy + (row == 0 ? 0 : Gap / 2), y1 = oy + TileH - (row == 0 ? Gap / 2 : 0);
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

    // portrait: head down to the waist (frame x 8..51, y 2..71) at 4x, centered, with a drop shadow, cut off at the tile bottom
    const int S = 4, X0 = 8, Y0 = 2, CW = 44, CH = 70;
    int px = (int)(cx - CW * S / 2f), py = y1 - CH * S + 6;
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
