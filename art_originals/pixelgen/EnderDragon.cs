// The Ender Dragon (Minecraft), the second Act 3 boss, and her End Crystals. Blocky like the Night Falls mobs (Px helpers in
// Mobs.cs). Rendered by mobgen (ender_dragon end_crystal end_crystal_caged), which also writes the dragon's map icon
// (images/map/ender_dragon_boss_icon.png + _outline.png). Drawn facing right; the game mirrors them to face the party.

public static class Tri
{
    /// <summary>A filled triangle.</summary>
    public static void Fill(Canvas c, V a, V b, V d, Col col)
    {
        int x0 = (int)MathF.Floor(MathF.Min(a.X, MathF.Min(b.X, d.X))), x1 = (int)MathF.Ceiling(MathF.Max(a.X, MathF.Max(b.X, d.X)));
        int y0 = (int)MathF.Floor(MathF.Min(a.Y, MathF.Min(b.Y, d.Y))), y1 = (int)MathF.Ceiling(MathF.Max(a.Y, MathF.Max(b.Y, d.Y)));
        float area = (b.X - a.X) * (d.Y - a.Y) - (d.X - a.X) * (b.Y - a.Y);
        if (MathF.Abs(area) < 0.01f) return;
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                float px = x + 0.5f, py = y + 0.5f;
                float w0 = ((b.X - px) * (d.Y - py) - (d.X - px) * (b.Y - py)) / area;
                float w1 = ((d.X - px) * (a.Y - py) - (a.X - px) * (d.Y - py)) / area;
                float w2 = 1 - w0 - w1;
                if (w0 >= 0 && w1 >= 0 && w2 >= 0) c.Set(x, y, col);
            }
    }
}

// 128 x 76. Black scales in mottled blocks, grey spine plates down her back and tail, glowing purple eyes, and big dark
// wings that flap. Attack: lunges, jaws open. Cast (Fireball / roar): rears her head and a purple fireball grows in her
// jaws. Dead: beams of light burst out of her and she fades away.
public class EnderDragonArt : IFrameArt
{
    public string Name => "ender_dragon";
    const int W = 128, H = 76;
    static readonly Col[] Scales = [Col.Hex("1a1a20"), Col.Hex("222228"), Col.Hex("16161a"), Col.Hex("2a2a32")];
    static readonly Col Plate = Col.Hex("5a5a66"), Membrane = Col.Hex("2c2834"), MembraneFar = Col.Hex("201c26"), Bone = Col.Hex("44404e");
    static readonly Col Eye = Col.Hex("e070ff"), EyeCore = Col.Hex("fbe0ff"), Fire = Col.Hex("c050f0"), FireCore = Col.Hex("f4c8ff");
    static readonly Col Light = Col.Hex("fff8e8");

    record struct P(int Flap = 0, int Dx = 0, int Lift = 0, int Jaw = 0, float Ball = 0, float Red = 0, float Rays = 0, float Fade = 0);
    // Flap: wing position, 0 up .. 3 down. Lift: head raised. Ball: fireball size 0..1. Rays/Fade: death.

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(Flap: 0), new(Flap: 1), new(Flap: 2), new(Flap: 3), new(Flap: 2), new(Flap: 1) }.Select(Draw).ToList(),
        ["attack"] = new P[] { new(Flap: 0, Dx: -3), new(Flap: 2, Dx: 6, Jaw: 3), new(Flap: 3, Dx: 8, Jaw: 3), new(Flap: 2, Dx: 3, Jaw: 1), new(Flap: 1) }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Flap: 0, Lift: 2, Jaw: 2, Ball: 0.3f), new(Flap: 1, Lift: 4, Jaw: 3, Ball: 0.6f), new(Flap: 2, Lift: 4, Jaw: 3, Ball: 1f), new(Flap: 3, Lift: 1, Jaw: 2), new(Flap: 2) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Flap: 1, Dx: -3, Red: 0.4f), new(Flap: 2, Dx: -1, Red: 0.2f), new(Flap: 1) }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Flap: 1, Lift: 3, Jaw: 3, Rays: 0.3f), new(Flap: 0, Lift: 4, Jaw: 3, Rays: 0.7f, Fade: 0.3f), new(Flap: 0, Lift: 4, Jaw: 3, Rays: 1f, Fade: 0.65f), new(Flap: 0, Lift: 4, Jaw: 3, Rays: 1f, Fade: 0.95f) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        int dx = p.Dx;
        int bx0 = 40 + dx, bx1 = 74 + dx, by0 = 34, by1 = 48;   // the body
        // wing tips by flap: up high, level, low
        float tipY = p.Flap switch { 0 => 2, 1 => 14, 2 => 30, _ => 46 };
        // far wing (behind everything)
        Tri.Fill(c, new V(bx0 + 14, by0 + 1), new V(bx0 + 28, by0 + 1), new V(bx0 + 2, tipY + 4), MembraneFar);
        // tail: block segments trailing left, waving gently, with a plate on each
        for (int i = 0; i < 9; i++)
        {
            int sx = bx0 - 4 - i * 4;
            int sy = by0 + 6 + (int)MathF.Round(MathF.Sin((i + p.Flap) * 0.7f) * 2) + i / 3;
            int r = i < 4 ? 3 : 2;
            Px.Mottled(c, sx - 2, sy - r, sx + 2, sy + r, Scales, 50 + i);
            c.Set(sx, sy - r - 1, Plate);
        }
        // legs: hind and fore, hanging a little under her
        Px.Mottled(c, bx0 + 4, by1, bx0 + 8, by1 + 10, Scales, 61);
        Px.Mottled(c, bx0 + 2, by1 + 10, bx0 + 9, by1 + 12, Scales, 62);
        Px.Mottled(c, bx1 - 9, by1, bx1 - 5, by1 + 8, Scales, 63);
        Px.Mottled(c, bx1 - 10, by1 + 8, bx1 - 3, by1 + 10, Scales, 64);
        // body, with spine plates along the back
        Px.Mottled(c, bx0, by0, bx1, by1, Scales, 70);
        for (int x = bx0 + 2; x < bx1; x += 5) Px.Rect(c, x, by0 - 2, x + 1, by0 - 1, Plate);
        // neck: blocks rising up and forward to the head
        int hx = 96 + dx, hy = 16 - p.Lift;   // the head's top-left
        for (int i = 0; i < 6; i++)
        {
            float t = i / 5f;
            int nx = (int)(bx1 - 2 + (hx - bx1 + 2) * t), ny = (int)(by0 + 2 + (hy + 4 - by0 - 2) * t);
            Px.Mottled(c, nx - 3, ny - 3, nx + 3, ny + 3, Scales, 80 + i);
            c.Set(nx, ny - 4, Plate);
        }
        // head: a long box with a snout, nostrils, and the jaw below (drops by Jaw)
        Px.Mottled(c, hx, hy, hx + 18, hy + 9, Scales, 90);
        Px.Mottled(c, hx + 19, hy + 4, hx + 26, hy + 9, Scales, 91);
        c.Set(hx + 24, hy + 5, Col.Hex("0a0a0c"));
        Px.Rect(c, hx + 2, hy - 2, hx + 3, hy - 1, Plate); Px.Rect(c, hx + 7, hy - 2, hx + 8, hy - 1, Plate);
        Px.Mottled(c, hx + 4, hy + 10 + p.Jaw, hx + 25, hy + 12 + p.Jaw, Scales, 92);
        if (p.Jaw > 0) Px.Rect(c, hx + 6, hy + 10, hx + 24, hy + 9 + p.Jaw, Col.Hex("3a1040"));
        // eyes: a purple glow
        Px.Rect(c, hx + 11, hy + 3, hx + 14, hy + 4, Eye);
        Px.Rect(c, hx + 12, hy + 3, hx + 13, hy + 3, EyeCore);
        // near wing (in front of the body): bone along the leading edge, membrane below
        var shoulder = new V(bx0 + 16, by0 + 2);
        var elbow = new V(bx0 + 4, (tipY + by0) / 2f - 4);
        var tip = new V(bx0 - 14, tipY);
        Tri.Fill(c, shoulder, new V(bx0 + 32, by0 + 2), elbow, Membrane);
        Tri.Fill(c, elbow, new V(bx0 + 32, by0 + 2), tip, Membrane);
        Tri.Fill(c, elbow, tip, new V(bx0 + 10, by0 + 4), Membrane);
        Px.Line(c, shoulder, elbow, Bone, 2);
        Px.Line(c, elbow, tip, Bone, 2);
        // the fireball in her jaws
        if (p.Ball > 0)
        {
            var at = new V(hx + 30, hy + 10);
            Px.Puff(c, at, 2 + 5 * p.Ball, Fire);
            Px.Puff(c, at, 1 + 2.5f * p.Ball, FireCore);
        }
        Px.Outline(c);
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        if (p.Fade > 0) Px.Tint(c, Light, p.Fade);
        // death: rays of light bursting out of her body
        if (p.Rays > 0)
        {
            var center = new V((bx0 + bx1) / 2f, by0 + 4);
            for (int i = 0; i < 9; i++)
            {
                float ang = -MathF.PI * (0.1f + i * 0.1f) + (i % 2) * 0.15f;
                float len = (20 + (i * 37 % 23)) * p.Rays;
                var end = center + new V(MathF.Cos(ang), MathF.Sin(ang)) * len;
                Px.Line(c, center, end, Light, i % 3 == 0 ? 2 : 1);
            }
        }
        if (p.Fade >= 0.9f)
        {
            // nearly gone: keep only the light, thinned out
            for (int i = 0; i < c.Px.Length; i++)
                if (c.Px[i] is { } px && px != Light && (i * 2654435761u % 5) != 0) c.Px[i] = null;
        }
        return c;
    }
}

// 32 x 100. An End Crystal on an obsidian pillar: a pink glass cube turning inside a frame, sitting on a bedrock block on
// top of a tall pillar. The caged one has iron bars around it. Cast (Healing Beam): it glows and a pink beam goes up
// toward the dragon (behind it, so the beam points left in the drawing and right on screen). Dead: blows up, leaving the
// bedrock with a little fire on it.
public class EndCrystalArt(bool caged) : IFrameArt
{
    public string Name => caged ? "end_crystal_caged" : "end_crystal";
    const int W = 32, H = 100;
    static readonly Col[] Obsidian = [Col.Hex("140f1c"), Col.Hex("1c1428"), Col.Hex("100c16"), Col.Hex("2a1e3a")];
    static readonly Col[] Bedrock = [Col.Hex("4a4a4a"), Col.Hex("3a3a3a"), Col.Hex("5c5c5c"), Col.Hex("2c2c2c")];
    static readonly Col Glass = Col.Hex("e070c0"), GlassHi = Col.Hex("ffc8f0"), Frame = Col.Hex("8a3a8a"), Iron = Col.Hex("9aa0a8");
    static readonly Col Beam = Col.Hex("ffb0f0"), FireCol = Col.Hex("ffa030"), FireHi = Col.Hex("fff070");

    record struct P(int Turn = 0, int Bob = 0, float Glow = 0, float Red = 0, int Boom = 0);
    // Turn: 0 square, 1 diamond. Boom: 0 whole, 1 exploding, 2 big puff, 3 gone (fire on bedrock).

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(Turn: 0), new(Turn: 1, Bob: 1), new(Turn: 0, Bob: 2), new(Turn: 1, Bob: 1) }.Select(Draw).ToList(),
        ["attack"] = new P[] { new(Glow: 0.5f), new(Turn: 1, Glow: 1), new() }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Glow: 0.4f), new(Turn: 1, Bob: 1, Glow: 1), new(Turn: 0, Bob: 2, Glow: 1), new(Turn: 1, Bob: 1, Glow: 0.4f) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Red: 0.45f), new(Turn: 1, Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Boom: 1), new(Boom: 2), new(Boom: 3), new(Boom: 4) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        // the pillar and the bedrock block on top
        Px.Mottled(c, 6, 46, 25, H - 1, Obsidian, 3);
        Px.Mottled(c, 8, 40, 23, 45, Bedrock, 4);
        if (p.Boom >= 3)
        {
            // just a little fire left burning on the bedrock
            int f = p.Boom == 3 ? 0 : 1;
            Px.Rect(c, 12, 34 + f, 19, 39, FireCol);
            Px.Rect(c, 14, 31 + f, 17, 36, FireHi);
            c.Set(13 + f * 4, 32, FireCol);
            Px.Outline(c);
            return c;
        }
        int cy = 24 - p.Bob;   // the cube's center
        var glass = p.Glow > 0 ? Glass.Lerp(GlassHi, p.Glow * 0.6f) : Glass;
        if (p.Boom == 0)
        {
            // the outer frame and the glass cube inside, turning
            if (p.Turn == 0)
            {
                Px.Rect(c, 8, cy - 8, 23, cy + 7, Frame);
                Px.Rect(c, 10, cy - 6, 21, cy + 5, Col.Hex("3a1a3a"));
                Px.Rect(c, 12, cy - 4, 19, cy + 3, glass);
                Px.Rect(c, 12, cy - 4, 15, cy - 3, GlassHi);
            }
            else
            {
                Tri.Fill(c, new V(16, cy - 11), new V(27, cy), new V(16, cy + 11), Frame);
                Tri.Fill(c, new V(16, cy - 11), new V(5, cy), new V(16, cy + 11), Frame);
                Tri.Fill(c, new V(16, cy - 7), new V(23, cy), new V(16, cy + 7), glass);
                Tri.Fill(c, new V(16, cy - 7), new V(9, cy), new V(16, cy + 7), glass);
                Px.Rect(c, 14, cy - 4, 15, cy - 2, GlassHi);
            }
            if (caged)
            {
                for (int x = 5; x <= 26; x += 4) Px.Rect(c, x, cy - 13, x, 39, Iron);
                Px.Rect(c, 4, cy - 14, 27, cy - 13, Iron);
            }
            // the healing beam: up and to the left (toward the dragon once mirrored)
            if (p.Glow >= 1)
                Px.Line(c, new V(15, cy - 2), new V(0, 0), Beam, 2);
        }
        else
        {
            // the explosion
            float r = p.Boom == 1 ? 9 : 14;
            Px.Puff(c, new V(16, cy), r, p.Boom == 1 ? FireCol : Col.Hex("9a9a9a"));
            Px.Puff(c, new V(16, cy), r * 0.55f, p.Boom == 1 ? FireHi : Col.Hex("c8c8c8"));
        }
        Px.Outline(c);
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}
