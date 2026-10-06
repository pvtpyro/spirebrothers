// The four brothers: looks, props, and their animation poses.

public abstract class AnimatedBrother : Brother
{
    // name -> (poses, fps, loop)
    public virtual Dictionary<string, (List<Pose> poses, float fps, bool loop)> Anims() => new()
    {
        ["idle"] = (Idle(), 6, true),
        ["attack"] = (Attack(), 14, false),
        ["cast"] = (Cast(), 10, false),
        ["hit"] = (Hit(), 12, false),
        ["dead"] = (Dead(), 8, false),
        ["rest"] = (Rest(), 5, true),
    };

    protected virtual Pose Base() => new();

    protected virtual List<Pose> Idle()
    {
        float[] bob = [0, 0, 0, 1, 1, 1, 1, 0];
        var list = new List<Pose>();
        for (int i = 0; i < 8; i++)
        {
            var p = Base();
            p.Bob = bob[i];
            p.FrontHand += new V(0, bob[i] * 0.5f - (i is 2 or 3 ? 0.5f : 0));
            p.Face = i == 5 ? Face.Blink : Face.Normal;
            p.Glow = i % 4 < 2;
            p.Fx = i;
            list.Add(p);
        }
        return list;
    }

    /// <summary>Sitting on a log at the campfire, hands on his knees, breathing. Brothers add their own pastime.</summary>
    protected virtual Pose Sit()
    {
        var p = Base();
        p.Sitting = true; p.Crouch = 11; p.Lean = 1;
        p.FootF = new V(11, 0); p.FootB = new V(12, -1);
        p.FrontHand = new V(8, 10); p.BackHand = new V(5, 11);
        p.FrontElbowPref = new V(0, 1); p.BackElbowPref = new V(0, 1);
        return p;
    }

    protected virtual List<Pose> Rest()
    {
        float[] bob = [0, 0, 1, 1, 1, 0, 0, 0];
        var list = new List<Pose>();
        for (int i = 0; i < 8; i++)
        {
            var p = Sit(); p.Bob = bob[i]; p.Fx = i; p.Face = i == 6 ? Face.Blink : Face.Happy; p.Glow = i % 4 < 2;
            list.Add(p);
        }
        return list;
    }

    protected abstract List<Pose> Attack();
    protected abstract List<Pose> Cast();

    protected virtual List<Pose> Hit()
    {
        var a = Base(); a.Lean = -5; a.HeadTilt = -2; a.Face = Face.Hurt; a.Tint = 0.3f; a.FrontHand = new V(5, 9); a.BackHand = new V(-6, 10);
        var b = Base(); b.Lean = -4; b.Crouch = 1; b.HeadTilt = -1; b.Face = Face.Hurt; b.FrontHand = new V(4, 11); b.BackHand = new V(-5, 12);
        var c = Base(); c.Lean = -2; c.Crouch = 1; c.Face = Face.Hurt;
        var d = Base(); d.Lean = -1;
        return [a, b, c, d];
    }

    protected virtual List<Pose> Dead()
    {
        var a = Base(); a.Lean = -4; a.HeadTilt = -2; a.Face = Face.Hurt; a.Tint = 0.3f;
        var b = Base(); b.Crouch = 6; b.Lean = 1; b.HeadDrop = 1; b.Face = Face.Hurt; b.FrontHand = new V(3, 14); b.BackHand = new V(0, 14);
        var c = Base(); c.Crouch = 12; c.Lean = 4; c.HeadDrop = 2; c.Face = Face.Dead; c.FrontHand = new V(4, 14); c.BackHand = new V(2, 14); c.FootF = new V(-2, 0);
        var d = Base(); d.Crouch = 16; d.Lean = 7; d.HeadDrop = 3; d.HeadTilt = 1; d.Face = Face.Dead; d.FrontHand = new V(5, 13); d.BackHand = new V(4, 13); d.FootF = new V(-3, 0); d.FootB = new V(-2, 0);
        foreach (var p in new[] { a, b, c, d }) { p.Prop = DeadProp(p); }
        return [a, b, c, d, d.Clone()];
    }

    protected virtual string DeadProp(Pose p) => "";

    /// <summary>A standard overhead swing for a held weapon, angle = direction from the hand to the tip.</summary>
    protected List<Pose> Swing(string prop)
    {
        var f0 = Base(); f0.Lean = -2; f0.Crouch = 1; f0.FrontHand = new V(-3, -7); f0.FrontElbowPref = new V(-1, 0); f0.PropAngle = -110; f0.Face = Face.Fierce;
        var f1 = Base(); f1.Lean = -4; f1.Crouch = 2; f1.FrontHand = new V(-5, -10); f1.FrontElbowPref = new V(-1, 0); f1.PropAngle = -140; f1.Face = Face.Fierce; f1.FootF = new V(1, 0);
        var f2 = Base(); f2.Lean = 6; f2.Crouch = 1; f2.FrontHand = new V(11, 1); f2.FrontElbowPref = new V(0, 1); f2.PropAngle = 0; f2.Face = Face.Fierce; f2.FootF = new V(4, 0); f2.Fx = 1;
        var f3 = Base(); f3.Lean = 6; f3.Crouch = 2; f3.FrontHand = new V(9, 7); f3.PropAngle = 40; f3.Face = Face.Fierce; f3.FootF = new V(4, 0);
        var f4 = Base(); f4.Lean = 3; f4.Crouch = 1; f4.FrontHand = new V(6, 11); f4.PropAngle = 70; f4.FootF = new V(2, 0);
        var f5 = Base(); f5.Lean = 1; f5.FrontHand = new V(4, 14); f5.PropAngle = 80;
        var list = new List<Pose> { f0, f1, f2, f3, f4, f5 };
        foreach (var p in list) p.Prop = prop;
        return list;
    }
}

// ---------------------------------------------------------------- Daniel

public class DanielArt : AnimatedBrother
{
    public override string Name => "daniel";
    static readonly Col Coat = Col.Hex("353a48");
    static readonly Col Shirt = Col.Hex("717a88");
    static readonly Col Cyan = Col.Hex("5ff0ff");
    static readonly Col Blue = Col.Hex("4fb3d9");

    public DanielArt()
    {
        HairC = Col.Hex("4a2e1c"); BeardC = Col.Hex("4a2e1c"); Hair = HairStyle.Short;
        Sleeve = Coat; Glove = Col.Hex("262a33"); Pants = Col.Hex("2a2e38"); Shoes = Col.Hex("22252c");
    }

    protected override void DrawBehind(Canvas c, Rig r, Pose p)
    {
        // backpack with an antenna
        var top = r.S + new V(-11, -24);
        c.Draw(q => S.Capsule(q, r.S + new V(-9, 2), top, 0.5f), Col.Hex("8a92a0"));
        c.Plot(top, p.Fx % 4 < 2 ? Cyan : Blue); c.Plot(top + new V(0, -1), p.Fx % 4 < 2 ? Col.Hex("ffffff") : Cyan);
        c.Draw(q => S.Box(q, r.S.X - 13, r.S.Y + 1, r.S.X - 6, r.S.Y + 14) - 1, Col.Hex("2b2f3a"));
    }

    protected override void DrawLeg(Canvas c, V hip, V knee, V foot, bool back)
    {
        base.DrawLeg(c, hip, knee, foot, back);
        // knee pad and a blue light on the boot
        c.Draw(q => S.Ellipse(q, knee + new V(1.5f, 0), 2.6f, 3f), back ? Back(Col.Hex("3c4150")) : Col.Hex("3c4150"));
        c.Plot(foot + new V(1, -2), back ? Blue.Mul(0.7f) : Blue);
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var hemY = MathF.Min(r.H.Y + 15, 74);
        var coat = TorsoPoly(r, hemY, 3);
        c.Draw(q => S.Poly(q, coat) - 1, Coat, hi: Coat.Light);
        // shirt showing through the open front
        V[] shirt = [r.S + new V(1, -1), r.S + new V(7, -1), r.H + new V(6, -2), r.H + new V(2, -2)];
        c.Draw(q => S.Poly(q, shirt), Shirt, outline: false);
        // coat edge
        c.Draw(q => S.Capsule(q, r.S + new V(1, 0), r.H + new V(2, 13), 0.4f), Coat.Shade, outline: false, noShade: true);
        // chest harness straps
        c.Draw(q => S.Capsule(q, r.S + new V(-6, 0), r.H + new V(5, -4), 0.6f), Col.Hex("1f222a"), outline: false, noShade: true);
        // chest gadget, glowing
        var g = r.S + new V(4.5f, 7);
        c.Draw(q => S.Circle(q, g, 2.4f), Col.Hex("2b2f3a"));
        c.Flat(q => S.Circle(q, g, 1.3f), p.Glow ? Cyan : Blue);
        c.Plot(g, Col.Hex("ffffff"));
        // cable to the belt
        for (int i = 3; i < 9; i++) c.Plot(g + new V(-1 - (i % 3 == 0 ? 1 : 0), i), Blue);
        // belt + multimeter
        c.Draw(q => S.Box(q, r.H.X - 8, r.H.Y - 4, r.H.X + 9, r.H.Y - 1), Col.Hex("3a2f28"), outline: false);
        c.Draw(q => S.Box(q, r.H.X + 2, r.H.Y - 7, r.H.X + 6, r.H.Y + 1), Col.Hex("e0b020"));
        c.Plot(r.H.X + 3, r.H.Y - 6, Col.Hex("203020")); c.Plot(r.H.X + 4, r.H.Y - 6, Col.Hex("70e070"));
        // collar
        c.Draw(q => S.Poly(q, r.S + new V(-6, 1), r.S + new V(-5, -5), r.S + new V(0, -3), r.S + new V(1, 1)), Coat, hi: Coat.Light);
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "wrench") return;
        var d = V.Dir(p.PropAngle); var h = r.HandF;
        var head = h + d * 11;
        c.Draw(q => S.U(S.Capsule(q, h - d * 2, h + d * 9, 1.2f),
            S.Sub(S.Sub(S.Circle(q, head, 3.3f), S.Circle(q, head + d * 1.6f, 1.5f)), S.RBox(q, head + d * 3, 1.6f, 1.2f, p.PropAngle))), Metal, hi: Col.Hex("e8eef5"));
        if (p.Fx == 1) Swoosh(c, r.SF, 18, -70, 20, Col.Hex("dff6ff"));
    }

    // the multimeter he fiddles with at the campfire: screen flickers, now and then a spark
    private static void Tinker(Canvas c, Rig r, Pose p)
    {
        var at = r.HandF + new V(-1, -3);
        c.Draw(q => S.Box(q, at.X - 2, at.Y - 3, at.X + 3, at.Y + 3), Col.Hex("e0b020"));
        c.Flat(q => S.Box(q, at.X - 1, at.Y - 2, at.X + 2, at.Y), p.Fx % 3 == 0 ? Cyan : Col.Hex("70e070"));
        c.Plot(at + new V(0, 1.5f), Col.Hex("203020"));
        if (p.Fx is 2 or 6) Sparkle(c, at + new V(5, -5), 1, Cyan);
        if (p.Fx == 3) Sparkle(c, at + new V(6, -7), 2, Cyan);
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "tinker") { Tinker(c, r, p); DrawHand(c, r.HandF, false); return; }
        if (p.Prop != "spark") return;
        var at = r.HandF + new V(3, -3);
        float rad = p.Fx switch { 1 => 1.5f, 2 => 2.5f, 3 => 3.5f, 4 => 2f, _ => 0 };
        if (rad > 0)
        {
            c.Flat(q => S.Circle(q, at, rad + 1), Blue);
            c.Flat(q => S.Circle(q, at, rad), Cyan);
            c.Flat(q => S.Circle(q, at, rad * 0.45f), Col.Hex("ffffff"));
        }
        if (p.Fx is 2 or 3) { Sparkle(c, at + new V(6, -4), 2, Cyan); Sparkle(c, at + new V(-4, -7), 1, Cyan); Bolt(c, at, p.Fx); }
        if (p.Fx == 4) for (int a = 0; a < 360; a += 20) c.Plot(at + V.Dir(a) * 7, a % 40 == 0 ? Cyan : Blue);
    }

    private static void Bolt(Canvas c, V from, int seed)
    {
        V[] dirs = [new(1, -1), new(1, 0), new(1, -1), new(0, -1), new(1, -1)];
        var pt = from;
        for (int i = 0; i < 9; i++) { pt += dirs[(i + seed) % dirs.Length]; c.Plot(pt + new V(2, 0), i % 2 == 0 ? Col.Hex("ffffff") : Cyan); }
    }

    protected override List<Pose> Rest()
    {
        var list = base.Rest();
        foreach (var p in list) { p.Prop = "tinker"; p.FrontHand = new V(10, 7); p.BackHand = new V(5, 8); }
        return list;
    }

    protected override List<Pose> Attack() => Swing("wrench");

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hands = [new(6, 4), new(9, -4), new(10, -6), new(10, -6), new(10, -5), new(6, 6)];
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "spark"; p.Fx = i; p.FrontHand = hands[i]; p.FrontElbowPref = new V(0, 1); p.Glow = i is 2 or 3;
            p.Lean = i is 2 or 3 ? 1 : 0; p.Face = i is 2 or 3 ? Face.Happy : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- David

public class DavidArt : AnimatedBrother
{
    public override string Name => "david";
    static readonly Col Hoodie = Col.Hex("4f9a52");
    static readonly Col Gold = Col.Hex("f0c040");

    public DavidArt()
    {
        HairC = Col.Hex("5e3d24"); BeardC = Col.Hex("5e3d24"); Hair = HairStyle.Shaggy;
        Sleeve = Hoodie; Pants = Col.Hex("3b5577"); Shoes = Col.Hex("e4e4e8");
    }

    protected override void DrawBehind(Canvas c, Rig r, Pose p)
    {
        // hood bunched behind the neck
        c.Draw(q => S.Ellipse(q, r.S + new V(-4, -2), 6.5f, 4.5f), Back(Hoodie));
    }

    protected override void DrawLeg(Canvas c, V hip, V knee, V foot, bool back)
    {
        base.DrawLeg(c, hip, knee, foot, back);
        c.Draw(q => S.I(S.Ellipse(q, foot + new V(2, 1.5f), 5.5f, 1.3f), q.Y - 79.5f), back ? Back(Col.Hex("9a9aa8")) : Col.Hex("9a9aa8"), outline: false);
    }

    static readonly Col Shades = Col.Hex("16161e");
    static readonly Col Frame = Col.Hex("2a2a36");

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        // sunglasses; when he's down they slide down his nose
        var h = r.Head + new V(0, p.Face == Face.Dead ? 2 : 0);
        // big round lenses, outlined so they stand out against the hair and beard
        c.Draw(q => S.Capsule(q, h + new V(-4.5f, -0.2f), h + new V(0.5f, 0.2f), 0.7f), Frame);         // arm back to the ear
        c.Draw(q => S.U(S.Ellipse(q, h + new V(2.4f, 1.2f), 3.4f, 3.1f), S.Ellipse(q, h + new V(9f, 1.2f), 3.4f, 3.1f)), Shades);
        c.Flat(q => S.Capsule(q, h + new V(5.4f, 0f), h + new V(6.2f, 0f), 0.6f), Frame);              // bridge
        foreach (var lens in new[] { h + new V(2.4f, 1.2f), h + new V(9f, 1.2f) })                      // glints
        {
            c.Plot(lens + new V(-1.6f, -1.2f), Col.Hex("ffffff"));
            c.Plot(lens + new V(-0.6f, -2f), Col.Hex("cfe6ff"));
            c.Plot(lens + new V(-1.8f, -0.2f), Col.Hex("cfe6ff"));
        }
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 3, 1);
        c.Draw(q => S.Poly(q, body) - 1, Hoodie, hi: Hoodie.Light);
        // waistband
        c.Draw(q => S.I(S.Poly(q, body) - 1, -(q.Y - (r.H.Y - 0.5f))), Hoodie.Shade, outline: false, noShade: true);
        // front pocket
        c.Draw(q => S.Poly(q, r.H + new V(-3, -9), r.H + new V(7, -9), r.H + new V(8, -2), r.H + new V(-4, -2)), Hoodie, shade: Hoodie.Shade, outline: false);
        c.Draw(q => S.I(MathF.Abs(S.Poly(q, r.H + new V(-3, -9), r.H + new V(7, -9), r.H + new V(8, -2), r.H + new V(-4, -2))) - 0.5f, -(q.Y - (r.H.Y - 9.5f))), Hoodie.Shade, outline: false, noShade: true);
        // drawstrings
        for (int i = 0; i < 6; i++) { c.Plot(r.S.X + 3, r.S.Y + i, Col.Hex("f2f2f2")); c.Plot(r.S.X + 6, r.S.Y + i - (i == 5 ? 1 : 0), Col.Hex("f2f2f2")); }
        // gold chain (he counts his Gold)
        for (int i = -4; i <= 7; i++) c.Plot(r.S.X + i, r.S.Y + 1.5f + MathF.Abs(i - 1.5f) * 0.35f, i % 2 == 0 ? Gold : Gold.Shade);
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "dagger")
        {
            var d = V.Dir(p.PropAngle); var h = r.HandF; var n = d.Perp;
            c.Draw(q => S.Poly(q, h + d * 2 + n * 1.5f, h + d * 11.5f, h + d * 2 - n * 1.5f), Metal, hi: Col.Hex("ffffff"));
            c.Draw(q => S.Capsule(q, h + d * 1.6f + n * 2.8f, h + d * 1.6f - n * 2.8f, 0.8f), Gold);
            c.Draw(q => S.Capsule(q, h - d * 2.5f, h + d * 1, 1.1f), Col.Hex("6a3b22"));
            if (p.Fx == 1) { for (int i = 0; i < 12; i++) c.Plot(h + d * (13 + i) + n * (i % 3 - 1) * 0.3f, i % 2 == 0 ? Col.Hex("ffffff") : Col.Hex("e04040")); }
        }
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "coin") return;
        float[] rise = [0, 4, 10, 14, 9, 1];
        float[] width = [2, 2, 0.7f, 2, 0.8f, 2];
        var at = r.HandF + new V(1.5f, -2.5f - rise[p.Fx]);
        if (p.Fx == 0) return;
        c.Draw(q => S.Ellipse(q, at, width[p.Fx], 2.2f), Gold, hi: Col.Hex("fff0a0"));
        if (p.Fx == 3) { Sparkle(c, at + new V(4, -3), 2, Gold); Sparkle(c, at + new V(-4, 1), 1, Gold); }
    }

    protected override List<Pose> Rest()
    {
        // flips a coin, catches it, admires it
        int[] fx = [0, 1, 2, 3, 4, 5, 0, 0];
        var list = base.Rest();
        for (int i = 0; i < list.Count; i++)
        {
            var p = list[i]; p.Prop = "coin"; p.Fx = fx[i]; p.FrontHand = new V(11, 4); p.FrontElbowPref = new V(0, 1);
            p.HeadTilt = fx[i] is 2 or 3 ? -0.5f : 0;
        }
        return list;
    }

    protected override List<Pose> Attack()
    {
        var f0 = Base(); f0.Lean = -3; f0.Crouch = 2; f0.FrontHand = new V(-5, 5); f0.FrontElbowPref = new V(-1, 0); f0.PropAngle = -10; f0.Face = Face.Fierce;
        var f1 = Base(); f1.Lean = -4; f1.Crouch = 3; f1.FrontHand = new V(-6, 4); f1.FrontElbowPref = new V(-1, 0); f1.PropAngle = -5; f1.Face = Face.Fierce; f1.FootF = new V(1, 0);
        var f2 = Base(); f2.Lean = 7; f2.Crouch = 2; f2.FrontHand = new V(14, 1); f2.PropAngle = 0; f2.Face = Face.Fierce; f2.FootF = new V(5, 0); f2.Fx = 1; f2.BackHand = new V(-8, 10);
        var f3 = Base(); f3.Lean = 7; f3.Crouch = 2; f3.FrontHand = new V(13, 3); f3.PropAngle = 10; f3.Face = Face.Fierce; f3.FootF = new V(5, 0); f3.BackHand = new V(-7, 11);
        var f4 = Base(); f4.Lean = 3; f4.Crouch = 1; f4.FrontHand = new V(8, 8); f4.PropAngle = 30; f4.FootF = new V(2, 0);
        var f5 = Base(); f5.Lean = 1; f5.FrontHand = new V(4, 13); f5.PropAngle = 60;
        var list = new List<Pose> { f0, f1, f2, f3, f4, f5 };
        foreach (var p in list) p.Prop = "dagger";
        return list;
    }

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hands = [new(9, 6), new(11, 3), new(11, 4), new(11, 4), new(11, 3), new(10, 4)];
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "coin"; p.Fx = i; p.FrontHand = hands[i]; p.FrontElbowPref = new V(0, 1);
            p.HeadTilt = i is 2 or 3 ? -0.5f : 0; p.Face = i >= 4 ? Face.Happy : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Joshua

public class JoshuaArt : AnimatedBrother
{
    public override string Name => "joshua";
    public override float Scale => 0.88f;   // the youngest and the shortest
    static readonly Col Flannel = Col.Hex("7b5bb5");
    static readonly Col Wood = Col.Hex("c8843a");
    static readonly Col NeckC = Col.Hex("5a3a22");
    static readonly Col NoteC = Col.Hex("ffd860");

    public JoshuaArt()
    {
        HairC = Col.Hex("70482a"); BeardC = Col.Hex("70482a"); Hair = HairStyle.Shaggy; BeardLen = 3.5f;
        Sleeve = Flannel; Pants = Col.Hex("2e3448"); Shoes = Col.Hex("6b4a2b");
    }

    protected override Pose Base()
    {
        // playing position: strumming hand at the body, fretting hand up the neck
        var p = new Pose { Prop = "guitar", FrontHand = new V(-3, 13), BackHand = new V(15, 6), BackElbowPref = new V(0, 1), FrontElbowPref = new V(-1, 0.3f) };
        return p;
    }

    protected override void DrawBehind(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "guitar") c.Draw(q => S.Capsule(q, r.S + new V(-6, 0), r.H + new V(-4, -3), 0.8f), Col.Hex("3a2a20"));
        if (p.Prop == "swing" && p.PropAngle < 120) Guitar(c, r.HandF - V.Dir(p.PropAngle) * 13, p.PropAngle);
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 3, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1, Flannel, hi: Flannel.Light);
        c.Pattern(q => S.Poly(q, body) - 1, (x, y) => (x % 5 == 0) ^ (y % 5 == 0) ? Flannel.Shade : (x % 5 == 0 && y % 5 == 0 ? Flannel.Shade.Shade : null));
        // open flannel over a dark tee
        V[] tee = [r.S + new V(2, -1), r.S + new V(7, -1), r.H + new V(6, 1), r.H + new V(3, 1)];
        c.Draw(q => S.Poly(q, tee), Col.Hex("2d2a35"), outline: false);
        c.Draw(q => S.Poly(q, r.S + new V(-5, 0), r.S + new V(-3, -4), r.S + new V(1, -2), r.S + new V(2, 3)), Flannel, hi: Flannel.Light);
    }

    protected override void DrawArm(Canvas c, V sh, V el, V hand, bool back)
    {
        base.DrawArm(c, sh, el, hand, back);
        // rolled cuff
        var cuff = el + (hand - el) * 0.7f;
        c.Draw(q => S.Capsule(q, cuff, cuff + (hand - el).Norm * 0.5f, 3f), back ? Back(Flannel.Shade) : Flannel.Shade);
        DrawHand(c, hand, back);
    }

    /// <summary>The Well-Worn Guitar: an acoustic. center = body, deg = direction from the body up the neck.</summary>
    private void Guitar(Canvas c, V center, float deg)
    {
        var d = V.Dir(deg); var n = d.Perp;
        c.Draw(q => S.U(S.Circle(q, center, 6f), S.Circle(q, center + d * 6.5f, 4.5f)), Wood, hi: Col.Hex("e8b070"));
        c.Flat(q => S.Circle(q, center + d * 4f, 1.8f), Col.Hex("2a1a12"));
        c.Flat(q => S.RBox(q, center - d * 2f, 0.6f, 2.2f, deg), Col.Hex("3a2418"));
        c.Draw(q => S.U(S.Capsule(q, center + d * 9.5f, center + d * 20f, 1.15f), S.Capsule(q, center + d * 20.5f, center + d * 23f, 1.8f)), NeckC);
        for (int i = 10; i < 20; i += 2) c.Plot(center + d * i, Col.Hex("d8d0c0"));
        c.Plot(center + d * 21 + n * 2.5f, Metal); c.Plot(center + d * 22.5f - n * 2.5f, Metal);
    }

    protected override void DrawHeld(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "guitar") return;
        Guitar(c, r.H + new V(2.5f, -5), -38);
        DrawHand(c, r.HandB, true);
        if (p.Fx > 0 && p.T > 0) { }
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "swing") return;
        if (p.PropAngle >= 120)
            Guitar(c, r.HandF - V.Dir(p.PropAngle) * 13, p.PropAngle);
        DrawHand(c, r.HandB, false);
        if (p.Fx == 1) Swoosh(c, r.SF, 19, -80, 30, Col.Hex("fff3c0"));
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.T <= 0) return;
        // notes drift up and to the right as he plays
        var start = r.H + new V(8, -14);
        for (int i = 0; i < 3; i++)
        {
            float t = p.T - i * 0.3f;
            if (t <= 0 || t > 1) continue;
            Note(c, start + new V(4 + i * 5 + t * 6, -t * 22 + i * 2), i == 1 ? Col.Hex("ffffff") : NoteC);
        }
    }

    protected override List<Pose> Idle()
    {
        var list = base.Idle();
        // gentle strum on the beat
        for (int i = 0; i < list.Count; i++) list[i].FrontHand += new V(0, i % 4 == 1 ? -1.5f : 0);
        return list;
    }

    protected override List<Pose> Rest()
    {
        var list = base.Rest();
        for (int i = 0; i < list.Count; i++)
        {
            var p = list[i]; var g = Base();
            p.Prop = "guitar"; p.FrontHand = g.FrontHand + new V(0, i % 2 == 0 ? -1.5f : 0); p.BackHand = g.BackHand;
            p.FrontElbowPref = g.FrontElbowPref; p.BackElbowPref = g.BackElbowPref;
            p.T = 0.1f + (i % 8) * 0.17f;
        }
        return list;
    }

    protected override List<Pose> Attack()
    {
        // grab the neck with both hands and smash
        float[] ang = [15, 30, 165, 205, 235, 245];
        V[] hand = [new(-3, -10), new(-4, -11), new(10, -1), new(9, 5), new(6, 9), new(4, 11)];
        float[] lean = [-2, -4, 5, 6, 3, 1];
        var list = new List<Pose>();
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "swing"; p.PropAngle = ang[i]; p.FrontHand = hand[i]; p.BackOnFront = true; p.Lean = lean[i];
            p.FrontElbowPref = i < 2 ? new V(-1, 0) : new V(0, 1); p.BackElbowPref = new V(0, 1);
            p.Face = i < 4 ? Face.Fierce : Face.Normal; p.Fx = i == 2 ? 1 : 0; p.FootF = new V(i is 2 or 3 ? 4 : 0, 0); p.Crouch = i is 1 or 3 ? 2 : 0;
            list.Add(p);
        }
        return list;
    }

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.T = 0.15f + i * 0.2f; p.Face = i is 1 or 2 or 3 ? Face.Happy : Face.Normal;
            p.FrontHand += new V(0, i % 2 == 0 ? -2 : 1); p.Bob = i % 2; p.HeadTilt = i % 2 == 0 ? 0.5f : -0.5f;
            list.Add(p);
        }
        return list;
    }

    protected override string DeadProp(Pose p) => "";
    protected override List<Pose> Dead()
    {
        var list = base.Dead();
        foreach (var p in list) { p.BackElbowPref = new V(-0.5f, 1); p.FrontElbowPref = new V(-0.5f, 1); }
        list[0].Prop = "guitar"; list[0].BackHand = new V(14, 7); list[0].FrontHand = new V(-3, 13);
        return list;
    }

    protected override List<Pose> Hit()
    {
        var list = base.Hit();
        foreach (var p in list) { p.Prop = "guitar"; p.BackHand = new V(15, 6) + new V(p.Lean * 0.2f, 0); p.FrontHand = new V(-3, 13); }
        return list;
    }
}

// ---------------------------------------------------------------- Tim

public class TimArt : AnimatedBrother
{
    public override string Name => "tim";
    static readonly Col Shirt = Col.Hex("c8643c");
    static readonly Col WoodC = Col.Hex("dcb46a");
    static readonly Col Paper = Col.Hex("3f78c8");

    public TimArt()
    {
        BeardC = Col.Hex("5c3b24"); Hair = HairStyle.Bald;
        Sleeve = Shirt; Forearm = Skin; Pants = Col.Hex("b39b6b"); Shoes = Col.Hex("5a3d28");
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 2, 0.5f);
        c.Draw(q => S.Poly(q, body) - 1, Shirt, hi: Shirt.Light);
        // collar and placket
        c.Draw(q => S.Poly(q, r.S + new V(-4, -1), r.S + new V(1, -3), r.S + new V(4, 2), r.S + new V(0, 1)), Col.Hex("f2e8dc"));
        for (int i = 2; i < 9; i += 3) c.Plot(r.S.X + 4, r.S.Y + i, Col.Hex("f2e8dc"));
        // belt
        c.Draw(q => S.Box(q, r.H.X - 8, r.H.Y - 2, r.H.X + 8, r.H.Y + 0.5f), Col.Hex("4a3020"), outline: false);
        c.Plot(r.H.X + 4, r.H.Y - 1, Col.Hex("e0c060"));
        // chest pocket with pens (the Draftsman)
        c.Draw(q => S.Box(q, r.S.X + 5, r.S.Y + 4, r.S.X + 9, r.S.Y + 9), Shirt.Shade, outline: false);
        c.Plot(r.S.X + 6, r.S.Y + 3, Col.Hex("2050c0")); c.Plot(r.S.X + 6, r.S.Y + 2, Col.Hex("2050c0"));
        c.Plot(r.S.X + 8, r.S.Y + 3, Col.Hex("e04040"));
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        // pencil tucked behind his ear
        var h = r.Head;
        c.Draw(q => S.Capsule(q, h + new V(-7.5f, 0.5f), h + new V(-2.5f, -1.5f), 0.7f), Col.Hex("f0c030"));
        c.Plot(h + new V(-8.5f, 1), Col.Hex("e88a9a"));
        c.Plot(h + new V(-1.5f, -2), Col.Hex("3a3030"));
    }

    private static void Marshmallow(Canvas c, Rig r, Pose p)
    {
        var d = V.Dir(p.PropAngle); var h = r.HandF;
        var tip = h + d * 21;
        c.Draw(q => S.Capsule(q, h - d * 1.5f, tip, 0.55f), Col.Hex("8a6a44"), noShade: true);
        var puff = tip + d * 1.5f;
        Col mallow = p.Fx switch { 0 => Col.Hex("fbf6ec"), 1 => Col.Hex("f6e2b8"), 2 or 3 => Col.Hex("e8b868"), 4 or 5 => Col.Hex("c07030"), _ => Col.Hex("3a2a24") };
        c.Draw(q => S.Ellipse(q, puff, 2.6f, 2.2f), mallow, hi: mallow.Light);
        if (p.Fx is 4 or 5)
        {
            float flick = p.Fx == 4 ? 0 : 1;
            c.Flat(q => S.Taper(q, puff + new V(0, -1), puff + new V(flick, -6.5f), 2.4f, 0.3f), Col.Hex("ff8a20"));
            c.Flat(q => S.Taper(q, puff + new V(0, -1), puff + new V(flick * 0.5f, -4.5f), 1.3f, 0.2f), Col.Hex("ffe060"));
        }
        if (p.Fx is 6 or 7)
            for (int i = 0; i < 3; i++) c.Plot(puff + new V(i * 0.8f - 0.5f + (p.Fx - 6), -3.5f - i * 1.6f), Col.Hex("b8b8c0"));
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "marshmallow") { Marshmallow(c, r, p); return; }
        if (p.Prop != "tsquare") return;
        var d = V.Dir(p.PropAngle); var n = d.Perp; var h = r.HandF;
        c.Draw(q => S.Capsule(q, h - d * 1, h + d * 17, 1.3f), WoodC, hi: Col.Hex("f4dca0"));
        for (int i = 3; i < 17; i += 3) c.Plot(h + d * i + n * 0.8f, Col.Hex("6a4a2a"));
        c.Draw(q => S.Capsule(q, h - d * 2 + n * 4.5f, h - d * 2 - n * 4.5f, 1.5f), WoodC.Shade);
        if (p.Fx == 1) Swoosh(c, r.SF, 21, -70, 25, Col.Hex("fff0d0"));
    }

    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "blueprint") return;
        float[] w = [0, 3, 7, 12, 12, 5];
        var left = r.S + new V(6, 2);
        float width = w[p.Fx];
        // roll on the right end
        if (width > 0)
        {
            c.Draw(q => S.Box(q, left.X, left.Y, left.X + width, left.Y + 10), Paper);
            for (float x = left.X + 2; x < left.X + width - 1; x += 3) for (float y = left.Y + 1; y < left.Y + 10; y++) c.Plot(x, y, Paper.Light);
            for (float x = left.X + 1; x < left.X + width; x++) c.Plot(x, left.Y + 5, Paper.Light);
            if (width >= 7)
            {
                // a little house sketch
                var b = left + new V(width / 2 - 2, 3);
                c.Plot(b + new V(2, 0), Col.Hex("ffffff")); c.Plot(b + new V(1, 1), Col.Hex("ffffff")); c.Plot(b + new V(3, 1), Col.Hex("ffffff"));
                for (int i = 0; i < 5; i++) { c.Plot(b + new V(i, 2), Col.Hex("ffffff")); c.Plot(b + new V(i, 5), Col.Hex("ffffff")); }
                for (int i = 2; i < 6; i++) { c.Plot(b + new V(0, i), Col.Hex("ffffff")); c.Plot(b + new V(4, i), Col.Hex("ffffff")); }
            }
        }
        c.Draw(q => S.Capsule(q, left + new V(width + 1, -1), left + new V(width + 1, 11), 1.6f), Paper.Light);
        DrawHand(c, r.HandF, false);
        DrawHand(c, r.HandB, false);
        if (p.Fx is 3 or 4) { Sparkle(c, left + new V(width + 5, -3), 2, Col.Hex("ffffff")); Sparkle(c, left + new V(-3, -2), 1, Col.Hex("a0d0ff")); }
    }

    protected override List<Pose> Rest()
    {
        // toasting a marshmallow... it catches fire, he blows it out, starts a fresh one
        var list = base.Rest();
        foreach (var p in list)
        {
            p.Prop = "marshmallow"; p.FrontHand = new V(11, 5); p.FrontElbowPref = new V(0, 1); p.PropAngle = -18 + (p.Fx % 2) * 2;
            if (p.Fx is 4 or 5) p.Face = Face.Hurt;
        }
        return list;
    }

    protected override List<Pose> Attack() => Swing("tsquare");

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        float[] w = [0, 3, 7, 12, 12, 5];
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "blueprint"; p.Fx = i;
            var left = new V(6, 2) + new V(0, 5);     // relative to S
            // hands: back hand on the left edge, front hand on the roll
            p.BackHand = left + new V(5, 0) - new V(-5, 3) + new V(-5, 0);
            p.FrontHand = left + new V(w[i] + 1, 0) - new V(5, 3);
            p.BackElbowPref = new V(0, 1); p.FrontElbowPref = new V(0, 1);
            p.Face = i is 3 or 4 ? Face.Happy : Face.Normal; p.HeadTilt = i is 3 or 4 ? 1 : 0;
            list.Add(p);
        }
        return list;
    }
}
