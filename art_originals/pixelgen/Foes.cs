// The Bible elites' human-shaped foes (Goliath and the Shield Bearer are in Giants.cs, the lions and the Wall in
// Beasts.cs). Drawn facing right like the brothers; the game mirrors them to face the party.

// A shared base: no campfire for foes.
public abstract class FoeArt : AnimatedBrother
{
    public override Dictionary<string, (List<Pose> poses, float fps, bool loop)> Anims()
    {
        var anims = base.Anims();
        anims.Remove("rest");
        return anims;
    }

    /// <summary>Attack frames from a list of (front hand, lean, fx) with a fierce face on the strike.</summary>
    protected List<Pose> Strike((V hand, float angle, float lean, int fx)[] frames)
    {
        var list = new List<Pose>();
        foreach (var (hand, angle, lean, fx) in frames)
        {
            var p = Base(); p.FrontHand = hand; p.PropAngle = angle; p.Lean = lean; p.Fx = fx;
            p.Face = fx == 1 || lean > 2 ? Face.Fierce : Face.Normal;
            if (fx == 1) p.FootF = new V(3, 0);
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Jericho Guard
// A Canaanite gate guard: conical bronze helmet, red tunic, small round shield on the back arm, spear.
public class JerichoGuardArt : FoeArt
{
    public override string Name => "jericho_guard";
    static readonly Col Tunic = Col.Hex("9a2f2a");
    static readonly Col Bronze = Col.Hex("c08a3a");
    static readonly Col Shaft = Col.Hex("7a5530");

    public JerichoGuardArt()
    {
        Skin = Col.Hex("c99a72");
        HairC = Col.Hex("1e1612"); BeardC = Col.Hex("1e1612"); Hair = HairStyle.Short; BeardLen = 3.5f;
        Sleeve = Tunic; Forearm = Skin; Pants = Skin.Mul(0.92f); Shoes = Col.Hex("5a3a22");
    }

    protected override Pose Base() => new() { Prop = "spear", PropAngle = -75, FrontHand = new V(10, 10), FrontElbowPref = new V(0, 1), BackHand = new V(-4, 10) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 7, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1, Tunic, hi: Tunic.Light);
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 3, r.H.X + 8, r.H.Y - 0.5f), Col.Hex("5a3a22"), outline: false);
        // a little round shield strapped to the back arm
        c.Draw(q => S.Circle(q, r.HandB + new V(-1, -1), 5.5f), Bronze, hi: Bronze.Light);
        c.Draw(q => S.Circle(q, r.HandB + new V(-1, -1), 1.6f), Bronze.Light.Light);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        c.Draw(q => S.Poly(q, h + new V(-10, -2.5f), h + new V(10, -2.5f), h + new V(0, -17)), Bronze, hi: Bronze.Light);
        for (float x = h.X - 9; x <= h.X + 9; x++) c.Plot(x, h.Y - 3, Bronze.Shade);
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "spear") return;
        var h = r.HandF; var d = V.Dir(p.PropAngle);
        c.Draw(q => S.Capsule(q, h - d * 12, h + d * 18, 0.9f), Shaft, hi: Shaft.Light);
        c.Draw(q => S.Poly(q, h + d * 17 + d.Perp * 2, h + d * 24, h + d * 17 - d.Perp * 2), Bronze, hi: Bronze.Light);
        if (p.Fx == 1 && p.Face == Face.Fierce) for (int i = 0; i < 3; i++) c.Plot(h - d * (4 + i * 3) + d.Perp * 3, Col.Hex("fff4d8"));
    }

    protected override List<Pose> Attack() => Strike([
        (new(4, 7), -10, -2, 0), (new(-2, 6), -5, -4, 0), (new(14, 5), 0, 6, 1),
        (new(13, 6), 0, 5, 0), (new(11, 8), -40, 2, 0), (new(10, 10), -75, 0, 0)]);

    // Cast: braces behind the shield.
    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] back = [new(-4, 10), new(2, 4), new(6, 0), new(6, 0), new(2, 5), new(-4, 10)];
        for (int i = 0; i < back.Length; i++)
        {
            var p = Base(); p.BackHand = back[i]; p.Crouch = i is 2 or 3 ? 2 : 0; p.Face = i is 2 or 3 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// ---------------------------------------------------------------- Jericho Archer
// An archer from the ramparts: headband, green tunic, quiver, short bow.
public class JerichoArcherArt : FoeArt
{
    public override string Name => "jericho_archer";
    static readonly Col Tunic = Col.Hex("4a6a3a");
    static readonly Col Wood = Col.Hex("8a5a30");

    public JerichoArcherArt()
    {
        Skin = Col.Hex("d2a07a");
        HairC = Col.Hex("2a1c14"); BeardC = Col.Hex("2a1c14"); Hair = HairStyle.Shaggy; BeardLen = 2.5f;
        Sleeve = Tunic; Forearm = Skin; Pants = Skin.Mul(0.92f); Shoes = Col.Hex("5a3a22");
    }

    protected override Pose Base() => new() { Prop = "bow", FrontHand = new V(9, 6), FrontElbowPref = new V(0, 1), BackHand = new V(-1, 12) };

    protected override void DrawBehind(Canvas c, Rig r, Pose p)
    {
        // quiver over the back, arrows poking out
        var a = r.S + new V(-9, 14); var b = r.S + new V(-4, -6);
        c.Draw(q => S.Capsule(q, a, b, 2.4f), Col.Hex("6a4224"));
        for (int i = 0; i < 3; i++) c.Plot(b + new V(i - 1, -2 - i % 2), Col.Hex("e8e0d0"));
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 7, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1, Tunic, hi: Tunic.Light);
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 3, r.H.X + 8, r.H.Y - 0.5f), Col.Hex("5a3a22"), outline: false);
        c.Draw(q => S.Capsule(q, r.S + new V(-5, 0), r.H + new V(6, -4), 0.6f), Col.Hex("5a3a22"), outline: false, noShade: true);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        for (float x = h.X - 9; x <= h.X + 9; x++) { c.Plot(x, h.Y - 4.5f, Col.Hex("c0302a")); c.Plot(x, h.Y - 3.5f, Col.Hex("c0302a")); }
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "bow") return;
        var h = r.HandF;
        // the bow: an arc bulging toward the enemy, with its string pulled back to the back hand while aiming
        var top = h + new V(-1, -11); var bottom = h + new V(-1, 11);
        for (float t = 0; t <= 1; t += 0.04f)
        {
            float y = top.Y + (bottom.Y - top.Y) * t;
            float x = h.X + 3.5f * MathF.Sin(t * MathF.PI) - 1;
            c.Draw(q => S.Circle(q, new V(x, y), 1f), Wood, outline: false);
        }
        // idle frames count Fx up too, so the draw and the release only show on the fierce attack frames
        bool aiming = p.Fx == 2 && p.Face == Face.Fierce, loosed = p.Fx == 1 && p.Face == Face.Fierce;
        var nock = aiming ? r.HandB : new V(top.X - 0.5f, h.Y);
        for (float t = 0; t <= 1; t += 0.05f) { c.Plot(top + (nock - top) * t, Col.Hex("e8e0d0")); c.Plot(bottom + (nock - bottom) * t, Col.Hex("e8e0d0")); }
        if (aiming) for (float x = r.HandB.X; x < h.X + 6; x++) c.Plot(x, r.HandB.Y, Col.Hex("c8b08a"));
        if (loosed) for (float x = h.X + 4; x < 63; x += 2) c.Plot(x, h.Y, Col.Hex("fff4d8"));
    }

    // Attack: nock, draw, loose.
    protected override List<Pose> Attack()
    {
        var list = new List<Pose>();
        (V back, int fx, Face face)[] frames =
        [
            (new(2, 6), 0, Face.Normal), (new(4, 2), 2, Face.Fierce), (new(-1, 1), 2, Face.Fierce),
            (new(-2, 2), 1, Face.Fierce), (new(-1, 6), 0, Face.Normal), (new(-1, 12), 0, Face.Normal),
        ];
        foreach (var (back, fx, face) in frames)
        {
            var p = Base(); p.FrontHand = new V(12, 2); p.BackHand = back; p.BackElbowPref = new V(-1, 0); p.Fx = fx; p.Face = face;
            list.Add(p);
        }
        return list;
    }

    protected override List<Pose> Cast() => Attack();
}

// ---------------------------------------------------------------- Alexander the Coppersmith
// A burly smith: bald with a thick grey-streaked beard, a leather apron over a rust shirt, sooty forearms, and a big
// copper-headed hammer. Sparks fly when he forges.
public class AlexanderArt : FoeArt
{
    public override string Name => "alexander";
    static readonly Col Apron = Col.Hex("6a4224");
    static readonly Col Shirt = Col.Hex("8a3a22");
    static readonly Col Copper = Col.Hex("c8743a");
    static readonly Col Spark = Col.Hex("ffd060");

    public AlexanderArt()
    {
        Skin = Col.Hex("d8a07a");
        HairC = Col.Hex("5a524a"); BeardC = Col.Hex("5a524a"); Hair = HairStyle.Bald; BeardLen = 5.5f;
        Sleeve = Shirt; Forearm = Skin.Mul(0.8f); Pants = Col.Hex("3a3028"); Shoes = Col.Hex("2a1c14");
    }

    protected override Pose Base() => new() { Prop = "hammer", PropAngle = 80, FrontHand = new V(5, 13) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 3, 2.5f);
        c.Draw(q => S.Poly(q, body) - 1, Shirt, hi: Shirt.Light);
        // leather apron from chest to knees, with a pocket and a burn mark
        V[] apron = [r.S + new V(0, 2), r.S + new V(8, 2), r.H + new V(9, 13), r.H + new V(-3, 13)];
        c.Draw(q => S.Poly(q, apron), Apron, hi: Apron.Light);
        c.Draw(q => S.Box(q, r.H.X + 1, r.H.Y - 2, r.H.X + 6, r.H.Y + 2), Apron.Shade, outline: false);
        c.Plot(r.S.X + 5, r.S.Y + 8, Col.Hex("2a1810")); c.Plot(r.S.X + 6, r.S.Y + 9, Col.Hex("2a1810"));
        c.Draw(q => S.Capsule(q, r.S + new V(0, 2), r.S + new V(-4, -1), 0.5f), Apron.Shade, outline: false, noShade: true);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // grey streaks in the beard and bushy brows
        for (int i = 0; i < 4; i++) c.Plot(h.X + 3 + i * 1.5f, h.Y + 9 + (i % 2), Col.Hex("a8a098"));
        c.Plot(h.X + 2, h.Y - 1, HairC); c.Plot(h.X + 3, h.Y - 1.5f, HairC); c.Plot(h.X + 6, h.Y - 1.5f, HairC); c.Plot(h.X + 7, h.Y - 1, HairC);
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop is not ("hammer" or "forge")) return;
        var h = r.HandF; var d = V.Dir(p.PropAngle);
        c.Draw(q => S.Capsule(q, h - d * 2, h + d * 12, 1.1f), Col.Hex("6a4224"), noShade: true);
        var head = h + d * 13;
        c.Draw(q => S.RBox(q, head, 3.2f, 5.5f, p.PropAngle), Copper, hi: Copper.Light);
        if (p.Fx == 1 && p.Face == Face.Fierce) Swoosh(c, r.SF, 18, -70, 30, Col.Hex("ffe8b0"));
    }

    // Sparks off the anvil when he forges.
    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "forge") return;
        var at = r.HandF + new V(6, 4);
        int n = p.Fx;
        if (n is >= 2 and <= 4)
            for (int a = -150; a < 0; a += 30) c.Plot(at + V.Dir(a + n * 7) * (3 + n * 2), a % 60 == 0 ? Spark : Col.Hex("ff8a30"));
    }

    protected override List<Pose> Attack() => Swing("hammer");

    // Cast: hammers something on an anvil only he can see, then shows it off.
    protected override List<Pose> Cast()
    {
        var list = Swing("hammer");
        for (int i = 0; i < list.Count; i++) { list[i].Fx = i; if (i is >= 2 and <= 4) list[i].Prop = "forge"; }
        return list;
    }
}

// ---------------------------------------------------------------- Copper Automaton
// A riveted copper construct with glowing cyan eyes and a pressure gauge on its chest.
public class CopperAutomatonArt : FoeArt
{
    public override string Name => "copper_automaton";
    static readonly Col Copper = Col.Hex("c8743a");
    static readonly Col Glow = Col.Hex("5ff0ff");

    public CopperAutomatonArt()
    {
        Skin = Copper; HairC = Copper; BeardC = Copper.Mul(0.85f); Hair = HairStyle.Bald; BeardLen = 3f;
        Sleeve = Copper.Mul(0.9f); Forearm = Copper; Glove = Col.Hex("8a4a22"); Pants = Copper.Mul(0.85f); Shoes = Col.Hex("6a3a1a");
    }

    protected override Pose Base() => new() { FrontHand = new V(6, 12), BackHand = new V(-3, 13) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 2, 0.5f);
        c.Draw(q => S.Poly(q, body) - 1.5f, Copper, hi: Copper.Light, line: Col.Hex("5a2a10"));
        for (float y = r.S.Y + 2; y < r.H.Y; y += 4) { c.Plot(r.S.X - 4, y, Copper.Light.Light); c.Plot(r.S.X + 7, y, Copper.Light.Light); }
        // pressure gauge
        var g = r.S + new V(3, 8);
        c.Draw(q => S.Circle(q, g, 2.6f), Col.Hex("e8e0d0"), line: Col.Hex("5a2a10"));
        c.Plot(g, Col.Hex("c0302a")); c.Plot(g + V.Dir(-60 + p.Fx * 20), Col.Hex("c0302a"));
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // a bolt on top, a seam down the face, and glowing eyes over the drawn ones
        c.Draw(q => S.Box(q, h.X - 1, h.Y - 13, h.X + 2, h.Y - 9), Col.Hex("8a4a22"));
        for (float y = h.Y - 9; y < h.Y + 10; y += 2) c.Plot(h.X - 3, y, Copper.Shade);
        if (p.Face is not Face.Dead and not Face.Blink)
            foreach (var x in new[] { h.X + 2.5f, h.X + 6.5f })
            {
                c.Plot(x - 0.5f, h.Y + 1, Glow); c.Plot(x + 0.5f, h.Y + 1, Glow); c.Plot(x - 0.5f, h.Y, Glow); c.Plot(x + 0.5f, h.Y, Col.Hex("ffffff"));
            }
    }

    protected override List<Pose> Attack() => Strike([
        (new(2, 8), 0, -2, 0), (new(-2, 6), 0, -4, 0), (new(14, 4), 0, 6, 1),
        (new(13, 5), 0, 5, 0), (new(9, 9), 0, 2, 0), (new(6, 12), 0, 0, 0)]);

    protected override List<Pose> Cast() => Attack();
}
