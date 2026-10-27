// Goliath of Gath and his Shield Bearer (the Act 1 elite, 1 Samuel 17). Drawn facing right like the brothers; the game
// mirrors them to face the party and draws Goliath much bigger. Saved to out/goliath/frames/ and out/shield_bearer/frames/.

// Goliath: a bronze helmet with a red crest, a bronze coat of scale mail, a leather skirt, bronze greaves on his shins,
// a huge black beard, a javelin slung across his back, and a spear "like a weaver's beam".
public class GoliathArt : AnimatedBrother
{
    public override string Name => "goliath";
    static readonly Col Bronze = Col.Hex("c08a3a");
    static readonly Col BronzeDark = Col.Hex("7a5222");
    static readonly Col Crest = Col.Hex("c0302a");
    static readonly Col Leather = Col.Hex("6a4a2c");
    static readonly Col Shaft = Col.Hex("7a5530");
    static readonly Col Iron = Col.Hex("9aa2ac");

    public GoliathArt()
    {
        Skin = Col.Hex("c99a72");
        HairC = Col.Hex("1a1414"); BeardC = Col.Hex("1a1414"); Hair = HairStyle.Short; BeardLen = 5f;
        Sleeve = Skin; Forearm = Bronze; Pants = Skin.Mul(0.92f); Shoes = Leather;
    }

    public override Dictionary<string, (List<Pose> poses, float fps, bool loop)> Anims()
    {
        var anims = base.Anims();
        anims.Remove("rest");
        return anims;
    }

    // Stands with the spear planted out in front of him, leaning forward.
    protected override Pose Base() => new() { Prop = "spear", PropAngle = -72, FrontHand = new V(13, 9), FrontElbowPref = new V(0, 1) };

    // The javelin of bronze between his shoulders.
    protected override void DrawBehind(Canvas c, Rig r, Pose p)
    {
        var a = r.S + new V(-10, 14); var b = r.S + new V(6, -14);
        c.Draw(q => S.Capsule(q, a, b, 0.9f), BronzeDark, noShade: true);
        var d = (b - a).Norm;
        c.Draw(q => S.Taper(q, b - d * 1, b + d * 4, 1.6f, 0.2f), Bronze, hi: Bronze.Light);
    }

    protected override void DrawLeg(Canvas c, V hip, V knee, V foot, bool back)
    {
        base.DrawLeg(c, hip, knee, foot, back);
        // greaves of bronze upon his legs
        var g = back ? Back(Bronze) : Bronze;
        c.Draw(q => S.Capsule(q, knee + new V(0.5f, 1.5f), foot + new V(0, -3.5f), 3.6f), g, hi: g.Light, line: BronzeDark.Mul(0.6f));
        c.Plot(knee + new V(2, 3), g.Light.Light);
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // leather skirt of strips below the mail
        for (int i = 0; i < 6; i++)
        {
            float x = r.H.X - 6 + i * 2.6f;
            c.Draw(q => S.Box(q, x, r.H.Y - 2, x + 2.1f, r.H.Y + 7 - (i % 2)), i % 2 == 0 ? Leather : Leather.Light, line: Leather.Mul(0.5f));
        }
        // the coat of mail: bronze scales, row by row
        var body = TorsoPoly(r, r.H.Y + 1, 1.5f);
        c.Draw(q => S.Poly(q, body) - 1.2f, Bronze, hi: Bronze.Light, line: BronzeDark.Mul(0.55f));
        for (float y = r.S.Y + 2; y < r.H.Y; y += 3)
            for (float x = r.H.X - 6 + (y % 2); x < r.H.X + 8; x += 3)
                if (S.Poly(new V(x, y), body) < -1.5f) { c.Plot(x, y, BronzeDark); c.Plot(x + 1, y, BronzeDark); c.Plot(x - 1, y - 1, Bronze.Light); }
        // broad belt
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 3, r.H.X + 9, r.H.Y), Leather.Mul(0.7f), outline: false);
        c.Draw(q => S.Box(q, r.H.X + 2, r.H.Y - 3.5f, r.H.X + 5, r.H.Y + 0.5f), Bronze.Light, outline: false);
        // bronze shoulder guard on the near shoulder
        c.Draw(q => S.Ellipse(q, r.SF + new V(0, -0.5f), 5f, 3.6f), Bronze, hi: Bronze.Light, line: BronzeDark.Mul(0.55f));
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // mouth wide open when he roars
        if (p.Face is Face.Fierce)
        {
            var mouth = Col.Hex("4a1414");
            c.Draw(q => S.Ellipse(q, h + new V(6, 6.5f), 2.2f, 1.6f), mouth, outline: false);
        }
        // Helmet of bronze: a dome down to the brow, a nose guard, and a red crest.
        c.Draw(q => S.I(S.Circle(q, h + new V(-0.5f, -1), 11.2f), q.Y - (h.Y - 1.5f + MathF.Max(0, -(q.X - h.X)) * 0.45f)), Bronze, hi: Bronze.Light, line: BronzeDark.Mul(0.55f));
        c.Draw(q => S.Box(q, h.X + 4.6f, h.Y - 2.5f, h.X + 6.2f, h.Y + 3), Bronze, outline: false, noShade: true);
        for (float x = h.X - 9; x <= h.X + 9; x++) c.Plot(x, h.Y - 2 - MathF.Max(0, -(x - h.X)) * 0.45f, BronzeDark);
        // crest: a red brush from front to back over the top
        for (int i = 0; i < 9; i++)
        {
            var at = h + V.Dir(-160 + i * 18) * 12.2f;
            c.Draw(q => S.Circle(q, at + new V(0, -1.2f), 2.3f), Crest, hi: Crest.Light, line: Crest.Mul(0.45f));
        }
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "spear") return;
        var h = r.HandF; var d = V.Dir(p.PropAngle);
        // a long thick shaft, mostly in front of the hand, with a big iron head
        c.Draw(q => S.Capsule(q, h - d * 14, h + d * 22, 1.3f), Shaft, hi: Shaft.Light);
        var tip = h + d * 30;
        c.Draw(q => S.Poly(q, h + d * 21 + d.Perp * 2.6f, tip, h + d * 21 - d.Perp * 2.6f), Iron, hi: Col.Hex("dfe6ee"));
        if (p.Fx == 1 && p.Face == Face.Fierce)
            for (int i = 0; i < 4; i++) c.Draw(q => S.Capsule(q, h - d * (6 + i * 2) + d.Perp * (i * 2.5f - 4), h + d * (2 - i * 2) + d.Perp * (i * 2.5f - 4), 0.35f), Col.Hex("fff4d8"), outline: false, noShade: true);
    }

    // Attack: draws the spear back, then thrusts it straight at the party.
    protected override List<Pose> Attack()
    {
        var list = new List<Pose>();
        (V hand, float angle, float lean, int fx, Face face)[] frames =
        [
            (new(2, 6), -20, -2, 0, Face.Fierce),
            (new(-4, 4), -10, -5, 0, Face.Fierce),
            (new(15, 3), 0, 6, 1, Face.Fierce),
            (new(13, 4), 0, 5, 0, Face.Fierce),
            (new(9, 8), -40, 2, 0, Face.Normal),
            (new(13, 9), -72, 0, 0, Face.Normal),
        ];
        foreach (var (hand, angle, lean, fx, face) in frames)
        {
            var p = Base(); p.FrontHand = hand; p.PropAngle = angle; p.Lean = lean; p.Fx = fx; p.Face = face;
            p.FrontElbowPref = new V(0, 1);
            if (fx == 1) p.FootF = new V(4, 0);
            list.Add(p);
        }
        return list;
    }

    // Cast: throws his free arm up and roars at the ranks.
    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] back = [new(-4, 10), new(-6, -4), new(-5, -12), new(-5, -13), new(-6, -6), new(-4, 10)];
        float[] tilt = [0, -1, -3, -3, -1, 0];
        for (int i = 0; i < back.Length; i++)
        {
            var p = Base(); p.BackHand = back[i]; p.BackElbowPref = new V(-1, 0); p.HeadTilt = tilt[i]; p.Lean = tilt[i] * 0.5f;
            p.Face = i is 2 or 3 or 4 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}

// The Shield Bearer: a lean young soldier in a plain tunic and leather cap, behind a tall bronze-rimmed shield.
public class ShieldBearerArt : AnimatedBrother
{
    public override string Name => "shield_bearer";
    static readonly Col Tunic = Col.Hex("c8b48a");
    static readonly Col Leather = Col.Hex("6a4a2c");
    static readonly Col Bronze = Col.Hex("c08a3a");
    static readonly Col Hide = Col.Hex("8a5a34");

    public ShieldBearerArt()
    {
        Skin = Col.Hex("d2a07a");
        HairC = Col.Hex("2a1c14"); BeardC = Col.Hex("2a1c14"); Hair = HairStyle.Short; BeardLen = 2f;
        Sleeve = Tunic; Forearm = Skin; Pants = Skin.Mul(0.92f); Shoes = Leather;
    }

    public override Dictionary<string, (List<Pose> poses, float fps, bool loop)> Anims()
    {
        var anims = base.Anims();
        anims.Remove("rest");
        return anims;
    }

    protected override Pose Base() => new() { Prop = "shield", FrontHand = new V(7, 9), FrontElbowPref = new V(0, 1) };

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        var body = TorsoPoly(r, r.H.Y + 8, 2f);
        c.Draw(q => S.Poly(q, body) - 1, Tunic, hi: Tunic.Light);
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 3, r.H.X + 8, r.H.Y - 0.5f), Leather, outline: false);
        // strap across the chest for the shield
        c.Draw(q => S.Capsule(q, r.S + new V(-5, 0), r.H + new V(6, -4), 0.6f), Leather.Mul(0.8f), outline: false, noShade: true);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // a round leather cap
        c.Draw(q => S.I(S.Circle(q, h + new V(-0.5f, -1.5f), 10.8f), q.Y - (h.Y - 3.5f)), Leather, hi: Leather.Light);
    }

    // The shield, in front of everything: tall, rounded at the ends, hide over wood with a bronze rim and boss.
    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        if (p.Prop != "shield") return;
        var m = r.HandF + new V(4, -3);
        float tilt = p.PropAngle;
        c.Draw(q => S.RBox(q, m, 6.5f, 14.5f, tilt) - 1.2f, Bronze, hi: Bronze.Light, line: Col.Hex("4a3014"));
        c.Draw(q => S.RBox(q, m, 5f, 13f, tilt) - 0.8f, Hide, outline: false, hi: Hide.Light);
        c.Draw(q => S.Circle(q, m, 2.6f), Bronze, hi: Bronze.Light.Light);
        for (int i = -10; i <= 10; i += 5) c.Plot(m + new V(-3, i).Rot(tilt), Bronze.Light);
        if (p.Fx == 1 && p.Face == Face.Fierce)
            for (int i = -2; i <= 2; i++) c.Plot(m + new V(9, i * 4), Col.Hex("fff4d8"));
    }

    // Attack: a shove with the shield.
    protected override List<Pose> Attack()
    {
        var list = new List<Pose>();
        (V hand, float lean, int fx, Face face)[] frames =
        [
            (new(4, 9), -3, 0, Face.Fierce), (new(3, 10), -4, 0, Face.Fierce), (new(13, 8), 6, 1, Face.Fierce),
            (new(12, 8), 5, 0, Face.Fierce), (new(9, 9), 2, 0, Face.Normal), (new(7, 9), 0, 0, Face.Normal),
        ];
        foreach (var (hand, lean, fx, face) in frames)
        {
            var p = Base(); p.FrontHand = hand; p.Lean = lean; p.Fx = fx; p.Face = face;
            if (fx == 1) p.FootF = new V(3, 0);
            list.Add(p);
        }
        return list;
    }

    // Cast: lifts the shield high to cover Goliath.
    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hand = [new(7, 9), new(8, 2), new(9, -5), new(9, -6), new(8, 1), new(7, 9)];
        for (int i = 0; i < hand.Length; i++)
        {
            var p = Base(); p.FrontHand = hand[i]; p.BackHand = hand[i] + new V(-6, 1); p.BackElbowPref = new V(0, 1);
            p.Crouch = i is 2 or 3 ? 1 : 0; p.Face = i is 2 or 3 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}
