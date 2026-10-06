using System.IO.Compression;

public readonly record struct V(float X, float Y)
{
    public static V operator +(V a, V b) => new(a.X + b.X, a.Y + b.Y);
    public static V operator -(V a, V b) => new(a.X - b.X, a.Y - b.Y);
    public static V operator *(V a, float k) => new(a.X * k, a.Y * k);
    public static V operator /(V a, float k) => new(a.X / k, a.Y / k);
    public float Len => MathF.Sqrt(X * X + Y * Y);
    public V Norm => Len < 1e-5f ? new V(0, 0) : this / Len;
    public float Dot(V o) => X * o.X + Y * o.Y;
    public V Perp => new(-Y, X);
    public static V Dir(float deg) => new(MathF.Cos(deg * MathF.PI / 180f), MathF.Sin(deg * MathF.PI / 180f));
    public V Rot(float deg) { var c = MathF.Cos(deg * MathF.PI / 180f); var s = MathF.Sin(deg * MathF.PI / 180f); return new(X * c - Y * s, X * s + Y * c); }
}

public readonly record struct Col(byte R, byte G, byte B, byte A = 255)
{
    public static Col Hex(string h) => new(Convert.ToByte(h[..2], 16), Convert.ToByte(h[2..4], 16), Convert.ToByte(h[4..6], 16));
    public Col Mul(float k) => new((byte)Math.Clamp(R * k, 0, 255), (byte)Math.Clamp(G * k, 0, 255), (byte)Math.Clamp(B * k, 0, 255), A);
    public Col Lerp(Col o, float t) => new((byte)(R + (o.R - R) * t), (byte)(G + (o.G - G) * t), (byte)(B + (o.B - B) * t), A);
    // Shading shifts slightly toward cool purple, highlights toward warm; reads more "pixel art" than plain darkening.
    public Col Shade => Lerp(new Col(40, 30, 70), 0.32f);
    public Col Light => Lerp(new Col(255, 245, 220), 0.28f);
}

public delegate float Sdf(V p);

public static class S
{
    public static float Circle(V p, V c, float r) => (p - c).Len - r;
    public static float Ellipse(V p, V c, float rx, float ry)
    {
        var d = new V((p.X - c.X) / rx, (p.Y - c.Y) / ry);
        return (d.Len - 1) * MathF.Min(rx, ry);
    }
    public static float SegDist(V p, V a, V b, out float t)
    {
        var ab = b - a; var ap = p - a;
        t = Math.Clamp(ap.Dot(ab) / MathF.Max(ab.Dot(ab), 1e-5f), 0, 1);
        return (p - (a + ab * t)).Len;
    }
    public static float Capsule(V p, V a, V b, float r) => SegDist(p, a, b, out _) - r;
    public static float Taper(V p, V a, V b, float ra, float rb) { var d = SegDist(p, a, b, out var t); return d - (ra + (rb - ra) * t); }
    public static float Box(V p, float x0, float y0, float x1, float y1)
    {
        var cx = (x0 + x1) / 2; var cy = (y0 + y1) / 2; var hx = (x1 - x0) / 2; var hy = (y1 - y0) / 2;
        var dx = MathF.Abs(p.X - cx) - hx; var dy = MathF.Abs(p.Y - cy) - hy;
        var outside = new V(MathF.Max(dx, 0), MathF.Max(dy, 0)).Len;
        return outside + MathF.Min(MathF.Max(dx, dy), 0);
    }
    /// <summary>Box rotated: centered at c, half sizes hx (along dir) and hy, rotated by deg.</summary>
    public static float RBox(V p, V c, float hx, float hy, float deg)
    {
        var q = (p - c).Rot(-deg);
        return Box(q, -hx, -hy, hx, hy);
    }
    public static float Poly(V p, params V[] v)
    {
        float d = float.MaxValue; bool inside = false;
        for (int i = 0, j = v.Length - 1; i < v.Length; j = i++)
        {
            d = MathF.Min(d, SegDist(p, v[j], v[i], out _));
            if ((v[i].Y > p.Y) != (v[j].Y > p.Y) && p.X < (v[j].X - v[i].X) * (p.Y - v[i].Y) / (v[j].Y - v[i].Y) + v[i].X) inside = !inside;
        }
        return inside ? -d : d;
    }
    public static float U(float a, float b) => MathF.Min(a, b);
    public static float I(float a, float b) => MathF.Max(a, b);
    public static float Sub(float a, float b) => MathF.Max(a, -b);
}

public class Canvas
{
    public readonly int W, H;
    public readonly Col?[] Px;
    public static readonly Col Outline = Col.Hex("1c1624");

    // Whole-body transform: scale about the feet (Joshua is shorter) plus an offset.
    public float K = 1f;
    public V Origin;
    public V Offset;

    public Canvas(int w, int h) { W = w; H = h; Px = new Col?[w * h]; Origin = new V(w / 2f, h); }

    public V ModelOf(V pix) => ToModel(pix);
    private V ToModel(V pix) => Origin + (pix - Origin - Offset) / K;
    private V ToPix(V model) => Origin + (model - Origin) * K + Offset;

    public void Set(int x, int y, Col c) { if (x >= 0 && y >= 0 && x < W && y < H) Px[y * W + x] = c; }
    public Col? Get(int x, int y) => x >= 0 && y >= 0 && x < W && y < H ? Px[y * W + x] : null;

    /// <summary>Plot one model-space point as a pixel.</summary>
    public void Plot(V p, Col c) { var q = ToPix(p); Set((int)MathF.Floor(q.X), (int)MathF.Floor(q.Y), c); }
    public void Plot(float x, float y, Col c) => Plot(new V(x, y), c);

    /// <summary>Fill a shape: outlined, shaded on the lower-right rim, optionally highlighted on the upper-left rim.</summary>
    public void Draw(Sdf s, Col baseC, bool outline = true, Col? shade = null, Col? hi = null, float shadeOff = 1.2f, bool noShade = false, Col? line = null)
    {
        var sh = shade ?? baseC.Shade;
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                var pc = new V(x + 0.5f, y + 0.5f);
                var d = s(ToModel(pc)) * K;
                if (d <= 0)
                {
                    var c = baseC;
                    if (!noShade && s(ToModel(pc + new V(shadeOff, shadeOff))) > 0) c = sh;
                    else if (hi.HasValue && s(ToModel(pc - new V(1.1f, 1.1f))) > 0) c = hi.Value;
                    Set(x, y, c);
                }
                else if (outline && d <= 1.0f) Set(x, y, line ?? Outline);
            }
    }

    /// <summary>Fill without outline or shading (details, glows).</summary>
    public void Flat(Sdf s, Col c) => Draw(s, c, outline: false, noShade: true);

    public void Overlay(Canvas o)
    {
        for (int i = 0; i < Px.Length; i++) if (o.Px[i].HasValue) Px[i] = o.Px[i];
    }

    public void SavePng(string path, int scale = 1, Col? bg = null)
    {
        int w = W * scale, h = H * scale;
        var raw = new byte[h * (w * 4 + 1)];
        for (int y = 0; y < h; y++)
        {
            int row = y * (w * 4 + 1);
            raw[row] = 0;
            for (int x = 0; x < w; x++)
            {
                var c = Px[(y / scale) * W + x / scale] ?? bg ?? new Col(0, 0, 0, 0);
                int o = row + 1 + x * 4;
                raw[o] = c.R; raw[o + 1] = c.G; raw[o + 2] = c.B; raw[o + 3] = c.A;
            }
        }
        Png.Write(path, w, h, raw);
    }
}

public static class Png
{
    private static readonly uint[] Crc = MakeCrc();
    private static uint[] MakeCrc()
    {
        var t = new uint[256];
        for (uint n = 0; n < 256; n++) { uint c = n; for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1; t[n] = c; }
        return t;
    }
    private static uint CrcOf(byte[] type, byte[] data)
    {
        uint c = 0xFFFFFFFF;
        foreach (var b in type) c = Crc[(c ^ b) & 0xFF] ^ (c >> 8);
        foreach (var b in data) c = Crc[(c ^ b) & 0xFF] ^ (c >> 8);
        return c ^ 0xFFFFFFFF;
    }
    private static void Chunk(Stream s, string type, byte[] data)
    {
        var t = System.Text.Encoding.ASCII.GetBytes(type);
        WriteBE(s, (uint)data.Length); s.Write(t); s.Write(data); WriteBE(s, CrcOf(t, data));
    }
    private static void WriteBE(Stream s, uint v) { s.WriteByte((byte)(v >> 24)); s.WriteByte((byte)(v >> 16)); s.WriteByte((byte)(v >> 8)); s.WriteByte((byte)v); }

    public static void Write(string path, int w, int h, byte[] raw)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using var f = File.Create(path);
        f.Write([137, 80, 78, 71, 13, 10, 26, 10]);
        var ihdr = new MemoryStream();
        WriteBE(ihdr, (uint)w); WriteBE(ihdr, (uint)h);
        ihdr.Write([8, 6, 0, 0, 0]);
        Chunk(f, "IHDR", ihdr.ToArray());
        var z = new MemoryStream();
        using (var zs = new ZLibStream(z, CompressionLevel.SmallestSize, true)) zs.Write(raw);
        Chunk(f, "IDAT", z.ToArray());
        Chunk(f, "IEND", []);
    }
}

public static class CanvasExt
{
    /// <summary>Recolor pixels inside a shape (skipping its 1px rim) with a pixel pattern, e.g. plaid.</summary>
    public static void Pattern(this Canvas c, Sdf s, Func<int, int, Col?> f)
    {
        for (int y = 0; y < c.H; y++)
            for (int x = 0; x < c.W; x++)
            {
                var q = c.ModelOf(new V(x + 0.5f, y + 0.5f));
                if (s(q) * c.K <= -1.2f && f(x, y) is { } col) c.Set(x, y, col);
            }
    }
}

public static class PngRead
{
    /// <summary>Decode an 8-bit RGB/RGBA/palette-free PNG into a Canvas.</summary>
    public static Canvas Load(string path)
    {
        var bytes = File.ReadAllBytes(path);
        int pos = 8, w = 0, h = 0, colorType = 0, depth = 0;
        var idat = new MemoryStream();
        byte[]? plte = null; byte[]? trns = null;
        while (pos < bytes.Length)
        {
            int len = (bytes[pos] << 24) | (bytes[pos + 1] << 16) | (bytes[pos + 2] << 8) | bytes[pos + 3];
            var type = System.Text.Encoding.ASCII.GetString(bytes, pos + 4, 4);
            var data = bytes.AsSpan(pos + 8, len);
            if (type == "IHDR") { w = (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]; h = (data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]; depth = data[8]; colorType = data[9]; }
            else if (type == "IDAT") idat.Write(data);
            else if (type == "PLTE") plte = data.ToArray();
            else if (type == "tRNS") trns = data.ToArray();
            pos += 12 + len;
        }
        if (depth != 8) throw new Exception($"depth {depth} unsupported");
        int bpp = colorType switch { 6 => 4, 2 => 3, 3 => 1, 0 => 1, 4 => 2, _ => throw new Exception($"color type {colorType}") };
        idat.Position = 0;
        using var z = new System.IO.Compression.ZLibStream(idat, System.IO.Compression.CompressionMode.Decompress);
        var raw = new MemoryStream(); z.CopyTo(raw);
        var r = raw.ToArray();
        int stride = w * bpp;
        var cur = new byte[stride]; var prev = new byte[stride];
        var c = new Canvas(w, h);
        int p = 0;
        for (int y = 0; y < h; y++)
        {
            int f = r[p++];
            for (int i = 0; i < stride; i++)
            {
                int a = i >= bpp ? cur[i - bpp] : 0, b = prev[i], cc = i >= bpp ? prev[i - bpp] : 0;
                int x = r[p++];
                cur[i] = (byte)(f switch
                {
                    0 => x, 1 => x + a, 2 => x + b, 3 => x + ((a + b) >> 1),
                    _ => x + Paeth(a, b, cc)
                });
            }
            for (int x = 0; x < w; x++)
            {
                int o = x * bpp;
                Col col = colorType switch
                {
                    6 => new Col(cur[o], cur[o + 1], cur[o + 2], cur[o + 3]),
                    2 => new Col(cur[o], cur[o + 1], cur[o + 2]),
                    0 => new Col(cur[o], cur[o], cur[o]),
                    4 => new Col(cur[o], cur[o], cur[o], cur[o + 1]),
                    _ => new Col(plte![cur[o] * 3], plte[cur[o] * 3 + 1], plte[cur[o] * 3 + 2], trns != null && cur[o] < trns.Length ? trns[cur[o]] : (byte)255)
                };
                c.Set(x, y, col);
            }
            (cur, prev) = (prev, cur);
        }
        return c;
    }

    private static int Paeth(int a, int b, int c)
    {
        int p = a + b - c, pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
    }
}
