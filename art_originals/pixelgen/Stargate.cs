// The Apophis boss (Stargate SG-1, Act 2): Apophis, his serpent guard Jaffa, and the Sarcophagus. Drawn facing right
// like the rest; the game mirrors them to face the party. Rendered by art_originals/mobgen (apophis jaffa sarcophagus),
// which also writes Apophis's map icon (images/map/apophis_boss_icon.png + _outline.png) from his idle frame.

// ---------------------------------------------------------------- Apophis
// A Goa'uld System Lord: dark skin, shaved head under a gold circlet with a cobra on the brow, a long black robe with a
// gold stripe and a broad gold-and-lapis collar, and the Kara Kesh (a gold hand device with a red stone) on his front
// hand. His eyes flash gold when he uses it. Attack: a palm blast. Cast: hand thrust out, the golden ribbon beam.
public class ApophisArt : FoeArt
{
    public override string Name => "apophis";
    static readonly Col Robe = Col.Hex("221c26"), Gold = Col.Hex("d8a838"), GoldDark = Col.Hex("8a6420");
    static readonly Col Lapis = Col.Hex("2a4aa0"), Gem = Col.Hex("e8402e"), Beam = Col.Hex("ffc850"), White = Col.Hex("fff8e0");

    public ApophisArt()
    {
        Skin = Col.Hex("6e4a32");
        HairC = Col.Hex("2a1c14"); BeardC = Col.Hex("2a1c14"); Hair = HairStyle.Bald; BeardLen = 0;
        Sleeve = Robe; Glove = Gold; Pants = Robe.Mul(0.85f); Shoes = GoldDark;
    }

    protected override Pose Base() => new() { Prop = "kara", FrontHand = new V(6, 12), BackHand = new V(-3, 14) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // a robe down to his ankles with a gold hem, and a gold sash with its ends hanging down one side
        // (no stripe down the front: with the belt it made a cross)
        var body = TorsoPoly(r, 75, 6);
        c.Draw(q => S.Poly(q, body) - 1, Robe, hi: Robe.Light);
        c.Flat(q => S.I(S.Poly(q, body) - 1, -(q.Y - 72.5f)), Gold);
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 3, r.H.X + 9, r.H.Y - 1), Gold, outline: false, hi: Gold.Light);
        c.Flat(q => S.Box(q, r.H.X + 4, r.H.Y - 1, r.H.X + 5.5f, r.H.Y + 7), GoldDark);
        c.Flat(q => S.Box(q, r.H.X + 6.5f, r.H.Y - 1, r.H.X + 8, r.H.Y + 5), GoldDark);
        // the broad collar: the lower half of a disc around his neck, gold with a lapis band
        var neck = r.S + new V(1, 0.5f);
        c.Draw(q => S.I(S.Ellipse(q, neck, 8.5f, 6f), -(q.Y - (neck.Y - 1))), Gold, hi: Gold.Light);
        c.Flat(q => S.I(MathF.Abs(S.Ellipse(q, neck, 6.2f, 4f)) - 0.55f, -(q.Y - neck.Y)), Lapis);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head; float fs = FaceShift;
        // the circlet, and a rearing gold cobra on his brow with a red eye
        c.Flat(q => S.I(S.Box(q, h.X - 10, h.Y - 6, h.X + 10, h.Y - 4.2f), S.Circle(q, h + new V(0, 0.6f), 9.6f)), Gold);
        var cobra = new V(h.X + 4.5f - fs, h.Y - 7.5f);
        c.Draw(q => S.Ellipse(q, cobra, 1.6f, 2.6f), Gold, hi: Gold.Light);
        c.Plot(cobra.X, cobra.Y - 1, Gem);
        // a Goa'uld's eyes flash when he uses his power
        if (p.Glow)
            foreach (var x in new[] { h.X + 2.5f - fs, h.X + 6.5f - fs })
            {
                c.Plot(x - 0.5f, h.Y + 1, White); c.Plot(x + 0.5f, h.Y + 1, Beam);
            }
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        // the Kara Kesh: a gold band at the wrist and the red stone in the palm
        var hand = r.HandF;
        c.Plot(hand + new V(-1.5f, -1), GoldDark); c.Plot(hand + new V(-1.5f, 0), GoldDark);
        c.Plot(hand + new V(0.5f, 0), p.Glow ? White : Gem);
        if (p.Prop == "beam") Ribbon(c, hand + new V(1.5f, 0), p.Fx);
        if (p.Prop == "blast") Blast(c, hand + new V(3, 0), p.Fx);
    }

    // The ribbon device's beam: a wavy golden ribbon out to the edge, brightest at the hand.
    private static void Ribbon(Canvas c, V from, int phase)
    {
        for (float x = from.X; x < 64; x += 0.5f)
        {
            float t = x - from.X;
            float y = from.Y + MathF.Sin(t * 0.55f + phase * 2.1f) * MathF.Min(2.2f, t * 0.25f);
            c.Plot(x, y, t < 4 ? White : Beam);
            c.Plot(x, y + 1, Beam.Mul(0.85f));
        }
        Px.Puff(c, from, 2.2f, White);
    }

    // The palm blast: a ring of force that grows over two frames.
    private static void Blast(Canvas c, V at, int fx)
    {
        float rr = fx == 1 ? 3f : 5.5f;
        c.Flat(q => MathF.Abs(S.Circle(q, at + new V(fx == 1 ? 0 : 3, 0), rr)) - 0.5f, fx == 1 ? White : Beam);
    }

    protected override List<Pose> Attack()
    {
        var list = Strike([(new(2, 10), 0, -2, 0), (new(-2, 7), 0, -4, 0), (new(14, 3), 0, 5, 1),
            (new(14, 3), 0, 5, 2), (new(10, 8), 0, 2, 0), (new(6, 12), 0, 0, 0)]);
        foreach (var p in list) { p.Prop = p.Fx > 0 ? "blast" : "kara"; p.Glow = p.Fx > 0; }
        return list;
    }

    protected override List<Pose> Cast()
    {
        (V hand, int fx)[] frames = [(new(6, 8), 0), (new(10, 2), 0), (new(15, 0), 1), (new(15, 0), 2), (new(15, 0), 3), (new(10, 6), 0), (new(6, 12), 0)];
        var list = new List<Pose>();
        foreach (var (hand, fx) in frames)
        {
            var p = Base(); p.FrontHand = hand; p.FrontElbowPref = new V(0, 1); p.Fx = fx;
            p.Prop = fx > 0 ? "beam" : "kara"; p.Glow = fx > 0 || hand.Y < 4;
            p.Face = fx > 0 ? Face.Fierce : Face.Normal; p.Lean = fx > 0 ? 3 : 0;
            list.Add(p);
        }
        return list;
    }

    /// <summary>
    /// The boss icon on the map: his idle frame, 3x, in greys (the map tints it with the act's colors, like vanilla's
    /// placeholder boss icons), and a 4-pixel ring around it for the outline image. Both 256 x 256.
    /// </summary>
    public static (Canvas icon, Canvas outline) MapIcon(Canvas idle)
    {
        const int Size = 256, K = 3, Ring = 4;
        int ox = (Size - idle.W * K) / 2, oy = (Size - idle.H * K) / 2;
        var icon = new Canvas(Size, Size);
        for (int y = 0; y < idle.H; y++)
            for (int x = 0; x < idle.W; x++)
                if (idle.Get(x, y) is { } px)
                {
                    byte g = (byte)(90 + (px.R * 0.3f + px.G * 0.59f + px.B * 0.11f) * 165 / 255);
                    for (int dy = 0; dy < K; dy++) for (int dx = 0; dx < K; dx++) icon.Set(ox + x * K + dx, oy + y * K + dy, new Col(g, g, g));
                }
        var outline = new Canvas(Size, Size);
        for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                if (icon.Get(x, y) != null) continue;
                bool near = false;
                for (int dy = -Ring; dy <= Ring && !near; dy++)
                    for (int dx = -Ring; dx <= Ring && !near; dx++)
                        near = dx * dx + dy * dy <= Ring * Ring && icon.Get(x + dx, y + dy) != null;
                if (near) outline.Set(x, y, new Col(255, 255, 255));
            }
        return (icon, outline);
    }
}

// ---------------------------------------------------------------- Jaffa
// One of Apophis's serpent guards: bald, with his gold serpent mark on the forehead, steel shoulder plates, chainmail over a leather
// kilt, and a staff weapon (a long bronze staff with a bulbous head that spits plasma). Attack: levels the staff and
// fires. Cast ("Kree!"): staff upright, fist raised.
public class JaffaArt : FoeArt
{
    public override string Name => "jaffa";
    static readonly Col Mail = Col.Hex("7e848c"), Leather = Col.Hex("5a3e2a"), Helm = Col.Hex("a8b0b8");
    static readonly Col Mark = Col.Hex("e0b040"), Staff = Col.Hex("b8904a"), Plasma = Col.Hex("ffb040"), White = Col.Hex("fff4d8");

    public JaffaArt()
    {
        Skin = Col.Hex("5a3c28");
        HairC = Col.Hex("1a1410"); BeardC = Col.Hex("1a1410"); Hair = HairStyle.Bald; BeardLen = 0;
        Sleeve = Mail; Forearm = Leather; Pants = Col.Hex("3a2c22"); Shoes = Leather.Mul(0.7f);
    }

    protected override Pose Base() => new() { Prop = "staff", PropAngle = -70, FrontHand = new V(9, 11), FrontElbowPref = new V(0, 1), BackHand = new V(2, 14) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // chainmail shirt (a checker of darker links), a belt, and a kilt of leather strips
        var body = TorsoPoly(r, r.H.Y + 2, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1, Mail, hi: Mail.Light);
        for (float y = r.S.Y + 2; y < r.H.Y + 1; y += 2)
            for (float x = r.S.X - 9; x < r.S.X + 10; x += 2)
            {
                var q = new V(x + (y % 4 < 2 ? 0 : 1), y);
                if (S.Poly(q, body) < -1.5f) c.Plot(q, Mail.Shade);
            }
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 1, r.H.X + 8, r.H.Y + 1), Leather.Mul(0.75f), outline: false);
        for (int i = 0; i < 5; i++)
        {
            float x = r.H.X - 6 + i * 3.2f;
            c.Draw(q => S.Box(q, x, r.H.Y + 1.5f, x + 2.4f, r.H.Y + 9), Leather, hi: Leather.Light);
        }
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        // Apophis's mark on his forehead: a gold serpent emblem (a ring with a tail curling down). A serpent helmet
        // hid the face and read as a grey balloon at this size.
        var h = r.Head; float fs = FaceShift;
        var mark = new V(h.X + 4.5f - fs, h.Y - 4.5f);
        c.Flat(q => MathF.Abs(S.Circle(q, mark, 1.6f)) - 0.5f, Mark);
        c.Plot(mark.X, mark.Y + 2, Mark); c.Plot(mark.X + 1, mark.Y + 2.5f, Mark);
        // shoulder plates over the mail
        c.Draw(q => S.Ellipse(q, r.SF + new V(0, 0.5f), 3.6f, 2.6f), Helm, hi: Helm.Light);
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "staff") return;
        var h = r.HandF; var d = V.Dir(p.PropAngle);
        c.Draw(q => S.Capsule(q, h - d * 14, h + d * 17, 1f), Staff, hi: Staff.Light);
        var tip = h + d * 19;
        c.Draw(q => S.Ellipse(q, tip, 2.4f, 2.4f), Staff, hi: Staff.Light);
        c.Plot(tip + d * 1.5f, Col.Hex("2a2018"));
        if (p.Fx == 1)
        {
            Px.Puff(c, tip + d * 6, 2.8f, Plasma);
            Px.Puff(c, tip + d * 6, 1.4f, White);
        }
    }

    protected override List<Pose> Attack() => Strike([
        (new(8, 9), -45, 0, 0), (new(11, 6), -10, 1, 0), (new(13, 5), 0, 3, 1),
        (new(12, 6), 0, 2, 0), (new(10, 9), -45, 1, 0), (new(9, 11), -70, 0, 0)]);

    // "Kree!": staff stood upright, free fist punched into the air.
    protected override List<Pose> Cast()
    {
        V[] fist = [new(2, 14), new(4, 2), new(5, -9), new(5, -9), new(4, 2), new(2, 14)];
        var list = new List<Pose>();
        for (int i = 0; i < fist.Length; i++)
        {
            var p = Base(); p.PropAngle = -88; p.FrontHand = new V(10, 10); p.BackHand = fist[i]; p.BackElbowPref = new V(0, 1);
            p.Face = i is 2 or 3 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Sarcophagus
// 64 x 44. A golden Goa'uld sarcophagus on a stone dais: lapis bands, a row of glyphs that pulse blue, and a lid that
// slides open with light pouring out when it heals Apophis. Hit: jolts red. Dead: cracks, the lid falls askew, the
// glow goes out.
public class SarcophagusArt : IFrameArt
{
    public string Name => "sarcophagus";
    const int W = 64, H = 44;
    static readonly Col Gold = Col.Hex("d8a838"), GoldDark = Col.Hex("8a6420"), Lapis = Col.Hex("2a4aa0");
    static readonly Col Stone = Col.Hex("6a5a4a"), Glyph = Col.Hex("6ac8ff"), Light = Col.Hex("fff0b0");

    record struct P(int Glow = 0, int Open = 0, float Beam = 0, int Jolt = 0, float Red = 0, int Broken = 0);
    // Open: how far the lid has slid (pixels). Broken: 0 = whole, 1 = cracked, 2 = lid fallen and dark.

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = Enumerable.Range(0, 8).Select(i => Draw(new P(Glow: i))).ToList(),
        ["attack"] = new P[] { new(Jolt: 1), new(Jolt: 2), new(Jolt: 1), new() }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Open: 3), new(Open: 7, Beam: 0.6f), new(Open: 10, Beam: 1), new(Open: 10, Beam: 1, Glow: 4), new(Open: 6, Beam: 0.5f), new(Open: 2) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Jolt: 1, Red: 0.4f), new(Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new P[] { new(Red: 0.3f, Broken: 1, Jolt: 1), new(Broken: 1), new(Broken: 2) }.Select(Draw).ToList(),
    };

    Canvas Draw(P p)
    {
        var c = new Canvas(W, H);
        int j = p.Jolt;
        // the dais: two stone steps
        Px.Mottled(c, 1, 38, 62, 43, [Stone, Stone.Mul(0.9f), Stone.Mul(1.08f)], 7);
        Px.Mottled(c, 5, 34, 58, 37, [Stone.Mul(1.1f), Stone, Stone.Mul(1.18f)], 9);
        // the casket: gold with lapis bands and a row of glyphs between them
        Px.Rect(c, 8, 20 - j, 55, 33, Gold);
        Px.Rect(c, 8, 32, 55, 33, GoldDark);
        Px.Rect(c, 8, 23 - j, 55, 23 - j, Lapis);
        Px.Rect(c, 8, 30, 55, 30, Lapis);
        bool lit = p.Broken < 2;
        for (int x = 11; x < 53; x += 3)
        {
            bool bright = lit && ((x / 3 + p.Glow) % 8 < 3 || p.Beam > 0);
            c.Set(x, 26 - j, bright ? Glyph : GoldDark);
            c.Set(x + 1, 27 - j, bright ? Glyph.Mul(0.8f) : GoldDark);
        }
        Px.Rect(c, 8, 20 - j, 9, 33, GoldDark); Px.Rect(c, 54, 20 - j, 55, 33, Gold.Light);
        // the lid: a wider slab with a raised ridge, slid right by Open; broken, it has slipped off to the side
        int o = p.Broken == 2 ? 9 : p.Open, drop = p.Broken == 2 ? 3 : 0;
        Px.Rect(c, 6 + o, 14 - j + drop, 57 + o, 19 - j + drop, Gold.Light);
        Px.Rect(c, 6 + o, 14 - j + drop, 7 + o, 19 - j + drop, GoldDark);
        Px.Rect(c, 12 + o, 12 - j + drop, 51 + o, 13 - j + drop, Gold);
        for (int x = 14; x < 50; x += 4) c.Set(x + o, 16 - j + drop, Lapis);
        // light pouring out of the gap
        if (p.Beam > 0)
            for (int x = 8; x < 8 + o; x++)
                for (int y = 0; y < 14; y++)
                    if ((x + y) % 2 == 0 || p.Beam >= 1) c.Set(x, y, Light.Lerp(Gold, y / 20f));
        if (p.Broken > 0)
        {
            Px.Line(c, new V(20, 21), new V(26, 28), Col.Hex("3a2a10"));
            Px.Line(c, new V(26, 28), new V(23, 33), Col.Hex("3a2a10"));
            Px.Line(c, new V(40, 20), new V(37, 26), Col.Hex("3a2a10"));
        }
        Px.Outline(c);
        if (p.Broken == 2) Px.Tint(c, Col.Hex("3a3040"), 0.35f);
        Px.Tint(c, Col.Hex("ff4040"), p.Red);
        return c;
    }
}
