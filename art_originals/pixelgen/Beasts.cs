// Foes that aren't people, drawn straight onto the canvas instead of on the Rig: the Lion and Lioness (Daniel's
// Lions' Den) and the Wall of Jericho. Facing right like everything else; the game mirrors them.
// Each returns its frames per animation; Program.cs saves them like the others.

public interface IFrameArt
{
    string Name { get; }
    Dictionary<string, List<Canvas>> Frames();
}

// ---------------------------------------------------------------- Lion / Lioness
// 72 x 48. Body, four legs (the far pair darker), a tail with a tuft, and a head; the Lion has a big dark mane.
public class LionArt(bool mane) : IFrameArt
{
    public string Name => mane ? "lion" : "lioness";
    const int W = 72, H = 48;
    Col Fur => mane ? Col.Hex("d8a050") : Col.Hex("dcb070");
    static readonly Col Mane = Col.Hex("8a4a1c");
    static readonly Col Nose = Col.Hex("5a2a20");

    record struct LionPose(float Dx = 0, float Bob = 0, float Crouch = 0, float HeadUp = 0, float Mouth = 0, float Tail = 0,
        float Reach = 0, float Tint = 0, bool Dead = false, float Lie = 0, int Fx = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 8).Select(i => Draw(new LionPose(Bob: i is 3 or 4 or 5 ? 1 : 0, Tail: MathF.Sin(i / 8f * MathF.Tau)))).ToList(),
        ["attack"] = new LionPose[]
        {
            new(Dx: -2, Crouch: 2), new(Dx: -4, Crouch: 3, Mouth: 0.5f), new(Dx: 8, HeadUp: 1, Mouth: 1, Reach: 5, Fx: 1),
            new(Dx: 7, Mouth: 1, Reach: 4), new(Dx: 3, Mouth: 0.3f, Reach: 1), new(),
        }.Select(Draw).ToList(),
        ["cast"] = new LionPose[]
        {
            new(HeadUp: 1), new(HeadUp: 3, Mouth: 0.6f), new(HeadUp: 5, Mouth: 1, Fx: 2), new(HeadUp: 5, Mouth: 1, Fx: 3),
            new(HeadUp: 3, Mouth: 0.5f, Fx: 2), new(),
        }.Select(Draw).ToList(),
        ["hit"] = new LionPose[] { new(Dx: -3, Tint: 0.35f, Mouth: 0.4f), new(Dx: -2, Mouth: 0.3f), new(Dx: -1), new() }.Select(Draw).ToList(),
        ["dead"] = new LionPose[]
        {
            new(Dx: -2, Tint: 0.35f, Mouth: 0.5f), new(Crouch: 3, Lie: 0.25f, Dead: true), new(Crouch: 6, Lie: 0.5f, Dead: true),
            new(Crouch: 8, Lie: 0.8f, Dead: true), new(Crouch: 9, Lie: 1, Dead: true),
        }.Select(Draw).ToList(),
    };

    Canvas Draw(LionPose p)
    {
        var c = new Canvas(W, H);
        float y0 = 27 + p.Bob + p.Crouch;
        var body = new V(32 + p.Dx, y0);
        var shoulder = body + new V(13, -1); var hip = body + new V(-12, 0);
        var head = body + new V(22 + p.Reach * 0.5f, -8 - p.HeadUp + p.Crouch * 0.3f);
        float legLen = 15 - p.Crouch;
        Col far = Fur.Mul(0.78f);

        // tail: a curve up from the rump, swishing, with a dark tuft
        var t0 = hip + new V(-5, -2);
        V prev = t0;
        for (int i = 1; i <= 8; i++)
        {
            float k = i / 8f;
            var pt = t0 + new V(-6 * k - 2 * k * k, -10 * k * k + p.Tail * 3 * k);
            var a = prev; var b = pt;
            c.Draw(q => S.Capsule(q, a, b, 1.1f), Fur, noShade: true);
            prev = pt;
        }
        var tip = prev;
        c.Draw(q => S.Circle(q, tip, 2.2f), Mane);

        // legs: far pair first (darker), folded under when lying down
        void Leg(V top, float dx, Col col)
        {
            var foot = new V(top.X + dx + p.Reach * (top == shoulder || top == shoulder + new V(-3, 0) ? 1 : 0), H - 2 - p.Lie * 3);
            if (p.Lie > 0) foot = new V(top.X + dx + 6 * p.Lie, MathF.Min(H - 2, top.Y + legLen * (1 - p.Lie * 0.6f)));
            c.Draw(q => S.Capsule(q, top, foot, 2.6f), col);
            c.Draw(q => S.Ellipse(q, foot + new V(1.5f, 0), 3.2f, 1.8f), col);
        }
        Leg(shoulder + new V(-3, 0), 3, far);
        Leg(hip + new V(3, 0), 2, far);

        // body
        c.Draw(q => S.Ellipse(q, body, 18, 8.5f), Fur, hi: Fur.Light);
        c.Draw(q => S.Ellipse(q, body + new V(2, 5), 13, 3), Fur.Light, outline: false, noShade: true);

        Leg(shoulder, 1, Fur);
        Leg(hip, 0, Fur);

        // mane behind the head
        if (mane)
            c.Draw(q => S.Circle(q, head + new V(-2, 1), 10.5f) + MathF.Abs(MathF.Sin(MathF.Atan2(q.Y - head.Y, q.X - head.X) * 7)) * 1.2f, Mane, hi: Mane.Light);

        // head, ears, muzzle
        c.Draw(q => S.Circle(q, head + new V(-3, -6), 2.4f), Fur);
        c.Draw(q => S.Circle(q, head, 6.5f), Fur, hi: Fur.Light);
        var muzzle = head + new V(5.5f, 3);
        c.Draw(q => S.Ellipse(q, muzzle, 4.2f, 3f), Fur.Light);
        c.Plot(muzzle + new V(3, -1.5f), Nose); c.Plot(muzzle + new V(2, -1.5f), Nose);
        if (p.Mouth > 0)
        {
            float open = 1 + p.Mouth * 2.5f;
            c.Draw(q => S.Poly(q, muzzle + new V(-1, 1), muzzle + new V(5, 0.5f), muzzle + new V(4, 1 + open)), Col.Hex("5a1414"), outline: false);
            c.Plot(muzzle + new V(3.5f, 1), Col.Hex("ffffff")); c.Plot(muzzle + new V(3, 1 + open - 0.5f), Col.Hex("ffffff"));
        }
        // eye
        var eye = head + new V(2.5f, -1.5f);
        if (p.Dead) { c.Plot(eye + new V(-1, -1), Brother.Eye); c.Plot(eye, Brother.Eye); c.Plot(eye + new V(1, 1), Brother.Eye); c.Plot(eye + new V(1, -1), Brother.Eye); c.Plot(eye + new V(-1, 1), Brother.Eye); }
        else if (p.Mouth >= 1 || p.Fx == 1) { c.Plot(eye, Brother.Eye); c.Plot(eye + new V(1, 0), Brother.Eye); c.Plot(eye + new V(-1, -1), Fur.Shade); c.Plot(eye + new V(1, -1.5f), Fur.Shade); }
        else { c.Plot(eye, Brother.Eye); c.Plot(eye + new V(0, 1), Brother.Eye); c.Plot(eye + new V(1, 0), Col.Hex("e8c040")); }

        // roar lines and a pounce streak
        if (p.Fx is 2 or 3)
            for (int i = 0; i < 3; i++)
            {
                float r = 5 + i * 3 + (p.Fx - 2) * 2;
                for (float a = -40; a <= 40; a += 10) c.Plot(muzzle + new V(2, 0) + V.Dir(a) * r, Col.Hex("fff4d8"));
            }
        if (p.Fx == 1) for (int i = 0; i < 4; i++) c.Plot(body + new V(-20 - i * 2, -4 + i * 3), Col.Hex("fff4d8"));

        if (p.Tint > 0) Tint(c, p.Tint);
        return c;
    }

    internal static void Tint(Canvas c, float t)
    {
        for (int i = 0; i < c.Px.Length; i++)
            if (c.Px[i] is { } px && px != Canvas.Outline) c.Px[i] = px.Lerp(Col.Hex("ff4040"), t);
    }
}

// ---------------------------------------------------------------- Wall of Jericho
// 64 x 80. A stretch of mud-brick wall with crenellations, a wooden gate with bronze bands, a red banner that flaps,
// and red cords hanging from a window (Rahab's scarlet cord, Joshua 2:18).
public class WallArt : IFrameArt
{
    public string Name => "wall_of_jericho";
    const int W = 64, H = 80;
    static readonly Col Brick = Col.Hex("b8916a");
    static readonly Col Mortar = Col.Hex("7a5a40");
    static readonly Col Gate = Col.Hex("6a4224");
    static readonly Col Band = Col.Hex("c08a3a");
    static readonly Col Banner = Col.Hex("c0302a");

    // Rocks: 0 = none, 1.. = how far the falling rocks have got.
    record struct WallPose(float Shake = 0, float Tint = 0, int Flap = 0, float Glow = 0, int Rocks = 0, float Fall = 0, int Cracks = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 4).Select(i => Draw(new WallPose(Flap: i))).ToList(),
        ["attack"] = Enumerable.Range(0, 6).Select(i => Draw(new WallPose(Flap: i % 4, Rocks: i + 1, Shake: i == 1 ? 1 : 0))).ToList(),
        ["cast"] = new[] { 0f, 0.4f, 0.8f, 1f, 0.6f, 0.2f }.Select((g, i) => Draw(new WallPose(Flap: i % 4, Glow: g))).ToList(),
        ["hit"] = new WallPose[] { new(Shake: 2, Tint: 0.3f, Cracks: 1), new(Shake: -1, Cracks: 1), new(Shake: 1), new() }.Select(Draw).ToList(),
        ["dead"] = Enumerable.Range(0, 5).Select(i => Draw(new WallPose(Shake: i % 2 == 0 ? 1 : -1, Fall: i / 4f, Cracks: 3, Tint: i == 0 ? 0.3f : 0))).ToList(),
    };

    Canvas Draw(WallPose p)
    {
        var c = new Canvas(W, H);
        float top = 22 + p.Fall * 40;      // the wall sinks into a heap of rubble as it falls
        float left = 4 + p.Shake, right = 60 + p.Shake;

        // the wall face, with crenellations on top
        c.Draw(q =>
        {
            float wall = S.Box(q, left, top, right, H);
            for (int i = 0; i < 5; i++)
            {
                float x = left + 1 + i * 12;
                wall = S.U(wall, S.Box(q, x, top - 6, x + 7, top + 1));
            }
            return wall;
        }, Brick, hi: Brick.Light);
        // brick rows
        for (float y = top + 4; y < H - 1; y += 5)
        {
            for (float x = left + 1; x < right; x++) c.Plot(x, y, Mortar);
            float off = ((int)((y - top) / 5) % 2) * 5;
            for (float x = left + 3 + off; x < right; x += 10)
                for (float yy = y + 1; yy < y + 5 && yy < H - 1; yy++) c.Plot(x, yy, Mortar);
        }
        if (p.Glow > 0)
            c.Pattern(q => S.Box(q, left, top, right, H), (x, y) => (x + y) % 5 == 0 ? Brick.Lerp(Col.Hex("fff4d8"), p.Glow * 0.7f) : null);

        if (p.Fall < 0.6f)
        {
            // the gate: an arch of wood with bronze bands
            float gTop = MathF.Max(top + 22, 46);
            c.Draw(q => S.U(S.Box(q, 22 + p.Shake, gTop, 42 + p.Shake, H), S.Circle(q, new V(32 + p.Shake, gTop), 10)), Gate, hi: Gate.Light);
            for (float y = gTop + 4; y < H; y += 9) for (float x = 23 + p.Shake; x < 42 + p.Shake; x++) c.Plot(x, y, Band);
            c.Draw(q => S.Box(q, 31.5f + p.Shake, gTop - 9, 32.5f + p.Shake, H), Gate.Shade, outline: false, noShade: true);
            // a window with the scarlet cord hanging out
            var win = new V(50 + p.Shake, top + 12);
            c.Draw(q => S.Box(q, win.X - 3, win.Y - 4, win.X + 3, win.Y + 3), Col.Hex("2a1c14"));
            for (float y = win.Y + 3; y < win.Y + 16; y++) c.Plot(win.X + MathF.Sin(y * 0.6f + p.Flap) * 0.6f, y, Banner);
            // the banner on a pole over the left tower
            var pole = new V(10 + p.Shake, top - 6);
            c.Draw(q => S.Capsule(q, pole, pole + new V(0, -12), 0.5f), Gate, noShade: true);
            float wave = p.Flap switch { 0 => 0, 1 => 1, 2 => 0, _ => -1 };
            c.Draw(q => S.Poly(q, pole + new V(0.5f, -12), pole + new V(11, -10 + wave), pole + new V(0.5f, -6)), Banner, hi: Banner.Light);
        }

        // cracks spreading as it's hurt
        for (int k = 0; k < p.Cracks; k++)
        {
            var at = new V(14 + k * 16 + p.Shake, top + 8 + k * 6);
            for (int i = 0; i < 8; i++) c.Plot(at + new V(i * (k % 2 == 0 ? 1 : -1) * 0.7f, i), Canvas.Outline);
        }

        // rubble: rocks tumbling off the top toward the party, or the heap once it has fallen
        if (p.Rocks > 0)
            for (int i = 0; i < 3; i++)
            {
                float t = (p.Rocks - 1) / 5f + i * 0.12f;
                var rock = new V(48 + t * 18, top - 4 + t * t * 60);
                c.Draw(q => S.Circle(q, rock, 2.4f), Brick.Mul(0.85f));
            }
        if (p.Fall > 0)
            for (int i = 0; i < 9; i++)
            {
                var rock = new V(6 + i * 6.5f + p.Shake, H - 3 - (i % 3) * 2 * p.Fall);
                c.Draw(q => S.Circle(q, rock, 3f), Brick.Mul(0.9f), hi: Brick.Light);
            }

        if (p.Tint > 0) LionArt.Tint(c, p.Tint);
        return c;
    }
}
