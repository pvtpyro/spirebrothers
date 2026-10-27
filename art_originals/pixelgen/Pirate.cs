// Captain Blockbeard, the insult pirate (Monkey Island meets Minecraft): red coat, tricorn hat, eye patch, a beard
// that is perfectly square, a blocky cyan cutlass, and a parrot on his shoulder. Drawn facing right like the
// brothers; the game mirrors him to face the party. Saved to out/blockbeard/frames/.

public class PirateArt : AnimatedBrother
{
    public override string Name => "blockbeard";
    static readonly Col Coat = Col.Hex("b3262e");
    static readonly Col Gold = Col.Hex("e8c04a");
    static readonly Col HatC = Col.Hex("2a2228");
    static readonly Col Blade = Col.Hex("4fd8d0");
    static readonly Col Parrot = Col.Hex("3cb44a");

    public PirateArt()
    {
        Skin = Col.Hex("e0a98a");
        HairC = Col.Hex("1e1a1a"); BeardC = Col.Hex("1e1a1a"); Hair = HairStyle.Short; BeardLen = 2f;
        Sleeve = Coat; Pants = Col.Hex("2c2c3a"); Shoes = Col.Hex("1b1b1b");
    }

    // No campfire for him.
    public override Dictionary<string, (List<Pose> poses, float fps, bool loop)> Anims()
    {
        var anims = base.Anims();
        anims.Remove("rest");
        return anims;
    }

    // Stands with the cutlass held point-down at his side.
    protected override Pose Base() { var p = new Pose { Prop = "cutlass", PropAngle = 75 }; return p; }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // long coat, flaring out below the hips
        var body = TorsoPoly(r, r.H.Y + 11, 3.5f);
        c.Draw(q => S.Poly(q, body) - 1, Coat, hi: Coat.Light);
        // white shirt showing in a V at the chest
        c.Draw(q => S.Poly(q, r.S + new V(1, -1), r.S + new V(6, -1), r.S + new V(4, 7)), Col.Hex("f2ece0"), outline: false);
        // belt with a big gold buckle
        c.Draw(q => S.Box(q, r.H.X - 7, r.H.Y - 3, r.H.X + 8, r.H.Y - 0.5f), Col.Hex("2a1c14"), outline: false);
        c.Draw(q => S.Box(q, r.H.X + 2, r.H.Y - 3.5f, r.H.X + 5, r.H.Y), Gold, outline: false);
        // gold buttons down the coat front
        for (int i = 1; i < 10; i += 3) c.Plot(r.S.X + 7, r.S.Y + 2 + i, Gold);
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // The square beard: a block from the cheeks down past the chin, checkered in 3-pixel squares like a block
        // texture.
        float top = h.Y + 3.5f, bottom = h.Y + 15, left = h.X - 1.5f, right = h.X + 10;
        c.Draw(q => S.Box(q, left, top, right, bottom), BeardC, line: BeardC.Mul(0.5f), noShade: true);
        var checker = BeardC.Lerp(Col.Hex("6a5a50"), 0.35f);
        for (float y = top + 1; y < bottom - 0.5f; y++)
            for (float x = left + 1; x < right - 0.5f; x++)
                if (((int)((x - left - 1) / 3) + (int)((y - top - 1) / 3)) % 2 == 0) c.Plot(x, y, checker);
        // mouth, a gap in the beard (open while gloating or hurt)
        var mouth = Col.Hex("5a2020");
        for (int x = 4; x <= 7; x++) c.Plot(h.X + x, h.Y + 6, mouth);
        if (p.Face is Face.Fierce or Face.Hurt or Face.Happy) for (int x = 4; x <= 7; x++) c.Plot(h.X + x, h.Y + 7, mouth);
        // eye patch over the far eye, with its strap
        if (p.Face != Face.Dead)
        {
            c.Draw(q => S.Ellipse(q, h + new V(6.5f, 1), 1.9f, 1.7f), Col.Hex("141012"), outline: false);
            for (float x = h.X - 6; x < h.X + 6; x += 1) c.Plot(x, h.Y - 1.5f - (x - h.X) * 0.12f, Col.Hex("141012"));
        }
        // Tricorn hat: a rounded crown on a wide brim whose ends turn up, gold trim, and a little white skull.
        c.Draw(q => S.I(S.Ellipse(q, h + new V(1, -9), 7.5f, 5.5f), q.Y - (h.Y - 5)), HatC, hi: HatC.Light);
        var brim = new[] { h + new V(-15, -11), h + new V(-10, -5), h + new V(11, -5), h + new V(16, -12), h + new V(11, -7.5f), h + new V(-10, -7.5f) };
        c.Draw(q => S.Poly(q, brim), HatC, hi: HatC.Light);
        for (float x = h.X - 10; x <= h.X + 11; x++) c.Plot(x, h.Y - 7.5f, Gold);
        for (int i = 0; i < 4; i++) { c.Plot(h.X - 11 - i, h.Y - 8 - i * 0.8f, Gold); c.Plot(h.X + 12 + i, h.Y - 8 - i * 1f, Gold); }
        c.Plot(h.X + 1, h.Y - 11, Col.Hex("f2ece0")); c.Plot(h.X + 2, h.Y - 11, Col.Hex("f2ece0"));
        c.Plot(h.X + 1, h.Y - 10, Col.Hex("f2ece0")); c.Plot(h.X + 2, h.Y - 10, Col.Hex("f2ece0"));
    }

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        var h = r.HandF;
        if (p.Prop == "tankard")
        {
            // a wooden mug of grog with foam on top, held up toward his mouth
            var m = h + new V(1, -3);
            c.Draw(q => S.Box(q, m.X - 3, m.Y - 4, m.X + 3, m.Y + 4), Col.Hex("8a5a30"), hi: Col.Hex("b07a48"));
            c.Plot(m.X - 3, m.Y - 1, Col.Hex("5a3a20")); c.Plot(m.X - 3, m.Y + 2, Col.Hex("5a3a20"));
            c.Draw(q => S.Ellipse(q, m + new V(0, -4.5f), 3.6f, 1.6f), Col.Hex("f4ecd0"), outline: false);
            c.Draw(q => S.Capsule(q, m + new V(3.5f, -2), m + new V(3.5f, 2), 1.2f), Col.Hex("6a4224"), noShade: true);
            if (p.Fx is 2 or 3) { c.Plot(m + new V(-1, -7), Col.Hex("7ac04a")); c.Plot(m + new V(1, -8), Col.Hex("7ac04a")); }
            return;
        }
        if (p.Prop != "cutlass") return;
        // Blocky cyan cutlass: a chunky blade made of square steps, with a gold guard and a brown grip.
        var d = V.Dir(p.PropAngle); var n = d.Perp;
        c.Draw(q => S.Capsule(q, h - d * 2.5f, h + d * 1, 1.2f), Col.Hex("6a4224"), noShade: true);
        c.Draw(q => S.Capsule(q, h + d * 1.5f + n * 3, h + d * 1.5f - n * 3, 1.1f), Gold);
        c.Draw(q => S.Taper(q, h + d * 2.5f, h + d * 17 + n * 1.5f, 2.2f, 1.2f), Blade, hi: Blade.Light, line: Blade.Mul(0.45f));
        for (int i = 4; i < 16; i += 3) c.Plot(h + d * i - n * 0.5f, Blade.Light.Light);
        if (Swinging(p)) Swoosh(c, r.SF, 21, -70, 25, Col.Hex("c8fff8"));
    }

    // The swing frame of his attack. Idle frames also count Fx up from 0, so Fx alone would flash the trail (and flap
    // the parrot) while he stands still.
    private static bool Swinging(Pose p) => p.Fx == 1 && p.Face == Face.Fierce;

    // His parrot, perched on his near shoulder. It bobs along with him and flaps when he swings.
    protected override void DrawFront(Canvas c, Rig r, Pose p)
    {
        var b = r.SF + new V(-6, -4 - (Swinging(p) ? 1 : 0));
        c.Draw(q => S.Ellipse(q, b, 3f, 3.8f), Parrot, hi: Parrot.Light);
        c.Draw(q => S.Circle(q, b + new V(1, -4f), 2.4f), Col.Hex("e03c32"), hi: Col.Hex("ff6a5a"));
        c.Plot(b + new V(3.4f, -4), Col.Hex("f2c040")); c.Plot(b + new V(4.2f, -3.4f), Col.Hex("f2c040"));
        c.Plot(b + new V(1.8f, -4.8f), Brother.Eye);
        // tail feathers and a wing
        c.Draw(q => S.Taper(q, b + new V(-1, 2.5f), b + new V(-3.5f, 6.5f), 1.2f, 0.4f), Col.Hex("2a7fd4"), outline: false);
        if (Swinging(p)) c.Draw(q => S.Taper(q, b + new V(-1, -1), b + new V(-5, -4), 1.4f, 0.4f), Parrot.Shade, outline: false);
    }

    protected override List<Pose> Attack() => Swing("cutlass");

    // Cast: a big swig of grog (used for Swig of Grog and Look Behind You!).
    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hand = [new(6, 8), new(6, 2), new(4, -3), new(4, -4), new(5, 1), new(5, 9)];
        float[] tilt = [0, -1, -3, -3, -1, 0];
        for (int i = 0; i < hand.Length; i++)
        {
            var p = Base(); p.Prop = "tankard"; p.FrontHand = hand[i]; p.FrontElbowPref = new V(0, 1);
            p.HeadTilt = tilt[i]; p.Lean = tilt[i] * 0.5f; p.Fx = i;
            p.Face = i is 2 or 3 ? Face.Blink : i == 4 ? Face.Happy : Face.Normal;
            list.Add(p);
        }
        return list;
    }
}
