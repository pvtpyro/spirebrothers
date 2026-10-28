// Regular-fight foes (not elites). Drawn pixel by pixel in their own frames like the lions (IFrameArt), facing right;
// the game mirrors them to face the party. Rendered by art_originals/mobgen (only these, so nothing else is redrawn).

/// <summary>Shared helpers for blocky, hand-placed pixel art.</summary>
public static class Px
{
    public static void Rect(Canvas c, int x0, int y0, int x1, int y1, Col col)
    {
        for (int y = y0; y <= y1; y++) for (int x = x0; x <= x1; x++) c.Set(x, y, col);
    }

    /// <summary>A rectangle in a few mottled shades, like a Minecraft texture. Same pixels every frame for the same seed.</summary>
    public static void Mottled(Canvas c, int x0, int y0, int x1, int y1, Col[] shades, int seed)
    {
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                uint h = (uint)((x - x0) * 73856093 ^ (y - y0) * 19349663 ^ seed * 83492791);
                h ^= h >> 13; h *= 0x5bd1e995; h ^= h >> 15;
                c.Set(x, y, shades[h % (uint)shades.Length]);
            }
    }

    /// <summary>A 1-pixel dark outline around everything drawn so far, matching the rest of the mod's sprites.</summary>
    public static void Outline(Canvas c)
    {
        var edge = new List<(int, int)>();
        for (int y = 0; y < c.H; y++)
            for (int x = 0; x < c.W; x++)
                if (c.Get(x, y) == null && (c.Get(x - 1, y) != null || c.Get(x + 1, y) != null || c.Get(x, y - 1) != null || c.Get(x, y + 1) != null))
                    edge.Add((x, y));
        foreach (var (x, y) in edge) c.Set(x, y, Canvas.Outline);
    }

    public static void Tint(Canvas c, Col col, float t)
    {
        if (t <= 0) return;
        for (int i = 0; i < c.Px.Length; i++)
            if (c.Px[i] is { } px && px != Canvas.Outline) c.Px[i] = px.Lerp(col, t);
    }

    /// <summary>A soft round puff (explosions, smoke).</summary>
    public static void Puff(Canvas c, V at, float r, Col col)
    {
        for (int y = (int)(at.Y - r); y <= at.Y + r; y++)
            for (int x = (int)(at.X - r); x <= at.X + r; x++)
                if ((new V(x + 0.5f, y + 0.5f) - at).Len <= r) c.Set(x, y, col);
    }

    public static void Line(Canvas c, V a, V b, Col col, float w = 1)
    {
        int n = (int)MathF.Ceiling((b - a).Len * 2) + 1;
        for (int i = 0; i <= n; i++)
        {
            var p = a + (b - a) * (i / (float)n);
            for (float dy = 0; dy < w; dy++) for (float dx = 0; dx < w; dx++) c.Set((int)(p.X + dx), (int)(p.Y + dy), col);
        }
    }
}

// ---------------------------------------------------------------- Creeper (Night Falls)
// 40 x 64. A tall green block on four stubby feet with the famous frown, front view. It swells and flashes white
// while it hisses, then bursts into a puff of smoke.
public class CreeperArt : IFrameArt
{
    public string Name => "creeper";
    const int W = 40, H = 64;
    static readonly Col[] Greens = [Col.Hex("5aa03c"), Col.Hex("4a8e30"), Col.Hex("6cb44c"), Col.Hex("3e7a2a"), Col.Hex("8ad06a")];
    static readonly Col Face = Col.Hex("101410");

    // Smoke and Blast count from 1 (0 = none): a record struct's new() zeroes every field, ignoring the defaults.
    record struct P(int Bob = 0, int Swell = 0, float White = 0, float Red = 0, int Smoke = 0, int Blast = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(), new(), new(Bob: 1), new(Bob: 1) }.Select(Draw).ToList(),
        // the hiss: it swells and flashes
        ["cast"] = new P[] { new(Swell: 1, White: 0.2f), new(Swell: 1, White: 0.5f), new(White: 0.15f), new(Swell: 1, White: 0.55f), new() }.Select(Draw).ToList(),
        // the explosion: a white flash, then the blast (the creeper is gone)
        ["attack"] = new P[] { new(Swell: 2, White: 0.8f), new(Blast: 1), new(Blast: 2), new(Blast: 3) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Red: 0.4f), new(Red: 0.2f), new() }.Select(Draw).ToList(),
        // gone in a puff of smoke
        ["dead"] = new P[] { new(Smoke: 1), new(Smoke: 2), new(Smoke: 3), new(Smoke: 4) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        if (p.Blast > 0)
        {
            int blast = p.Blast - 1;
            float r = 10 + blast * 6;
            Px.Puff(c, new V(20, 40), r, blast == 2 ? Col.Hex("8a8a8a") : Col.Hex("f0a030"));
            Px.Puff(c, new V(20, 40), r * 0.6f, blast == 2 ? Col.Hex("b0b0b0") : Col.Hex("ffe070"));
            if (blast < 2) Px.Puff(c, new V(20, 40), r * 0.3f, Col.Hex("ffffff"));
            return c;
        }
        if (p.Smoke > 0)
        {
            int smoke = p.Smoke - 1;
            var grey = Col.Hex("c8c8c8").Lerp(Col.Hex("707070"), smoke / 3f);
            for (int i = 0; i < 5; i++)
            {
                var at = new V(10 + i * 5, 50 - smoke * 6 - (i % 2) * 4);
                Px.Puff(c, at, 4 - smoke * 0.7f, grey);
            }
            return c;
        }
        int s = p.Swell, b = p.Bob;
        // feet (front pair, the back pair peeking out a shade darker)
        Px.Mottled(c, 10 - s, 55, 18, 62, [Col.Hex("3e7a2a"), Col.Hex("2e6420")], 1);
        Px.Mottled(c, 22, 55, 30 + s, 62, [Col.Hex("3e7a2a"), Col.Hex("2e6420")], 2);
        // body
        Px.Mottled(c, 13 - s, 30 + b, 27 + s, 56, Greens, 3);
        // head
        Px.Mottled(c, 9 - s, 8 + b - s, 31 + s, 30 + b, Greens, 4);
        // the face: two square eyes and the frown
        int fy = 14 + b - s;
        Px.Rect(c, 12, fy, 16, fy + 4, Face);
        Px.Rect(c, 24, fy, 28, fy + 4, Face);
        Px.Rect(c, 17, fy + 5, 23, fy + 8, Face);
        Px.Rect(c, 15, fy + 8, 16, fy + 12, Face);
        Px.Rect(c, 24, fy + 8, 25, fy + 12, Face);
        Px.Rect(c, 17, fy + 9, 23, fy + 10, Face);
        Px.Outline(c);
        Px.Tint(c, Col.Hex("ffffff"), p.White);
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}

// ---------------------------------------------------------------- Skeleton (Night Falls)
// 48 x 80. Square skull, thin bones, a ribcage, and a bow in his front hand. Attack: draws and looses an arrow.
// Cast: takes aim (bow up, a glint). Dead: falls into a pile of bones.
public class SkeletonArt : IFrameArt
{
    public string Name => "skeleton";
    const int W = 48, H = 80;
    static readonly Col Bone = Col.Hex("dcdcd2"), BoneShade = Col.Hex("a8a8a0"), BoneDark = Col.Hex("6a6a64");
    static readonly Col Wood = Col.Hex("8a5a2c"), String = Col.Hex("e8e8e8");

    // Arrow counts from 1 (0 = no arrow in flight).
    record struct P(int Bob = 0, float Draw = 0, int Arrow = 0, int BowUp = 0, bool Glint = false, float Red = 0, int Fall = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(), new(), new(Bob: 1), new(Bob: 1) }.Select(Draw).ToList(),
        ["attack"] = new P[] { new(Draw: 0.5f), new(Draw: 1), new(Arrow: 1), new(Arrow: 2), new() }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(BowUp: 2), new(BowUp: 4, Draw: 0.6f), new(BowUp: 4, Draw: 1, Glint: true), new(BowUp: 4, Draw: 1), new() }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Red: 0.4f), new(Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Red: 0.3f), new(Fall: 1), new(Fall: 2), new(Fall: 3) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        if (p.Fall >= 2)
        {
            // a pile of bones with the skull on top
            int pile = p.Fall == 2 ? 6 : 8;
            for (int i = 0; i < 7; i++) Px.Line(c, new V(10 + i * 4, 78 - (i % 3)), new V(14 + i * 4 - (i % 2) * 6, 74 - (i % 3)), i % 2 == 0 ? Bone : BoneShade, 2);
            Px.Rect(c, 18, 78 - pile - 8, 28, 78 - pile + 1, Bone);
            Px.Rect(c, 20, 78 - pile - 5, 21, 78 - pile - 4, BoneDark); Px.Rect(c, 25, 78 - pile - 5, 26, 78 - pile - 4, BoneDark);
            Px.Line(c, new V(30, 76), new V(40, 70), Wood, 2);
            Px.Outline(c);
            return c;
        }
        int b = p.Bob + (p.Fall == 1 ? 3 : 0);
        // legs
        Px.Rect(c, 19, 46 + b, 21, 78, BoneShade);
        Px.Rect(c, 26, 46 + b, 28, 78, Bone);
        // pelvis
        Px.Rect(c, 18, 44 + b, 29, 47 + b, BoneShade);
        // spine and ribs
        Px.Rect(c, 23, 22 + b, 24, 44 + b, BoneShade);
        for (int y = 24; y <= 38; y += 4) Px.Rect(c, 18, y + b, 29, y + 1 + b, Bone);
        // back arm hanging
        Px.Rect(c, 15, 23 + b, 17, 42 + b, BoneShade);
        // skull
        Px.Rect(c, 16, 6 + b, 31, 21 + b, Bone);
        Px.Rect(c, 16, 18 + b, 31, 21 + b, BoneShade);
        Px.Rect(c, 23, 11 + b, 26, 14 + b, BoneDark);   // eye sockets (side view: the near one big, the far one thin)
        Px.Rect(c, 29, 11 + b, 30, 14 + b, BoneDark);
        Px.Rect(c, 27, 16 + b, 28, 17 + b, BoneDark);   // nose
        for (int x = 22; x <= 31; x += 2) c.Set(x, 20 + b, BoneDark);   // teeth
        // front arm out to the bow
        int hy = 28 + b - p.BowUp;
        Px.Line(c, new V(28, 24 + b), new V(36, hy), Bone, 2);
        // the bow: an arc in front of him, the string pulled back by Draw
        for (int i = -12; i <= 12; i++)
        {
            float k = i / 12f;
            c.Set(38 + (int)(3 * (1 - k * k)), hy + i, Wood);
            c.Set(39 + (int)(3 * (1 - k * k)), hy + i, Wood.Shade);
        }
        int pull = (int)(p.Draw * 7);
        Px.Line(c, new V(38, hy - 12), new V(37 - pull, hy), String);
        Px.Line(c, new V(38, hy + 12), new V(37 - pull, hy), String);
        if (p.Draw > 0) Px.Line(c, new V(37 - pull, hy), new V(46, hy), Col.Hex("c8c8c0"));   // arrow on the string
        if (p.Arrow > 0) Px.Line(c, new V(37 + p.Arrow * 3, hy), new V(47, hy), Col.Hex("ffffff"));
        Px.Outline(c);
        if (p.Glint) { c.Set(44, hy - 3, Col.Hex("ffffff")); c.Set(43, hy - 3, Col.Hex("ffe070")); c.Set(45, hy - 3, Col.Hex("ffe070")); c.Set(44, hy - 4, Col.Hex("ffe070")); c.Set(44, hy - 2, Col.Hex("ffe070")); }
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}

// ---------------------------------------------------------------- Spider (Night Falls)
// 72 x 40. A low, wide body (dark grey-brown abdomen, black head) with red eyes, and eight legs arching up over it and
// down to the ground, all drawn behind the body so it reads clearly. Attack: lunges and bites. Cast: rears up the wall
// (it gains Block). Dead: flips over with its legs curled.
public class SpiderArt : IFrameArt
{
    public string Name => "spider";
    const int W = 72, H = 40;
    static readonly Col[] Abdomen = [Col.Hex("4a423c"), Col.Hex("3e3732"), Col.Hex("564c44")];
    static readonly Col[] HeadCols = [Col.Hex("2a2624"), Col.Hex("332e2b")];
    static readonly Col Leg = Col.Hex("4a423c"), LegFar = Col.Hex("2e2824");
    static readonly Col Eye = Col.Hex("d02028"), EyeHi = Col.Hex("ff7060");

    record struct P(int Dx = 0, int Rise = 0, int Step = 0, float Red = 0, bool Flipped = false, bool Fangs = false);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(), new(Step: 1), new(), new(Step: 2) }.Select(Draw).ToList(),
        ["attack"] = new P[] { new(Dx: -3), new(Dx: 6, Fangs: true), new(Dx: 8, Fangs: true), new(Dx: 3), new() }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Rise: 3), new(Rise: 6, Step: 1), new(Rise: 6, Step: 2), new(Rise: 3), new() }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Dx: -2, Red: 0.4f), new(Dx: -1, Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Red: 0.3f), new(Flipped: true), new(Flipped: true, Step: 1) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        int cx = 36 + p.Dx;
        int top = p.Flipped ? 24 : 20 - p.Rise;
        // legs, far side first: four per side, hips along the body, knees just above it, feet spread wide on the ground
        for (int side = 0; side < 2; side++)
        {
            var col = side == 0 ? LegFar : Leg;
            for (int i = 0; i < 4; i++)
            {
                float hipX = cx - 10 + i * 6 + side * 2;
                var hip = new V(hipX, top + 5);
                bool lifted = (p.Step == 1 && i % 2 == 0) || (p.Step == 2 && i % 2 == 1);
                float spread = (i - 1.5f) * 11 + (side == 0 ? -3 : 3);
                if (p.Flipped)
                {
                    var up = hip + new V(spread * 0.35f, -9 - (lifted ? 2 : 0));
                    Px.Line(c, hip, up, col, 2);
                    Px.Line(c, up, up + new V(-spread * 0.15f, 4), col, 2);
                    continue;
                }
                var knee = new V(hipX + spread * 0.55f, top - 6 - (lifted ? 2 : 0));
                var foot = new V(hipX + spread * 1.2f, H - 2 - (lifted ? 2 : 0));
                Px.Line(c, hip, knee, col, 2);
                Px.Line(c, knee, foot, col, 2);
            }
        }
        // body on top of the legs: the round-ish abdomen behind, the head in front
        Px.Mottled(c, cx - 22, top, cx - 1, top + 11, Abdomen, 7);
        Px.Mottled(c, cx - 20, top - 1, cx - 3, top - 1, Abdomen, 9);
        Px.Mottled(c, cx, top + 1, cx + 11, top + 10, HeadCols, 8);
        if (!p.Flipped)
        {
            // eyes: two big red ones, two small
            Px.Rect(c, cx + 7, top + 3, cx + 9, top + 5, Eye); c.Set(cx + 9, top + 3, EyeHi);
            Px.Rect(c, cx + 2, top + 3, cx + 4, top + 5, Eye); c.Set(cx + 4, top + 3, EyeHi);
            c.Set(cx + 10, top + 7, Eye); c.Set(cx + 1, top + 7, Eye);
            if (p.Fangs) { Px.Rect(c, cx + 9, top + 10, cx + 10, top + 13, Col.Hex("e8e0d0")); Px.Rect(c, cx + 5, top + 10, cx + 6, top + 13, Col.Hex("e8e0d0")); }
        }
        Px.Outline(c);
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}

// ---------------------------------------------------------------- Enderman (The Enderman)
// 40 x 96. Very tall and thin: a black block head with glowing purple eyes, a narrow body, long dangling arms and legs,
// and purple specks drifting around him. Attack: a long-armed swipe (mouth open when enraged). Cast: teleports, fading
// into specks and back. Dead: comes apart into specks.
public class EndermanArt : IFrameArt
{
    public string Name => "enderman";
    const int W = 40, H = 96;
    static readonly Col[] Black = [Col.Hex("141018"), Col.Hex("1c1622"), Col.Hex("100c14")];
    static readonly Col Eye = Col.Hex("e070ff"), EyeCore = Col.Hex("f8d0ff"), Speck = Col.Hex("c050f0");

    record struct P(int Seed = 0, int Swing = 0, float Fade = 0, float Red = 0, bool Jaw = false, bool Gone = false);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 6).Select(i => Draw(new P(Seed: i))).ToList(),
        ["attack"] = new P[] { new(Seed: 1, Swing: 1), new(Seed: 2, Swing: 2, Jaw: true), new(Seed: 3, Swing: 3, Jaw: true), new(Seed: 4, Swing: 1) }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Seed: 1, Fade: 0.3f), new(Seed: 2, Fade: 0.65f), new(Seed: 3, Fade: 0.95f), new(Seed: 4, Fade: 0.6f), new(Seed: 5, Fade: 0.25f) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Red: 0.4f), new(Seed: 1, Red: 0.2f), new(Seed: 2) }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Seed: 1, Fade: 0.4f, Red: 0.2f), new(Seed: 2, Fade: 0.75f), new(Seed: 3, Fade: 1, Gone: true), new(Seed: 4, Fade: 1, Gone: true) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        if (!p.Gone)
        {
            // legs (the far one a shade darker), body, arms
            Px.Mottled(c, 16, 46, 17, 94, [Col.Hex("0e0a10"), Col.Hex("16121a")], 21);
            Px.Mottled(c, 21, 46, 23, 94, Black, 22);
            Px.Mottled(c, 16, 18, 23, 46, Black, 23);
            Px.Mottled(c, 13, 19, 14, 62, [Col.Hex("0e0a10"), Col.Hex("16121a")], 24);   // back arm
            if (p.Swing == 0) Px.Mottled(c, 25, 19, 26, 62, Black, 25);
            else
            {
                var hand = p.Swing switch { 1 => new V(31, 14), 2 => new V(38, 30), _ => new V(36, 46) };
                Px.Line(c, new V(25, 20), hand, Black[1], 2);
            }
            // head
            Px.Mottled(c, 12, 4, 27, 17, Black, 26);
            // eyes: a purple slit each, the near one wider
            Px.Rect(c, 19, 10, 23, 11, Eye); Px.Rect(c, 20, 10, 21, 10, EyeCore);
            Px.Rect(c, 25, 10, 27, 11, Eye); c.Set(26, 10, EyeCore);
            if (p.Jaw) Px.Rect(c, 19, 14, 27, 19, Col.Hex("060408"));   // the enraged gape
            Px.Outline(c);
            if (p.Fade > 0)
            {
                // dissolve: knock out pixels in a scattered order, more as Fade grows
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                    {
                        uint h = (uint)(x * 374761393 + y * 668265263); h = (h ^ (h >> 13)) * 1274126177; h ^= h >> 16;
                        if ((h % 1000) / 1000f < p.Fade) c.Px[y * W + x] = null;
                    }
            }
            Px.Tint(c, Col.Hex("ff4040"), p.Red);
        }
        // drifting specks, more while teleporting or dying
        int specks = 6 + (int)(p.Fade * 22);
        for (int i = 0; i < specks; i++)
        {
            uint h = (uint)(i * 73856093 ^ p.Seed * 19349663 ^ 0x2545F491);
            h ^= h >> 13; h *= 0x5bd1e995; h ^= h >> 15; h *= 0x27d4eb2d; h ^= h >> 16;
            int x = 4 + (int)(h % 32), y = 6 + (int)((h / 32) % 86);
            y = (y - p.Seed * 3 + H) % H;
            c.Set(x, y, i % 3 == 0 ? EyeCore : Speck);
        }
        return c;
    }
}

// ---------------------------------------------------------------- Deckhands (Blockbeard's Deckhands)
// Two of Captain Blockbeard's crew, drawn on the Rig like the brothers. Both wear striped shirts and bandanas.

/// <summary>Red bandana, red-and-white stripes, a brass spyglass. Attack: a jab with the spyglass. Cast: cups his mouth to shout an insult.</summary>
public class LookoutArt : FoeArt
{
    public override string Name => "lookout";
    static readonly Col Stripe = Col.Hex("c83a32"), Cream = Col.Hex("f0e6d0"), Bandana = Col.Hex("b02a2a"), Brass = Col.Hex("d0a040");

    public LookoutArt()
    {
        Skin = Col.Hex("d8a87e");
        HairC = Col.Hex("4a2e1c"); BeardC = Col.Hex("4a2e1c"); Hair = HairStyle.Shaggy; BeardLen = 0; Stubble = true; StubbleAmt = 0.2f;
        Sleeve = Cream; Pants = Col.Hex("5a4a3a"); Shoes = Col.Hex("2a2018");
    }

    protected override Pose Base() => new() { Prop = "spyglass", FrontHand = new V(8, 10), FrontElbowPref = new V(0, 1), BackHand = new V(-3, 14) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 3, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1, Cream, hi: Cream.Light);
        c.Recolor(q => S.Poly(q, body) - 1, px => px);
        for (float y = r.S.Y + 2; y < r.H.Y + 3; y += 4)
        {
            float yy = y;
            c.Recolor(q => S.I(S.Poly(q, body) - 1, MathF.Abs(q.Y - yy) - 0.9f), px => px == Canvas.Outline ? px : Stripe);
        }
        c.Draw(q => S.Box(q, r.H.X - 8, r.H.Y - 1.5f, r.H.X + 9, r.H.Y + 1), Col.Hex("2a2018"), outline: false);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        c.Draw(q => S.I(S.Circle(q, h + new V(-0.5f, -1.5f), 10.6f), q.Y - (h.Y - 3.5f)), Bandana, hi: Bandana.Light);
        c.Draw(q => S.Taper(q, h + new V(-9, -4), h + new V(-14, 1), 2f, 1f), Bandana);   // the knot's tail
        if (p.Prop == "shout")
            for (int i = 0; i < 3; i++) c.Plot(h + new V(14 + i * 2, -2 + i * 2), Col.Hex("ffffff"));
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "spyglass") return;
        var h = r.HandF; var d = V.Dir(p.PropAngle);
        c.Draw(q => S.Taper(q, h - d * 2, h + d * 11, 1.3f, 1.8f), Brass, hi: Brass.Light);
        c.Draw(q => S.Capsule(q, h + d * 4, h + d * 4.6f, 1.6f), Brass.Shade);
        if (p.Fx == 1) Sparkle(c, h + d * 13, 2, Col.Hex("fff4d8"));
    }

    protected override List<Pose> Attack() => Strike([
        (new(6, 8), -20, 0, 0), (new(2, 6), -30, -2, 0), (new(15, 3), 0, 5, 1), (new(13, 5), 0, 4, 0), (new(8, 10), 0, 0, 0)]);

    // Cast: hand cupped at his mouth, shouting
    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        for (int i = 0; i < 5; i++)
        {
            var p = Base(); p.Prop = i is 1 or 2 or 3 ? "shout" : "spyglass";
            if (i is 1 or 2 or 3) { p.FrontHand = new V(11, -9); p.Face = Face.Fierce; p.Lean = 2; }
            list.Add(p);
        }
        return list;
    }
}

/// <summary>A wiry kid with a blue bandana and a powder keg. Cast: lights the fuse (sparks). Attack: hurls the keg (a burst).</summary>
public class PowderMonkeyArt : FoeArt
{
    public override string Name => "powder_monkey";
    static readonly Col Stripe = Col.Hex("2c5aa0"), Cream = Col.Hex("f0e6d0"), Bandana = Col.Hex("2a4a8a");
    static readonly Col Wood = Col.Hex("8a5a2c"), Hoop = Col.Hex("4a4a50");

    public PowderMonkeyArt()
    {
        Skin = Col.Hex("c8946a");
        HairC = Col.Hex("2a1c14"); BeardC = Col.Hex("2a1c14"); Hair = HairStyle.Messy; BeardLen = 0;   // just a kid: no beard
        Sleeve = Cream; Pants = Col.Hex("6a5a44"); Shoes = Col.Hex("2a2018");
    }

    protected override Pose Base() => new() { Prop = "keg", FrontHand = new V(7, 11), FrontElbowPref = new V(0, 1), BackHand = new V(3, 12), BackElbowPref = new V(0, 1) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 3, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1, Cream, hi: Cream.Light);
        for (float y = r.S.Y + 2; y < r.H.Y + 3; y += 4)
        {
            float yy = y;
            c.Recolor(q => S.I(S.Poly(q, body) - 1, MathF.Abs(q.Y - yy) - 0.9f), px => px == Canvas.Outline ? px : Stripe);
        }
        c.Draw(q => S.Box(q, r.H.X - 8, r.H.Y - 1.5f, r.H.X + 9, r.H.Y + 1), Col.Hex("2a2018"), outline: false);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        c.Draw(q => S.I(S.Circle(q, h + new V(-0.5f, -1.5f), 10.6f), q.Y - (h.Y - 3.5f)), Bandana, hi: Bandana.Light);
    }

    private void Keg(Canvas c, V at, int fx)
    {
        c.Draw(q => S.RBox(q, at, 5.5f, 6.5f, 0) - 1.5f, Wood, hi: Wood.Light);
        c.Draw(q => S.Box(q, at.X - 6, at.Y - 4, at.X + 6, at.Y - 3), Hoop, outline: false);
        c.Draw(q => S.Box(q, at.X - 6, at.Y + 3, at.X + 6, at.Y + 4), Hoop, outline: false);
        // the fuse, lit while he's getting ready to throw
        var top = at + new V(1, -7);
        c.Draw(q => S.Capsule(q, top, top + new V(2, -4), 0.5f), Col.Hex("e8d8b0"), noShade: true);
        if (fx >= 1) Sparkle(c, top + new V(2, -5), fx == 2 ? 2 : 1, Col.Hex("ffc040"));
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop is "keg" or "keg_lit") { Keg(c, (r.HandF + r.HandB) * 0.5f + new V(1, 2), p.Prop == "keg_lit" ? p.Fx : 0); DrawHand(c, r.HandF, false); }
        if (p.Prop == "thrown")
        {
            var at = r.HandF + new V(8 + p.Fx * 6, -4 + p.Fx);
            if (p.Fx < 2) Keg(c, at, 2);
            else { for (int i = 0; i < 3; i++) Sparkle(c, at + new V(i * 3 - 3, (i % 2) * 3 - 2), 3 - i % 2, i == 1 ? Col.Hex("ffffff") : Col.Hex("ffa030")); }
        }
    }

    protected override List<Pose> Attack()
    {
        var list = new List<Pose>();
        V[] hands = [new(4, 4), new(2, -2), new(14, 2), new(14, 4), new(7, 11)];
        for (int i = 0; i < hands.Length; i++)
        {
            var p = Base(); p.FrontHand = hands[i]; p.BackHand = new V(-4, 12);
            p.Prop = i is 2 or 3 ? "thrown" : (i == 4 ? "keg" : "keg_lit"); p.Fx = i is 2 or 3 ? i - 1 : 2;
            if (i == 4) p.Fx = 0;
            p.Lean = i == 2 ? 4 : (i == 1 ? -2 : 0); p.Face = i is 1 or 2 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }

    // Cast: strikes a match and lights the fuse
    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        for (int i = 0; i < 5; i++)
        {
            var p = Base(); p.Prop = "keg_lit"; p.Fx = i is 1 or 2 ? 1 : (i == 3 ? 2 : 0);
            p.Face = i == 3 ? Face.Happy : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Philistine Scout (Philistine Scouts)
// A scout from Goliath's army: a bronze helmet with a red crest (not the historical feathered crown, which at this
// size reads as a war bonnet), a short kilt, a round bronze shield on the back arm, and a spear. Attack: a spear thrust. Cast: raises the shield (guarding).
public class PhilistineScoutArt : FoeArt
{
    public override string Name => "philistine_scout";
    static readonly Col Kilt = Col.Hex("d8c8a0"), Trim = Col.Hex("2a5a9a"), Bronze = Col.Hex("c08a3a"), Shaft = Col.Hex("7a5530");
    static readonly Col Band = Col.Hex("b03a2a");   // the helmet crest

    public PhilistineScoutArt()
    {
        Skin = Col.Hex("c99a72");
        HairC = Col.Hex("1e1612"); BeardC = Col.Hex("1e1612"); Hair = HairStyle.Short; BeardLen = 0;
        Sleeve = Skin; Forearm = Skin; Pants = Skin.Mul(0.92f); Shoes = Col.Hex("5a3a22");
    }

    protected override Pose Base() => new() { Prop = "spear", PropAngle = -75, FrontHand = new V(10, 10), FrontElbowPref = new V(0, 1), BackHand = new V(-4, 10) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // bare chest with a crossed strap, and a kilt with a blue hem
        var body = TorsoPoly(r, r.H.Y, 1f);
        c.Draw(q => S.Poly(q, body) - 1, Skin, hi: Skin.Light);
        c.Draw(q => S.Capsule(q, r.S + new V(-4, 1), r.H + new V(5, -3), 0.8f), Col.Hex("5a3a22"), outline: false);
        c.Draw(q => S.Poly(q, r.H + new V(-8, -2), r.H + new V(9, -2), r.H + new V(11, 9), r.H + new V(-10, 9)), Kilt, hi: Kilt.Light);
        c.Draw(q => S.Box(q, r.H.X - 10, r.H.Y + 7, r.H.X + 11, r.H.Y + 9), Trim, outline: false);
        // the round shield on the back arm (DrawFront raises it in front when guarding)
        if (p.Prop != "guard") Shield(c, r.HandB + new V(-1, -1));
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // bronze cap over the top of the head, with a red horsehair crest running front to back
        c.Draw(q => S.I(S.Circle(q, h + new V(-0.5f, -1.5f), 10.8f), q.Y - (h.Y - 3f)), Bronze, hi: Bronze.Light);
        c.Draw(q => S.I(S.Ellipse(q, h + new V(-1, -11.5f), 9f, 4f), q.Y - (h.Y - 10f)), Band, hi: Band.Light);
        for (float x = h.X - 9; x <= h.X + 9; x += 3) c.Plot(x, h.Y - 3.5f, Bronze.Shade);
    }

    private static void Shield(Canvas c, V at)
    {
        c.Draw(q => S.Circle(q, at, 7.5f), Bronze, hi: Bronze.Light);
        c.Draw(q => MathF.Abs(S.Circle(q, at, 5f)) - 0.5f, Bronze.Shade, outline: false);
        c.Draw(q => S.Circle(q, at, 1.8f), Bronze.Light.Light);
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "guard") Shield(c, r.SF + new V(9, 8));
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop is not ("spear" or "guard")) return;
        var h = r.HandF; var d = V.Dir(p.PropAngle);
        c.Draw(q => S.Capsule(q, h - d * 12, h + d * 18, 0.9f), Shaft, hi: Shaft.Light);
        c.Draw(q => S.Poly(q, h + d * 17 + d.Perp * 2, h + d * 24, h + d * 17 - d.Perp * 2), Bronze, hi: Bronze.Light);
        if (p.Fx == 1) for (int i = 0; i < 3; i++) c.Plot(h - d * (4 + i * 3) + d.Perp * 3, Col.Hex("fff4d8"));
    }

    protected override List<Pose> Attack() => Strike([
        (new(4, 7), -10, -2, 0), (new(-2, 6), -5, -4, 0), (new(14, 5), 0, 6, 1),
        (new(13, 6), 0, 5, 0), (new(11, 8), -40, 2, 0), (new(10, 10), -75, 0, 0)]);

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] back = [new(-4, 10), new(2, 4), new(6, 0), new(6, 0), new(2, 5), new(-4, 10)];
        for (int i = 0; i < back.Length; i++)
        {
            var p = Base(); p.Prop = i is 2 or 3 ? "guard" : "spear"; p.BackHand = back[i]; p.Crouch = i is 2 or 3 ? 2 : 0;
            p.Face = i is 2 or 3 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Jackal (The Jackals)
// 56 x 36. A lean desert jackal: sandy coat with a dark saddle on the back, tall ears, long legs, a bushy tail with a
// black tip. Attack: lunges with jaws open. Cast: head up, howling. Dead: lies down.
public class JackalArt : IFrameArt
{
    public string Name => "jackal";
    const int W = 56, H = 36;
    static readonly Col Fur = Col.Hex("c89a5a"), Saddle = Col.Hex("5a4a3a"), Belly = Col.Hex("e8d0a0"), Nose = Col.Hex("2a2020");

    record struct P(float Dx = 0, float Bob = 0, float HeadUp = 0, float Mouth = 0, float Tail = 0, float Tint = 0, float Lie = 0, bool Dead = false, int Fx = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 6).Select(i => Draw(new P(Bob: i is 2 or 3 ? 1 : 0, Tail: MathF.Sin(i / 6f * MathF.Tau)))).ToList(),
        ["attack"] = new P[] { new(Dx: -2, Bob: 1), new(Dx: -3, Bob: 2, Mouth: 0.5f), new(Dx: 6, Mouth: 1, Fx: 1), new(Dx: 5, Mouth: 1), new(Dx: 2, Mouth: 0.3f) }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(HeadUp: 2), new(HeadUp: 5, Mouth: 0.6f), new(HeadUp: 7, Mouth: 1, Fx: 2), new(HeadUp: 7, Mouth: 1, Fx: 3), new(HeadUp: 3) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Dx: -2, Tint: 0.4f), new(Dx: -1, Tint: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Tint: 0.3f), new(Lie: 0.4f, Dead: true), new(Lie: 0.8f, Dead: true), new(Lie: 1, Dead: true) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        float y0 = 18 + p.Bob + p.Lie * 8;
        var body = new V(25 + p.Dx, y0);
        var shoulder = body + new V(9, 1); var hip = body + new V(-9, 1);
        var head = body + new V(15, -7 - p.HeadUp + p.Lie * 3);
        Col far = Fur.Mul(0.78f);

        // tail: bushy, hanging and swishing, with a black tip
        var t0 = hip + new V(-3, -1);
        var tip = t0 + new V(-8, 6 + p.Tail * 2 - p.Lie * 3);
        c.Draw(q => S.Taper(q, t0, tip, 1.6f, 2.4f), Fur);
        c.Draw(q => S.Circle(q, tip, 2.2f), Saddle);

        void Leg(V top, float dx, Col col)
        {
            var foot = new V(top.X + dx, H - 2);
            if (p.Lie > 0) foot = new V(top.X + dx + 7 * p.Lie, MathF.Min(H - 2, top.Y + 9 * (1 - p.Lie * 0.6f)));
            c.Draw(q => S.Capsule(q, top, foot, 1.5f), col);
            c.Draw(q => S.Ellipse(q, foot + new V(1, 0), 2f, 1.2f), col);
        }
        Leg(shoulder + new V(-2, 0), 2, far);
        Leg(hip + new V(2, 0), 1, far);

        // body: sandy with a dark saddle along the back and a pale belly
        c.Draw(q => S.Ellipse(q, body, 13, 5.5f), Fur, hi: Fur.Light);
        c.Draw(q => S.I(S.Ellipse(q, body + new V(-1, -2), 11, 3.5f), q.Y - (body.Y - 1)), Saddle, outline: false);
        c.Draw(q => S.Ellipse(q, body + new V(1, 3.5f), 9, 1.6f), Belly, outline: false, noShade: true);

        Leg(shoulder, 0, Fur);
        Leg(hip, -1, Fur);

        // neck, head, tall ears, long snout
        c.Draw(q => S.Capsule(q, shoulder + new V(0, -2), head + new V(-1, 2), 3f), Fur);
        c.Draw(q => S.Poly(q, head + new V(-3, -2), head + new V(-2, -10), head + new V(1, -3)), Fur, hi: Fur.Light);
        c.Draw(q => S.Poly(q, head + new V(0, -2), head + new V(2, -9), head + new V(3, -2)), Fur.Mul(0.85f));
        c.Draw(q => S.Circle(q, head, 4f), Fur, hi: Fur.Light);
        var snout = head + new V(5, 1.5f);
        c.Draw(q => S.Ellipse(q, snout, 4f, 1.8f), Belly);
        c.Plot(snout + new V(4, -1), Nose);
        if (p.Mouth > 0)
        {
            float open = 1 + p.Mouth * 2;
            c.Draw(q => S.Poly(q, snout + new V(-2, 1), snout + new V(4, 1), snout + new V(3, 1 + open)), Col.Hex("5a1414"), outline: false);
            c.Plot(snout + new V(3, 1), Col.Hex("ffffff"));
        }
        var eye = head + new V(1.5f, -1);
        if (p.Dead) { c.Plot(eye + new V(-1, -1), Brother.Eye); c.Plot(eye, Brother.Eye); c.Plot(eye + new V(1, 1), Brother.Eye); }
        else { c.Plot(eye, Brother.Eye); c.Plot(eye + new V(1, 0), Col.Hex("e8c040")); }

        // howl lines going up, and a lunge streak
        if (p.Fx is 2 or 3)
            for (int i = 0; i < 3; i++)
            {
                float r = 4 + i * 3 + (p.Fx - 2) * 2;
                for (float a = -120; a <= -60; a += 12) c.Plot(snout + V.Dir(a) * r, Col.Hex("fff4d8"));
            }
        if (p.Fx == 1) for (int i = 0; i < 4; i++) c.Plot(body + new V(-15 - i * 2, -3 + i * 2), Col.Hex("fff4d8"));

        if (p.Tint > 0) LionArt.Tint(c, p.Tint);
        return c;
    }
}

// ---------------------------------------------------------------- Roomba (Runaway Roomba)
// 52 x 24. A robot vacuum seen from the side: a low dark disc with a lighter top, a bumper, a blinking status light,
// and a spinning side brush. Attack: rams forward. Cast: dust swirls get sucked in. Dead: sparks and smoke, light goes red.
public class RoombaArt : IFrameArt
{
    public string Name => "roomba";
    const int W = 52, H = 24;
    static readonly Col Shell = Col.Hex("3a3c42"), Top = Col.Hex("5a5e66"), Bumper = Col.Hex("26282c"), Light = Col.Hex("40c0ff");

    record struct P(int Dx = 0, int Brush = 0, bool LightOn = true, int Dust = 0, float Red = 0, int Broken = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 4).Select(i => Draw(new P(Brush: i, LightOn: i != 3))).ToList(),
        ["attack"] = new P[] { new(Dx: -2, Brush: 1), new(Dx: 6, Brush: 2), new(Dx: 8, Brush: 3), new(Dx: 3, Brush: 0) }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Brush: 1, Dust: 1), new(Brush: 2, Dust: 2), new(Brush: 3, Dust: 3), new(Brush: 0, Dust: 4) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Dx: -2, Red: 0.4f), new(Dx: -1, Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Red: 0.3f, Broken: 1), new(Broken: 2, LightOn: false), new(Broken: 3, LightOn: false), new(Broken: 4, LightOn: false) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        int x0 = 8 + p.Dx, x1 = 42 + p.Dx;
        // body: a low slab, rounded at the ends
        Px.Rect(c, x0 + 1, 12, x1 - 1, 21, Shell);
        Px.Rect(c, x0, 13, x1, 20, Shell);
        Px.Rect(c, x0 + 2, 11, x1 - 2, 13, Top);
        Px.Rect(c, x0 + 6, 10, x1 - 6, 11, Top.Light);
        Px.Rect(c, x1 - 3, 14, x1, 20, Bumper);           // front bumper
        Px.Rect(c, x0 + 3, 21, x1 - 3, 22, Bumper);       // wheels' shadow
        // status light and a little button on top
        var light = p.Broken > 0 ? Col.Hex("ff3a2a") : (p.LightOn ? Light : Light.Mul(0.5f));
        Px.Rect(c, x1 - 9, 15, x1 - 7, 16, light);
        Px.Rect(c, (x0 + x1) / 2 - 2, 9, (x0 + x1) / 2 + 2, 10, Col.Hex("2a2c30"));
        // the side brush: three spokes turning under the front
        int bx = x1 - 2, by = 22;
        for (int s = 0; s < 3; s++)
        {
            float a = (p.Brush * 40 + s * 120) * MathF.PI / 180;
            c.Set(bx + (int)MathF.Round(MathF.Cos(a) * 3), by + (int)MathF.Round(MathF.Sin(a) * 1), Col.Hex("8a8e96"));
        }
        Px.Outline(c);
        // dust swirling in
        if (p.Dust > 0)
            for (int i = 0; i < 6; i++)
            {
                float k = (i + p.Dust) % 6 / 6f;
                c.Set(x1 + 9 - (int)(k * 9), 20 - (int)(MathF.Sin(k * 6 + i) * 3), i % 2 == 0 ? Col.Hex("b8b0a0") : Col.Hex("8a8478"));
            }
        if (p.Broken > 0)
        {
            for (int i = 0; i < p.Broken + 1; i++) Px.Puff(c, new V((x0 + x1) / 2 - 4 + i * 3, 8 - i * 2 - p.Broken), 1.5f + (i % 2), Col.Hex("8a8a8a"));
            if (p.Broken < 3) { c.Set(x1 - 4, 9, Col.Hex("ffe070")); c.Set(x1 - 2, 7, Col.Hex("ffffff")); }
        }
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}

// ---------------------------------------------------------------- Laptop (Windows Update)
// 48 x 44. An open laptop, front view: a dark bezel around a blue screen with a loading spinner and a progress bar,
// sitting on its keyboard base. Attack: a pop-up window flashes up and the lid jolts. Cast: the screen restarts (goes
// black, then blue). Dead: the screen cracks and goes dark, with sparks.
public class LaptopArt : IFrameArt
{
    public string Name => "laptop";
    const int W = 48, H = 44;
    static readonly Col Bezel = Col.Hex("26282e"), ScreenBlue = Col.Hex("1e6ac8"), Base = Col.Hex("8a8e98"), Keys = Col.Hex("5a5e66");

    record struct P(int Spin = 0, int Jolt = 0, bool Popup = false, int Screen = 0, float Red = 0, bool Cracked = false);
    // Screen: 0 = updating (blue), 1 = black (restarting), 2 = off (dead)

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 8).Select(i => Draw(new P(Spin: i))).ToList(),
        ["attack"] = new P[] { new(Spin: 1, Popup: true), new(Spin: 2, Jolt: 2, Popup: true), new(Spin: 3, Jolt: 1, Popup: true), new(Spin: 4) }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Spin: 1), new(Screen: 1), new(Screen: 1), new(Spin: 0), new(Spin: 2) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Spin: 1, Red: 0.4f, Jolt: 1), new(Spin: 2, Red: 0.2f), new(Spin: 3) }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Red: 0.3f, Cracked: true), new(Screen: 2, Cracked: true, Jolt: 1), new(Screen: 2, Cracked: true) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        int j = p.Jolt;
        // keyboard base (a shallow trapezoid seen from the front)
        Px.Rect(c, 3, 36, 44, 41, Base);
        Px.Rect(c, 1, 41, 46, 42, Base.Shade);
        for (int x = 6; x < 42; x += 3) { c.Set(x, 38, Keys); c.Set(x + 1, 38, Keys); }
        Px.Rect(c, 18, 40, 29, 40, Keys);
        // the lid: bezel and screen
        Px.Rect(c, 5, 4 - j, 42, 35, Bezel);
        var screen = p.Screen switch { 1 => Col.Hex("080a0e"), 2 => Col.Hex("101216"), _ => ScreenBlue };
        Px.Rect(c, 8, 7 - j, 39, 32, screen);
        if (p.Screen == 0)
        {
            // loading spinner: a ring of dots, one bright
            var mid = new V(23.5f, 15 - j);
            for (int i = 0; i < 8; i++)
            {
                float a = i * MathF.PI / 4;
                var at = mid + new V(MathF.Cos(a) * 4, MathF.Sin(a) * 4);
                c.Set((int)at.X, (int)at.Y, i == p.Spin % 8 ? Col.Hex("ffffff") : Col.Hex("8ab8f0"));
            }
            // "Updating..." progress bar
            Px.Rect(c, 12, 24 - j, 35, 26 - j, Col.Hex("12408a"));
            Px.Rect(c, 12, 24 - j, 12 + 6 + (p.Spin % 8) * 2, 26 - j, Col.Hex("e8f0ff"));
            for (int x = 15; x < 32; x += 2) c.Set(x, 29 - j, Col.Hex("bcd4f4"));   // the tiny text under the bar
        }
        if (p.Popup)
        {
            // an annoying window pops up over the screen
            Px.Rect(c, 14, 10 - j, 36, 24 - j, Col.Hex("f0f0f0"));
            Px.Rect(c, 14, 10 - j, 36, 12 - j, Col.Hex("2a5ab0"));
            Px.Rect(c, 33, 10 - j, 35, 12 - j, Col.Hex("e04040"));
            for (int x = 17; x < 33; x += 2) { c.Set(x, 15 - j, Col.Hex("5a5e66")); c.Set(x, 18 - j, Col.Hex("5a5e66")); }
            Px.Rect(c, 26, 20 - j, 33, 22 - j, Col.Hex("2a8a3a"));
        }
        if (p.Cracked)
        {
            Px.Line(c, new V(28, 9), new V(22, 18), Col.Hex("e8e8f0"));
            Px.Line(c, new V(22, 18), new V(26, 27), Col.Hex("e8e8f0"));
            Px.Line(c, new V(22, 18), new V(13, 21), Col.Hex("c8c8d0"));
        }
        Px.Outline(c);
        if (p.Cracked && p.Screen == 2) { c.Set(40, 2, Col.Hex("ffe070")); c.Set(42, 1, Col.Hex("ffffff")); c.Set(6, 3, Col.Hex("ffe070")); }
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}

// ---------------------------------------------------------------- Chores (Laundry Pile, Dirty Dishes, Overgrown Lawn)
// Household chores come to life, each with a pair of googly eyes. Shared poses: idle wobble, attack hop, cast puff
// (getting bigger), hit flash, dead flattens out.
public abstract class ChoreArt : IFrameArt
{
    public abstract string Name { get; }
    protected abstract int W { get; }
    protected abstract int H { get; }

    protected record struct P(int Hop = 0, int Lean = 0, int Grow = 0, float Red = 0, float Flat = 0, int Look = 0, int Fx = 0);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(), new(Look: 1), new(Lean: 1), new(Look: 1, Lean: 1) }.Select(Make).ToList(),
        ["attack"] = new P[] { new(Hop: 2, Lean: -1), new(Hop: 5, Lean: 2, Fx: 1), new(Hop: 2, Lean: 3, Fx: 1), new() }.Select(Make).ToList(),
        ["cast"] = new P[] { new(Grow: 1), new(Grow: 2, Fx: 2), new(Grow: 2, Fx: 2), new(Grow: 1) }.Select(Make).ToList(),
        ["hit"] = new P[] { new(Red: 0.4f, Lean: -2), new(Red: 0.2f, Lean: -1), new() }.Select(Make).ToList(),
        ["dead"] = new P[] { new(Red: 0.3f), new(Flat: 0.4f), new(Flat: 0.8f), new(Flat: 1) }.Select(Make).ToList(),
    };

    Canvas Make(P p)
    {
        var c = new Canvas(W, H);
        Body(c, p);
        Px.Outline(c);
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }

    protected abstract void Body(Canvas c, P p);

    /// <summary>Two googly eyes: white with a black pupil that wanders. Shut (a line) once it's flattened.</summary>
    protected static void Eyes(Canvas c, int x, int y, P p)
    {
        foreach (int ex in new[] { x, x + 6 })
        {
            if (p.Flat >= 0.8f) { Px.Rect(c, ex - 1, y + 1, ex + 2, y + 1, Col.Hex("101010")); continue; }
            Px.Rect(c, ex - 1, y - 1, ex + 2, y + 2, Col.Hex("ffffff"));
            int px = ex + (p.Look == 1 ? 1 : 0), py = y + (p.Fx == 1 ? 0 : 1);
            Px.Rect(c, px, py, px + 1, py, Col.Hex("101010"));
        }
    }
}

public class LaundryArt : ChoreArt
{
    public override string Name => "laundry_pile";
    protected override int W => 44; protected override int H => 40;

    protected override void Body(Canvas c, P p)
    {
        int bottom = 38 - p.Hop, l = p.Lean;
        float squash = 1 - p.Flat * 0.7f, grow = 1 + p.Grow * 0.12f;
        // clothes heaped up out of the top of the basket (drawn first, the basket covers their bottoms)
        int rim = bottom - 15;
        (Col col, float x, float r)[] lumps =
        [
            (Col.Hex("3a6ab0"), 13, 7), (Col.Hex("5a9a4a"), 30, 7), (Col.Hex("c84a3a"), 21, 8), (Col.Hex("e8d070"), 25, 5),
        ];
        foreach (var (col, x, r) in lumps)
        {
            var cl = col;
            c.Draw(q => S.Circle(q, new V(x + l, rim - (r - 2) * squash * grow), r), cl, hi: cl.Light, outline: false);
        }
        // a wicker basket: wider at the top, tan with a woven pattern and a darker rim and handle holes
        for (int y = rim; y <= bottom; y++)
        {
            int inset = (y - rim) * 3 / 15;
            for (int x = 6 + inset; x <= 37 - inset; x++)
            {
                bool weave = ((x / 3) + (y / 2)) % 2 == 0;
                c.Set(x, y, weave ? Col.Hex("c8a060") : Col.Hex("a8803e"));
            }
        }
        Px.Rect(c, 5, rim - 1, 38, rim + 1, Col.Hex("8a6430"));
        Px.Rect(c, 10, rim + 3, 13, rim + 4, Col.Hex("5a3e1a"));
        Px.Rect(c, 30, rim + 3, 33, rim + 4, Col.Hex("5a3e1a"));
        // a striped dirty sock hanging over the rim, down the front
        int sx = 31;
        Px.Rect(c, sx - 1, rim - 2, sx + 3, rim + 8, Col.Hex("f0f0f0"));
        for (int y = rim; y <= rim + 6; y += 3) Px.Rect(c, sx - 1, y, sx + 3, y, Col.Hex("d04040"));
        Px.Rect(c, sx - 3, rim + 8, sx + 3, rim + 10, Col.Hex("f0f0f0"));   // the foot, toes pointing out
        c.Set(sx - 2, rim + 9, Col.Hex("8a7a50")); c.Set(sx + 1, rim + 7, Col.Hex("8a7a50"));   // dirt smudges
        // a sleeve flopped over the other side
        Px.Line(c, new V(9, rim - 2), new V(4, rim + 6), Col.Hex("3a6ab0"), 3);
        Eyes(c, 17 + l, (int)(rim - 6 * squash * grow), p);
        if (p.Fx == 1) { Px.Rect(c, 40, 10, 43, 12, Col.Hex("5a9a4a")); }   // a shirt flung on the attack
        if (p.Fx == 2) { c.Set(4, 6, Col.Hex("ffffff")); c.Set(40, 4, Col.Hex("ffffff")); }
    }
}

public class DishesArt : ChoreArt
{
    public override string Name => "dirty_dishes";
    protected override int W => 40; protected override int H => 48;

    protected override void Body(Canvas c, P p)
    {
        int bottom = 46 - p.Hop;
        int plates = p.Flat > 0 ? Math.Max(1, (int)(9 * (1 - p.Flat))) : 9 + p.Grow;
        // a leaning stack of plates with food smudges
        for (int i = 0; i < plates; i++)
        {
            int y = bottom - i * 3;
            int dx = (int)(i * 0.5f * (p.Lean + 1) - i * 0.3f);
            Px.Rect(c, 8 + dx, y - 1, 31 + dx, y, Col.Hex("e8eef0"));
            Px.Rect(c, 9 + dx, y - 2, 30 + dx, y - 2, Col.Hex("c8d4d8"));
            if (i % 3 == 1) Px.Rect(c, 12 + dx + i % 5, y - 1, 15 + dx + i % 5, y - 1, Col.Hex("a86a3a"));   // smudge
        }
        int topY = bottom - plates * 3;
        // a mug and a fork on top
        int mx = 14 + (int)(plates * 0.5f * (p.Lean + 1) - plates * 0.3f);
        Px.Rect(c, mx, topY - 8, mx + 7, topY - 1, Col.Hex("c84a3a"));
        Px.Rect(c, mx + 8, topY - 6, mx + 9, topY - 3, Col.Hex("c84a3a"));
        Px.Line(c, new V(mx + 11, topY - 1), new V(mx + 15, topY - 10), Col.Hex("b8bcc4"));
        Eyes(c, 15 + mx - 14, topY - 6 + (int)(p.Flat * 4), p);
        // a flying plate on the attack
        if (p.Fx == 1) Px.Rect(c, 33, 18, 39, 19, Col.Hex("e8eef0"));
    }
}

public class LawnArt : ChoreArt
{
    public override string Name => "overgrown_lawn";
    protected override int W => 56; protected override int H => 40;

    protected override void Body(Canvas c, P p)
    {
        int bottom = 38;
        float height = (22 + p.Grow * 4) * (1 - p.Flat * 0.8f);
        // a dirt patch and a dense clump of tall grass blades, two shades of green, swaying with Lean
        Px.Rect(c, 4, bottom - 2, 51, bottom, Col.Hex("6a4a2a"));
        for (int i = 0; i < 22; i++)
        {
            int x = 6 + i * 2;
            float h = height * (0.6f + ((i * 37) % 10) / 25f);
            var tip = new V(x + (p.Lean + (i % 3 - 1)) * 2 + (p.Fx == 1 ? 6 : 0), bottom - 2 - h - p.Hop);
            Px.Line(c, new V(x, bottom - 2), tip, i % 2 == 0 ? Col.Hex("4a9a3a") : Col.Hex("3a7a2c"), 2);
        }
        // a couple of dandelions poking out
        foreach (int x in new[] { 14, 40 })
        {
            int y = (int)(bottom - 4 - height * 1.1f) - p.Hop;
            Px.Line(c, new V(x, bottom - 2), new V(x, y), Col.Hex("5aaa3a"));
            Px.Rect(c, x - 1, y - 2, x + 2, y + 1, Col.Hex("f0d030"));
        }
        Eyes(c, 24 + p.Lean, (int)(bottom - 4 - height * 0.6f) - p.Hop, p);
    }
}

// ---------------------------------------------------------------- Poxwalker (Poxwalker Horde)
// A shambling plague zombie, drawn on the Rig: grey-green skin with sores, bald, hunched forward with both arms reaching,
// in torn brown rags. Attack: a clawing lunge. Cast: throws its head back and groans.
public class PoxwalkerArt : FoeArt
{
    public override string Name => "poxwalker";
    static readonly Col Rags = Col.Hex("5e4e3a"), Sore = Col.Hex("c8b850"), SoreRed = Col.Hex("8a3a2a");

    public PoxwalkerArt()
    {
        Skin = Col.Hex("9aa07a");
        HairC = Col.Hex("6a7050"); BeardC = Col.Hex("6a7050"); Hair = HairStyle.Bald; BeardLen = 0;
        Sleeve = Skin; Forearm = Skin; Pants = Rags.Mul(0.8f); Shoes = Skin.Mul(0.8f);
    }

    // hunched forward, arms out in front
    protected override Pose Base() => new()
    {
        Lean = 4, HeadDrop = 2, Face = Face.Fierce, FrontHand = new V(13, 5), BackHand = new V(11, 7),
        FrontElbowPref = new V(0, 1), BackElbowPref = new V(0, 1),
    };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 4, 2f);
        c.Draw(q => S.Poly(q, body) - 1, Rags, hi: Rags.Light);
        // tears in the rags showing skin, and a ragged hem
        c.Draw(q => S.Ellipse(q, r.S + new V(3, 7), 2.2f, 1.6f), Skin, outline: false);
        c.Draw(q => S.Ellipse(q, r.H + new V(-3, -5), 1.8f, 1.4f), Skin, outline: false);
        for (float x = r.H.X - 7; x < r.H.X + 8; x += 3) c.Plot(x, r.H.Y + 5, Rags.Shade);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // sores and a sickly vein or two
        c.Plot(h + new V(-3, -5), Sore); c.Plot(h + new V(-2, -5), Sore);
        c.Plot(h + new V(4, -6), SoreRed); c.Plot(h + new V(-6, 1), Sore);
        c.Plot(h + new V(1, -8), Skin.Shade); c.Plot(h + new V(2, -7), Skin.Shade);
        if (p.Prop == "groan") for (int i = 0; i < 3; i++) c.Plot(h + new V(12 + i * 2, 4 - i * 2), Col.Hex("b8c890"));
    }

    protected override List<Pose> Attack()
    {
        var list = new List<Pose>();
        (V f, V b, float lean)[] frames = [(new(9, 0), new(8, 2), 2), (new(6, -4), new(5, -2), 1), (new(16, 7), new(14, 9), 7), (new(15, 8), new(13, 9), 6), (new(13, 5), new(11, 7), 4)];
        foreach (var (f, b, lean) in frames)
        {
            var p = Base(); p.FrontHand = f; p.BackHand = b; p.Lean = lean;
            list.Add(p);
        }
        return list;
    }

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        for (int i = 0; i < 5; i++)
        {
            var p = Base(); p.Prop = i is 1 or 2 or 3 ? "groan" : "";
            if (i is 1 or 2 or 3) { p.HeadTilt = -2; p.HeadDrop = 0; p.Lean = 2; }
            list.Add(p);
        }
        return list;
    }
}
