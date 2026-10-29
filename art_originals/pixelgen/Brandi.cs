// Brandi, Private Pyro (BRANDI_CARDS.md): long wavy dark brown hair, red tee, green camo pants, black boots, and dog tags
// stamped 33. Attack: a sniper rifle shot. Cast: flicks her Zippo. Campfire: happily staring into the fire.
// Not in Program.cs's main render list yet: `dotnet run -c Release -- out brandi` renders only her preview.

public class BrandiArt : AnimatedBrother
{
    public override string Name => "brandi";
    static readonly Col Tee = Col.Hex("c8202a");
    static readonly Col Camo = Col.Hex("4f5a35");
    static readonly Col CamoDark = Col.Hex("323a22");
    static readonly Col CamoTan = Col.Hex("7a7550");
    static readonly Col Boot = Col.Hex("1c1c20");
    static readonly Col Tag = Col.Hex("c4ccd6");
    static readonly Col Rifle = Col.Hex("2c2e33");
    static readonly Col Flash = Col.Hex("ffd060");
    static readonly Col FlameO = Col.Hex("ff8a1e");
    static readonly Col FlameY = Col.Hex("ffe060");

    public BrandiArt()
    {
        // no beard: stubble at 0 is plain skin, and BeardC only colors the mouth
        Skin = Col.Hex("f2c6a8"); HairC = Col.Hex("2a1810"); BeardC = Col.Hex("a05a5a");
        Hair = HairStyle.Bald; Stubble = true; StubbleAmt = 0;
        Sleeve = Tee; Pants = Camo; Shoes = Boot;
    }

    /// <summary>Facing the player like the vanilla characters (the test for everyone); false draws the old side-on body.</summary>
    public bool FacePlayer = true;
    public override bool Frontal => FacePlayer;

    static float Wave(V q, float amp, float freq) => MathF.Sin(q.Y * freq + q.X * 0.3f) * amp;

    protected override void DrawBehind(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // a long wavy ponytail from the back of her head down between her shoulder blades, with a red hair tie
        var tie = h + new V(-9f, -0.5f);
        var mid = tie + new V(-3.5f + p.Lean * 0.1f, 9);
        var end = mid + new V(0.5f + p.Lean * 0.15f, 11);
        c.Draw(q => S.U(S.Taper(q, tie, mid, 2.3f, 3.2f), S.Taper(q, mid, end, 3.2f, 1.2f)) + Wave(q, 0.6f, 0.9f),
            HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
        c.Plot(mid + new V(0, -3), HairC.Light); c.Plot(mid + new V(-1, 3), HairC.Light);
    }

    protected override void DrawHair(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // pulled back smooth from the forehead to the ponytail, the ear showing (Bald above, so the Rig adds no hair of its own)
        // (facing the player, the hairline stays level over the far temple and never sticks out past her face)
        float temple = Frontal ? 0 : 0.6f;
        // where a point lands on screen (the Rig draws hair scaled about the head when facing the player)
        var hc = h + new V(0, 2);
        V Screen(V q) => Frontal ? hc + (q - hc) * HairScale : q;
        c.Draw(q => S.I(S.I(S.Ellipse(q, h + new V(-0.7f, -1.2f), Frontal ? 10.2f : 11f, 11.2f) + Wave(q, 0.5f, 1.1f),
                S.U(q.Y - (h.Y - 4.5f) - MathF.Max(0, -(q.X - (h.X + 1))) * 0.9f - MathF.Max(0, q.X - (h.X + 5)) * temple,
                    S.I(q.X - (h.X - 4.5f), q.Y - (h.Y + 6.5f)))),
                // the front of her hair follows the curve of her head instead of jutting past the forehead
                Frontal && q.X > h.X ? S.Ellipse(Screen(q), h + new V(0, 0.4f), Rig.HeadR - 0.6f, Rig.HeadR + 0.4f) : -1),
            HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
    }

    protected override void DrawHeadExtras(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        // a smaller ear than the Rig's: paint its ear over with hair, then draw hers
        var ear = h + new V(-6.8f + FaceShift * 0.2f, 2.8f);
        c.Flat(q => S.I(S.Ellipse(q, h + new V(-7f + FaceShift * 0.2f, 2.6f), 3.3f, 4.1f), q.X - (h.X - 4.6f)), HairC);
        c.Draw(q => S.Ellipse(q, ear, 1.4f, 2f), Skin, outline: false);
        c.Plot(ear.X - 0.2f, ear.Y - 0.2f, Skin.Shade);
        DogTags(c, r, p);
        // the red hair tie at the base of the ponytail (here, after the hair, so the hair's scaling doesn't move it off the ponytail)
        c.Draw(q => S.Ellipse(q, h + new V(-9.2f, -0.2f), 1f, 1.5f), Tee);
        // thin brows (arched, or low and flat when she's fierce) and a lash at the outer corner of each eye
        if (p.Face is Face.Normal or Face.Fierce or Face.Happy)
            foreach (var x in new[] { h.X + 2.5f - FaceShift, h.X + 6.5f - FaceShift })
            {
                float y = p.Face == Face.Fierce ? h.Y - 1.2f : h.Y - 1.8f;
                c.Plot(x - 0.5f, y, HairC); c.Plot(x + 0.5f, y - (p.Face == Face.Fierce ? 0 : 0.4f), HairC); c.Plot(x + 1.5f, y + 0.4f, HairC);
            }
    }

    protected override void DrawArm(Canvas c, V sh, V el, V hand, bool back)
    {
        // short tee sleeve over the upper arm, bare arm below
        var skin = back ? Back(Skin) : Skin;
        c.Draw(q => S.U(S.Capsule(q, sh, el, 2.6f), S.Capsule(q, el, hand, 2.4f)), skin);
        var cuff = sh + (el - sh) * 0.6f;
        c.Draw(q => S.Capsule(q, sh, cuff, 3.4f), back ? Back(Tee) : Tee);
        if (back) DrawHand(c, hand, true);
    }

    protected override void DrawLeg(Canvas c, V hip, V knee, V foot, bool back)
    {
        var pants = back ? Back(Camo) : Camo;
        Sdf leg = q => S.U(S.Capsule(q, hip, knee, 4f), S.Capsule(q, knee, foot + new V(0, -2), 3.4f));
        c.Draw(leg, pants);
        // woodland camo blotches inside the leg
        var dark = back ? Back(CamoDark) : CamoDark; var tan = back ? Back(CamoTan) : CamoTan;
        c.Recolor(q => S.I(leg(q) + 1, 0.9f - MathF.Sin(q.X * 0.9f + q.Y * 0.35f) - MathF.Sin(q.Y * 0.8f - q.X * 0.2f)), _ => dark);
        c.Recolor(q => S.I(leg(q) + 1, 1.2f + MathF.Sin(q.X * 0.7f - q.Y * 0.5f) + MathF.Sin(q.Y * 1.1f + 1)), _ => tan);
        // black shoes, the same shape as the brothers'
        var boot = back ? Back(Boot) : Boot;
        c.Draw(q => S.I(S.U(S.Ellipse(q, foot + new V(2, 0), 5f, 3f), S.Box(q, foot.X - 3.5f, foot.Y - 4, foot.X + 3, foot.Y + 2)), q.Y - 79.5f), boot);
    }

    protected override void DrawTorso(Canvas c, Rig r, Pose p)
    {
        // camo pants up to the waist under a shorter tee, with a narrow waist between the bust and the hips
        float hem = r.H.Y - 7;   // a short torso: the waist sits high
        // (no outline, so it blends into the legs instead of drawing a line across the thighs)
        V[] hips = Frontal
            ? [new(r.H.X - 5, hem - 1), new(r.H.X + 6, hem - 1), new(r.H.X + 7, r.H.Y + 2), new(r.H.X - 6, r.H.Y + 2)]
            : [new(r.H.X - 3.5f, hem - 1), new(r.H.X + 5.5f, hem - 1), new(r.H.X + 6.5f, r.H.Y + 2), new(r.H.X - 4.5f, r.H.Y + 2)];
        c.Draw(q => S.Poly(q, hips) - 1, Camo, outline: false, noShade: true);
        c.Recolor(q => S.I(S.Poly(q, hips), 0.9f - MathF.Sin(q.X * 0.9f + q.Y * 0.35f) - MathF.Sin(q.Y * 0.8f - q.X * 0.2f)), _ => CamoDark);
        V[] body = Frontal
            ? [r.S + new V(-7, -1), r.S + new V(7.5f, -1), r.S + new V(8, 5), r.S + new V(6.5f, 10), new(r.H.X + 5, r.H.Y - 10),
               new(r.H.X + 6, hem), new(r.H.X - 5, hem), new(r.H.X - 4.5f, r.H.Y - 10), r.S + new V(-6, 10), r.S + new V(-7.5f, 5)]
            : [r.S + new V(-3.5f, -1), r.S + new V(6.5f, -1), r.S + new V(8.5f, 5), r.S + new V(6.5f, 10),
            new(r.H.X + 4.5f, r.H.Y - 10), new(r.H.X + 5.5f, hem), new(r.H.X - 3.5f, hem), new(r.H.X - 3f, r.H.Y - 10), r.S + new V(-3.5f, 8)];
        c.Draw(q => S.Poly(q, body) - 1, Tee, hi: Tee.Light);
        // a black belt at the waist, and the dog tags on a ball chain
        c.Draw(q => S.Box(q, r.H.X - (Frontal ? 5.5f : 4.5f), hem - 0.5f, r.H.X + 6.5f, hem + 1.5f), Col.Hex("1e1a18"), outline: false);
        c.Plot(new V(r.H.X + 4, hem + 0.5f), Col.Hex("b8a060"));
    }

    /// <summary>The dog tags (stamped 33): a V of ball chain from the collar and two tags, drawn after the neck so it can't cover them.</summary>
    private void DogTags(Canvas c, Rig r, Pose p)
    {
        float mx = Frontal ? 1.5f : 3;   // the middle of her chest
        for (int i = -3; i <= 3; i++) if (i != 0) c.Plot(r.S + new V(mx + i, -0.5f + (3 - MathF.Abs(i)) * 1.3f), Tag);
        var t = r.S + new V(mx, 4.5f);
        c.Draw(q => S.Box(q, t.X + 0.5f, t.Y + 0.5f, t.X + 2.5f, t.Y + 3.5f), Tag.Shade, outline: false);   // the second tag, peeking out
        c.Draw(q => S.Box(q, t.X - 1, t.Y, t.X + 1, t.Y + 3), Tag, hi: Col.Hex("ffffff"));
        if (p.Fx % 8 == 2) c.Plot(t + new V(-0.5f, 0.5f), Col.Hex("ffffff"));
    }

    // ---- props ----

    protected override void DrawWeapon(Canvas c, Rig r, Pose p)
    {
        if (p.Prop == "rifle")
        {
            var d = V.Dir(p.PropAngle); var n = d.Perp;
            var butt = r.SF + new V(-2, -1) + d * -1;
            // stock, body, long barrel, scope
            c.Draw(q => S.U(S.U(S.RBox(q, butt + d * 4 + n * 0.8f, 4, 2, p.PropAngle), S.RBox(q, butt + d * 12, 5, 1.6f, p.PropAngle)),
                S.Capsule(q, butt + d * 16, butt + d * 29, 0.7f)), Rifle, hi: Col.Hex("50545c"));
            c.Draw(q => S.RBox(q, butt + d * 11 - n * 2.6f, 3.2f, 1f, p.PropAngle), Col.Hex("18191c"), hi: Col.Hex("40444c"));
            c.Plot(butt + d * 14.5f - n * 2.6f, Col.Hex("8ec8ff"));   // scope glint
            if (p.Fx == 1)
            {
                var at = butt + d * 31.5f;
                c.Flat(q => S.Circle(q, at, 2.4f), Flash);
                c.Flat(q => S.Circle(q, at, 1.1f), Col.Hex("ffffff"));
                c.Plot(at + d * 3, Flash); c.Plot(at + n * 2.5f, Flash); c.Plot(at - n * 2.5f, Flash);
            }
            if (p.Fx == 2) for (int i = 0; i < 3; i++) c.Plot(butt + d * (31 + i * 1.5f) - n * i, Col.Hex("b8b8c0"));
            DrawHand(c, r.HandB, false);
        }
        if (p.Prop == "zippo")
        {
            // the engraved Zippo: lid flips open, a spark, then a flame she admires
            var h = r.HandF + new V(0, -2.5f);
            c.Draw(q => S.Box(q, h.X - 1.3f, h.Y - 1.5f, h.X + 1.3f, h.Y + 1.5f), Col.Hex("c4ccd6"), hi: Col.Hex("ffffff"));
            if (p.Fx >= 1) c.Draw(q => S.Box(q, h.X - 3.5f, h.Y - 3.5f, h.X - 1.3f, h.Y - 1.5f), Col.Hex("aab2bc"), outline: false);
            if (p.Fx == 2) Sparkle(c, h + new V(0.5f, -3), 1, FlameY);
            if (p.Fx >= 3)
            {
                float k = p.Fx % 2 == 0 ? 1f : 0.85f;
                var b = h + new V(0, -2);
                c.Flat(q => S.Taper(q, b, b + new V(0.6f, -6 * k), 2f, 0.2f), FlameO);
                c.Flat(q => S.Taper(q, b, b + new V(0.3f, -3.5f * k), 1.1f, 0.2f), FlameY);
                // its glow on her face
                c.Recolor(q => S.Circle(q, r.Head + new V(7, 3), 5f), px => px == Canvas.Outline ? px : px.Lerp(FlameO, 0.12f));
            }
            DrawHand(c, r.HandF, false);
        }
    }

    protected override List<Pose> Attack()
    {
        // shoulders the rifle, settles, fires, rides the recoil, lowers it
        var list = new List<Pose>();
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "rifle"; p.Fx = i is 1 or 2 ? i : 0;
            p.PropAngle = i == 0 ? 8 : i == 2 ? -6 : i == 5 ? 12 : -1;
            p.FrontHand = new V(5, 2); p.BackHand = new V(8, 0);
            p.FrontElbowPref = new V(0, 1); p.BackElbowPref = new V(0, 1);
            p.Lean = i is 2 or 3 ? -2 : 0; p.HeadTilt = i is 1 or 2 or 3 ? 1 : 0; p.HeadDrop = i is 1 or 2 or 3 ? 1 : 0;
            p.Face = i is 1 or 2 or 3 ? Face.Fierce : Face.Normal;
            list.Add(p);
        }
        return list;
    }

    protected override List<Pose> Cast()
    {
        var list = new List<Pose>();
        V[] hands = [new(6, 8), new(7, 2), new(7, 0), new(7, 0), new(7, 0), new(6, 2)];
        for (int i = 0; i < 6; i++)
        {
            var p = Base(); p.Prop = "zippo"; p.Fx = i; p.FrontHand = hands[i]; p.FrontElbowPref = new V(0, 1);
            p.Face = i >= 3 ? Face.Happy : Face.Normal;
            list.Add(p);
        }
        return list;
    }

    /// <summary>Renders her animation sheet, everyone's sheets (John and Huffman included), and a lineup of all seven.</summary>
    public static void Preview(string outDir)
    {
        var b = new BrandiArt();
        var bg = Col.Hex("5a5f6a");
        AnimatedBrother[] all = [new DanielArt(), b, new DavidArt(), new JoshuaArt(), new TimArt(), new JohnArt(), new HuffmanArt()];
        foreach (var who in all)
        {
            var whoAnims = who.Anims();
            int maxF = whoAnims.Values.Max(a => a.poses.Count);
            var sheet = new Canvas(64 * maxF + (maxF - 1) * 2, whoAnims.Count * 82 - 2);
            int row = 0;
            foreach (var (name, (poses, _, _)) in whoAnims)
            {
                for (int i = 0; i < poses.Count; i++)
                {
                    var f = who.Render(poses[i]);
                    for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) sheet.Set(i * 66 + x, row * 82 + y, f.Get(x, y) ?? bg);
                }
                row++;
            }
            sheet.SavePng(Path.Combine(outDir, $"preview_{who.Name}.png"), 3);
        }
        var anims = b.Anims();
        var lineup = new Canvas(66 * all.Length - 2, 80);
        for (int i = 0; i < all.Length; i++)
        {
            var f = all[i].Render(all[i].Anims()["idle"].poses[0]);
            for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) lineup.Set(i * 66 + x, y, f.Get(x, y) ?? bg);
        }
        lineup.SavePng(Path.Combine(outDir, "lineup_brandi.png"), 5);
        // zoomed heads and chests, for checking details
        var close = new Canvas(46 * all.Length - 2, 50);
        for (int i = 0; i < all.Length; i++)
        {
            var f = all[i].Render(all[i].Anims()["idle"].poses[0]);
            for (int y = 0; y < 50; y++) for (int x = 0; x < 44; x++) close.Set(i * 46 + x, y, f.Get(x + 8, y + 4) ?? bg);
        }
        close.SavePng(Path.Combine(outDir, "closeup_all.png"), 6);
        // just the heads, very zoomed
        var tops = new Canvas(30 * all.Length - 2, 32);
        for (int i = 0; i < all.Length; i++)
        {
            var f = all[i].Render(all[i].Anims()["idle"].poses[0]);
            for (int y = 0; y < 32; y++) for (int x = 0; x < 28; x++) tops.Set(i * 30 + x, y, f.Get(x + 16, y + 2) ?? bg);
        }
        tops.SavePng(Path.Combine(outDir, "tops_all.png"), 10);
        var big = b.Render(anims["idle"].poses[0]);
        var solo = new Canvas(64, 80);
        for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) solo.Set(x, y, big.Get(x, y) ?? bg);
        solo.SavePng(Path.Combine(outDir, "brandi_big.png"), 8);
    }
}
