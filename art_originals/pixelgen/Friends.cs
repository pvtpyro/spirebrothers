// The two friends, John and Huffman: looks and first-draft animation poses (drawn from photos/john.png and photos/huffman.png).
// Not in Program.cs's render list yet; FriendButtons (art_originals/friendbuttons) uses them for the select button mockups.

// ---------------------------------------------------------------- John

public class JohnArt : AnimatedBrother
{
    public override string Name => "john";
    static readonly Col Tee = Col.Hex("2e2e36");
    static readonly Col Chain = Col.Hex("b8bcc4");
    static readonly Col Gun = Col.Hex("4a4e58");
    static readonly Col Flash = Col.Hex("ffd060");

    public JohnArt()
    {
        // shortish wavy brown hair, a big full beard a shade darker
        // (Bald only so the Rig adds no hair of its own: his hair is drawn in DrawHair below, and the ear still shows)
        HairC = Col.Hex("6e4b2e"); BeardC = Col.Hex("5a3a24"); Hair = HairStyle.Bald; BeardLen = 8.5f;
        Sleeve = Tee; Forearm = Skin; Pants = Col.Hex("3b4a66"); Shoes = Col.Hex("2a2622");
    }

    protected override void DrawHair(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // short and a little wavy, swept back off the forehead, down to the top of the ear at the back
        c.Draw(q => S.I(S.Circle(q, h + new V(-0.8f, -1.4f), 10.9f) + Wobble(q, h, 1.1f),
            S.U(q.Y - (h.Y - 5f) - MathF.Max(0, -(q.X - h.X)) * 0.6f + MathF.Max(0, q.X - (h.X + 3)) * 0.7f, S.I(q.X - (h.X - 4f), q.Y - (h.Y + 0.5f)))), HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
        // a couple of darker strands
        c.Plot(h.X - 1, h.Y - 7, HairC.Shade); c.Plot(h.X + 2, h.Y - 8, HairC.Shade); c.Plot(h.X - 4, h.Y - 5, HairC.Shade);
    }

    private static float Wobble(V q, V center, float amp)
    {
        var a = MathF.Atan2(q.Y - center.Y, q.X - center.X);
        return -MathF.Abs(MathF.Sin(a * 6f)) * amp;
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // thick eyebrows (only over open eyes: on the happy and hurt faces they read as an "x")
        if (p.Face is not (Face.Normal or Face.Fierce)) return;
        foreach (var x in new[] { h.X + 2.5f, h.X + 6.5f }) { c.Plot(x - 0.5f, h.Y - 1.5f, BeardC); c.Plot(x + 0.5f, h.Y - 1.5f, BeardC); }
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // a big guy in a black tee: a wider torso with a round front
        var body = TorsoPoly(r, r.H.Y + 2, 2.5f);
        c.Draw(q => S.U(S.Poly(q, body) - 1, S.Ellipse(q, r.H + new V(4.5f, -8), 6f, 8f)), Tee, hi: Tee.Light);
        // belt
        c.Draw(q => S.Box(q, r.H.X - 8.5f, r.H.Y - 1.5f, r.H.X + 9.5f, r.H.Y + 1), Col.Hex("1e1a18"), outline: false);
        // thin chain at the neck, mostly hidden by the beard
        for (int i = -3; i <= 3; i++) c.Plot(r.S.X + i, r.S.Y + 1.5f + MathF.Abs(i) * 0.3f, i % 2 == 0 ? Chain : Chain.Shade);
    }

    // Hive Scum autopistol
    private static void Pistol(Canvas c, V hand, float deg, bool flash)
    {
        var d = V.Dir(deg); var n = d.Perp;
        c.Draw(q => S.U(S.RBox(q, hand + d * 3.5f + n * -1.5f, 4.5f, 1.4f, deg), S.RBox(q, hand + n * 0.5f, 1.1f, 2.2f, deg + 15)), Gun, hi: Gun.Light);
        if (flash)
        {
            var at = hand + d * 9.5f + n * -1.5f;
            c.Flat(q => S.Circle(q, at, 2.6f), Flash);
            c.Flat(q => S.Circle(q, at, 1.2f), Col.Hex("ffffff"));
            c.Plot(at + d * 3.5f, Flash); c.Plot(at + d * 2 + n * 2.5f, Flash); c.Plot(at + d * 2 - n * 2.5f, Flash);
        }
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "pistols") return;
        Pistol(c, r.HandF, p.PropAngle, p.Fx is 1 or 3);
        DrawHand(c, r.HandF, false);
    }

    protected override void DrawHeld(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "pistols") Pistol(c, r.HandB, p.PropAngle - 4, p.Fx == 2);
        if (p.Prop == "stimm")
        {
            // the Cartel Special: a glowing stimm injector held up
            var h = r.HandF; var tip = h + new V(1, -7);
            c.Draw(q => S.Capsule(q, h + new V(0, -1), tip, 1.4f), Col.Hex("8a929c"), hi: Col.Hex("d8dee8"));
            c.Flat(q => S.Capsule(q, h + new V(0.3f, -2.5f), tip + new V(0, 1.5f), 0.6f), p.Fx % 2 == 0 ? Col.Hex("70ff80") : Col.Hex("40c050"));
            c.Plot(tip + new V(0.3f, -1.5f), Col.Hex("e8eef5"));
            if (p.Fx is 2 or 3) Sparkle(c, tip + new V(4, -2), 2, Col.Hex("70ff80"));
        }
    }

    protected override List<Pose> Attack()
    {
        // both autopistols up, three shots
        var list = new List<Pose>();
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "pistols"; p.Fx = i is 1 or 2 or 3 ? i : 0; p.PropAngle = i is 1 or 3 ? -8 : -2;
            p.FrontHand = new V(i == 0 ? 9 : 12, -1 + (i is 1 or 3 ? -1 : 0)); p.BackHand = new V(i == 0 ? 7 : 10, 1);
            p.FrontElbowPref = new V(0, 1); p.BackElbowPref = new V(0, 1);
            p.Lean = i is 1 or 2 or 3 ? -1 : 0; p.Face = i < 4 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hands = [new(6, 6), new(8, -2), new(9, -5), new(9, -5), new(8, -3), new(6, 6)];
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "stimm"; p.Fx = i; p.FrontHand = hands[i]; p.FrontElbowPref = new V(0, 1);
            p.Face = i is 2 or 3 ? Face.Happy : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Huffman

public class HuffmanArt : AnimatedBrother
{
    public override string Name => "huffman";
    static readonly Col Shirt = Col.Hex("2f63c4");
    static readonly Col Maul = Col.Hex("5a5e68");
    static readonly Col Shock = Col.Hex("8fe8ff");

    public HuffmanArt()
    {
        // short dark spiky hair, a goatee and mustache, clean cheeks
        HairC = Col.Hex("2e2119"); BeardC = Col.Hex("3a2a1e"); Hair = HairStyle.Crew; Stubble = true; StubbleAmt = 0.08f;
        Sleeve = Shirt; Pants = Col.Hex("2e3448"); Shoes = Col.Hex("1e1c20");
    }

    protected override void DrawHair(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // short on the sides, a few spikes up top
        c.Draw(q => S.U(
            S.I(S.I(S.Circle(q, h + new V(-0.3f, -0.8f), 10.8f), S.U(q.Y - (h.Y - 5f) - MathF.Max(0, -(q.X - (h.X + 2))) * 0.8f, S.I(q.X - (h.X - 4f), q.Y - (h.Y + 1)))), q.X - (h.X + 8.5f)),
            S.U(S.U(S.Taper(q, h + new V(-3, -9), h + new V(-4.5f, -12.5f), 1.8f, 0.4f), S.Taper(q, h + new V(0.5f, -9.5f), h + new V(0.5f, -13f), 1.8f, 0.4f)),
                S.U(S.Taper(q, h + new V(3.5f, -9), h + new V(5, -12f), 1.7f, 0.4f), S.Taper(q, h + new V(6, -7), h + new V(8.5f, -9f), 1.4f, 0.4f)))),
            HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // mustache over the mouth, goatee on the chin, joined at the corners of the mouth
        var b = BeardC; var bl = BeardC.Light;
        for (float x = h.X + 4; x <= h.X + 8; x++) c.Plot(x, h.Y + 5, x == h.X + 6 ? bl : b);
        c.Plot(h.X + 4, h.Y + 6, b); c.Plot(h.X + 8, h.Y + 6, b);
        for (int i = 0; i <= 4; i++) { c.Plot(h.X + 4 + i, h.Y + 7, b); c.Plot(h.X + 4 + i, h.Y + 8, i is 0 or 4 ? b.Shade : b); }
        c.Plot(h.X + 5, h.Y + 9, b.Shade); c.Plot(h.X + 6, h.Y + 9, b); c.Plot(h.X + 7, h.Y + 9, b.Shade);
        // re-draw the mouth over the mustache's lower edge
        var mouth = p.Face is Face.Hurt or Face.Dead ? Eye : Col.Hex("8a4a3a");
        c.Plot(h.X + 5, h.Y + 6, mouth); c.Plot(h.X + 6, h.Y + 6, mouth); c.Plot(h.X + 7, h.Y + 6, mouth);
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // a big guy in a blue button-up
        var body = TorsoPoly(r, r.H.Y + 2, 2.5f);
        c.Draw(q => S.U(S.Poly(q, body) - 1, S.Ellipse(q, r.H + new V(4.5f, -8), 6f, 8f)), Shirt, hi: Shirt.Light);
        // collar points and the button placket
        c.Draw(q => S.Poly(q, r.S + new V(-4, -1), r.S + new V(1, -3), r.S + new V(3.5f, 2.5f), r.S + new V(0, 1)), Shirt.Light, line: Shirt.Shade);
        c.Draw(q => S.Poly(q, r.S + new V(2, -2), r.S + new V(7, -1), r.S + new V(5, 3)), Shirt.Light, line: Shirt.Shade);
        for (int i = 3; i < 16; i += 3) c.Plot(r.S.X + 4.5f + i * 0.08f, r.S.Y + i, Col.Hex("e8eef8"));
        // belt
        c.Draw(q => S.Box(q, r.H.X - 8.5f, r.H.Y - 1.5f, r.H.X + 9.5f, r.H.Y + 1), Col.Hex("2a2018"), outline: false);
        c.Plot(r.H.X + 5, r.H.Y - 0.5f, Col.Hex("c0c4cc"));
        // IT badge on a lanyard
        for (int i = 0; i < 7; i++) c.Plot(r.S.X - 2 + i * 0.6f, r.S.Y + i, Col.Hex("202020"));
        c.Draw(q => S.Box(q, r.S.X + 1, r.S.Y + 7, r.S.X + 4.5f, r.S.Y + 11.5f), Col.Hex("f2f2f2"), noShade: true);
        c.Plot(r.S.X + 2, r.S.Y + 9, Col.Hex("3070d0")); c.Plot(r.S.X + 3, r.S.Y + 9, Col.Hex("3070d0"));
    }

    protected override void DrawArm(Canvas c, V sh, V el, V hand, bool back)
    {
        base.DrawArm(c, sh, el, hand, back);
        // buttoned cuff, a shade darker than the sleeve
        var cuff = el + (hand - el) * 0.6f;
        c.Draw(q => S.Capsule(q, cuff, cuff + (hand - el).Norm * 0.5f, 2.9f), back ? Back(Shirt.Shade) : Shirt.Shade);
        DrawHand(c, hand, back);
    }

    // Arbites shock maul
    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "maul") return;
        var d = V.Dir(p.PropAngle); var h = r.HandF; var n = d.Perp;
        c.Draw(q => S.U(S.Capsule(q, h - d * 2.5f, h + d * 9, 1.2f), S.RBox(q, h + d * 12.5f, 4f, 2.6f, p.PropAngle)), Maul, hi: Col.Hex("8a909c"));
        for (int i = -1; i <= 1; i++) c.Plot(h + d * (12.5f + i * 2.5f), p.Fx == 1 ? Col.Hex("ffffff") : Shock);
        if (p.Fx == 1)
        {
            Swoosh(c, r.SF, 20, -70, 20, Shock);
            for (int i = 0; i < 6; i++) c.Plot(h + d * (16 + i) + n * ((i % 2) * 2 - 1), i % 2 == 0 ? Col.Hex("ffffff") : Shock);
        }
        DrawHand(c, h, false);
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "law") return;
        // holds up a hand: "stop, that's against the law", with a little badge glint
        if (p.Fx is 2 or 3) { Sparkle(c, r.HandF + new V(4, -4), 2, Col.Hex("ffe070")); Sparkle(c, r.HandF + new V(-3, -7), 1, Col.Hex("ffffff")); }
    }

    protected override List<Pose> Attack() => Swing("maul");

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hands = [new(7, 6), new(10, -3), new(12, -6), new(12, -6), new(11, -4), new(7, 6)];
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "law"; p.Fx = i; p.FrontHand = hands[i]; p.FrontElbowPref = new V(0, 1);
            p.Face = i is 2 or 3 ? Face.Fierce : Face.Normal; p.Lean = i is 2 or 3 ? 1 : 0;
            list.Add(p);
        }
        return list;
    }
}
