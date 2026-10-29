public enum Face { Normal, Blink, Hurt, Dead, Fierce, Happy, Down }

public class Pose
{
    public float Bob, Lean, Crouch, HeadTilt, HeadDrop;
    public V FrontHand = new(2, 16), BackHand = new(-2, 16);
    public V FrontElbowPref = new(-0.5f, 1), BackElbowPref = new(-0.5f, 1);
    public V FootF, FootB;
    public Face Face = Face.Normal;
    public string Prop = "";
    public float PropAngle;
    public float T;         // effect progress 0..1
    public int Fx;          // effect frame
    public bool Glow;       // Daniel's gadget pulse
    public float Tint;      // red hit flash amount
    public bool BackOnFront;  // both hands on one grip (guitar swing)
    public bool Sitting;      // on a log at the campfire (rest sites)
    public Pose Clone() => (Pose)MemberwiseClone();
}

/// <summary>Joint positions for one pose, in model space (64 x 80 canvas, feet on the bottom row).</summary>
public class Rig
{
    public const float Cx = 28;
    public V H, S, Head, SF, SB, HandF, HandB, ElbowF, ElbowB, HipF, HipB, KneeF, KneeB, FootF, FootB;
    public const float HeadR = 10;
    public readonly bool Frontal;

    public Rig(Pose p, bool frontal = false)
    {
        Frontal = frontal;
        H = new V(Cx + p.Lean * 0.35f, 54 + p.Crouch);
        S = new V(Cx + p.Lean, 32 + p.Bob + p.Crouch * 1.05f);
        Head = S + new V(1 + p.HeadTilt, -13 + p.HeadDrop);
        // Three-quarter view facing right: the near (front) shoulder sits over the chest, the far one is tucked
        // behind it toward the enemy, so the torso reads as turned instead of square to the camera.
        SF = S + new V(-1.5f, 3);
        SB = S + new V(4.5f, 2);
        // Frontal (a test, Brandi only so far): the chest faces the player like the vanilla characters, one shoulder on
        // each side, and only the head turns toward the enemies.
        if (frontal) { SF = S + new V(-6, 3); SB = S + new V(6.5f, 3.5f); }
        (ElbowF, HandF) = Ik(SF, SF + p.FrontHand, 9, 9, p.FrontElbowPref);
        if (p.BackOnFront) p = WithBack(p, SF + p.FrontHand + new V(-2.5f, 1.5f) - SB);
        // facing the player, a far arm that just hangs down hangs beside the body (it was hidden behind it); raised or
        // holding arms keep their pose
        else if (frontal && p.BackHand.Y >= 12) p = WithBack(p, p.BackHand + new V(5.5f, 0));   // (from inside the shoulder, angled out: a shoulder of its own read as a second shoulder)
        (ElbowB, HandB) = Ik(SB, SB + p.BackHand, 9, 9, p.BackElbowPref);
        HipF = H + new V(frontal ? -3 : 3, 0);
        HipB = H + new V(frontal ? 3 : -3, 0);
        // Staggered stance: front foot forward, back foot behind and a touch higher (farther from the camera).
        FootF = (frontal ? new V(Cx - 4, 77) : new V(Cx + 7, 77)) + p.FootF;
        FootB = (frontal ? new V(Cx + 5, 77) : new V(Cx - 4, 76)) + p.FootB;
        (KneeF, FootF) = Ik(HipF, FootF, 12, 12, new V(1, 0));
        (KneeB, FootB) = Ik(HipB, FootB, 12, 12, new V(1, 0));
    }

    private static Pose WithBack(Pose p, V rel) { var q = p.Clone(); q.BackHand = rel; return q; }

    public static (V elbow, V hand) Ik(V a, V b, float l1, float l2, V pref)
    {
        var d = b - a;
        var len = Math.Clamp(d.Len, 0.5f, l1 + l2 - 0.05f);
        var dir = d.Norm;
        b = a + dir * len;
        float x = (l1 * l1 - l2 * l2 + len * len) / (2 * len);
        float h = MathF.Sqrt(MathF.Max(0, l1 * l1 - x * x));
        var basePt = a + dir * x;
        var e1 = basePt + dir.Perp * h; var e2 = basePt - dir.Perp * h;
        return ((e1 - a).Dot(pref) >= (e2 - a).Dot(pref) ? e1 : e2, b);
    }
}

// Messy / Fringe / Crew / Buzzed were drawn from photos of the brothers (Daniel / David / Joshua / Tim).
public enum HairStyle { Short, Shaggy, Bald, Messy, Fringe, Crew, Buzzed }

public abstract class Brother
{
    public abstract string Name { get; }
    public virtual float Scale => 1f;
    public bool Portrait;   // full size, for the select button
    public Col Skin = Col.Hex("f0c39b");
    public Col HairC = Col.Hex("5a3a22");
    public Col BeardC = Col.Hex("5a3a22");
    public HairStyle Hair = HairStyle.Shaggy;
    public float BeardLen = 4.5f;
    public bool Stubble;        // a shadow of stubble instead of a beard
    public float StubbleAmt = 0.13f;   // how dark the stubble is (0 = skin, 1 = beard color)
    public bool StubbleFull;    // stubble along the whole jaw back to the ear, not just the chin
    public Col Pants = Col.Hex("3b5577");
    public Col Shoes = Col.Hex("3a2a20");
    public Col Sleeve = Col.Hex("888888");
    public Col? Forearm;        // null = same as sleeve
    public Col? Glove;          // null = skin
    public static readonly Col Eye = Col.Hex("241a1a");
    public static readonly Col Metal = Col.Hex("b4bcc8");

    public Canvas Render(Pose p)
    {
        var c = new Canvas(64, 80) { K = (Portrait ? 1f : Scale), Origin = new V(Rig.Cx, 80) };
        var r = new Rig(p, Frontal);
        // (sitting poses have no log of their own: rest sites already have logs to sit on)
        DrawBehind(c, r, p);
        DrawArm(c, r.SB, r.ElbowB, r.HandB, back: true);
        DrawLeg(c, r.HipB, r.KneeB, r.FootB, back: true);
        DrawLeg(c, r.HipF, r.KneeF, r.FootF, back: false);
        DrawTorso(c, r, p);
        DrawHead(c, r, p);
        DrawHeld(c, r, p);
        DrawArm(c, r.SF, r.ElbowF, r.HandF, back: false);
        DrawWeapon(c, r, p);
        DrawHand(c, r.HandF, false);
        DrawFront(c, r, p);
        if (p.Tint > 0) TintAll(c, Col.Hex("ff4040"), p.Tint);
        return c;
    }

    /// <summary>The log he sits on at the campfire, under his hips.</summary>
    protected static void DrawLog(Canvas c)
    {
        var bark = Col.Hex("6b4a2b"); var cut = Col.Hex("c99a62");
        c.Draw(q => S.Capsule(q, new V(Rig.Cx - 13, 72), new V(Rig.Cx + 6, 72), 6f), bark, hi: bark.Light);
        for (float x = Rig.Cx - 10; x < Rig.Cx + 6; x += 4) c.Plot(x, 70 + (x % 8 == 0 ? 1 : 3), bark.Shade);
        c.Draw(q => S.Ellipse(q, new V(Rig.Cx - 15, 72), 3.2f, 5.6f), cut);
        c.Flat(q => MathF.Abs(S.Ellipse(q, new V(Rig.Cx - 15, 72), 1.6f, 3f)) - 0.4f, cut.Shade);
    }

    // ---- body parts ----

    protected Col Back(Col c) => c.Mul(0.78f);

    protected virtual void DrawArm(Canvas c, V sh, V el, V hand, bool back)
    {
        var sleeve = back ? Back(Sleeve) : Sleeve;
        if (Forearm is { } fa)
        {
            var mid = el + (hand - el) * 0.35f;
            c.Draw(p => S.U(S.Capsule(p, sh, el, 3.2f), S.Capsule(p, el, mid, 3f)), sleeve);
            c.Draw(p => S.Capsule(p, mid, hand, 2.4f), back ? Back(fa) : fa);
            c.Draw(p => S.Capsule(p, el, mid, 3f), sleeve);
        }
        else c.Draw(p => S.U(S.Capsule(p, sh, el, 3.2f), S.Capsule(p, el, hand, 2.8f)), sleeve);
        if (back) DrawHand(c, hand, true);
    }

    protected void DrawHand(Canvas c, V hand, bool back)
    {
        var col = Glove ?? Skin;
        c.Draw(p => S.Circle(p, hand + new V(0.5f, 0.5f), 2.6f), back ? Back(col) : col);
    }

    protected virtual void DrawLeg(Canvas c, V hip, V knee, V foot, bool back)
    {
        var pants = back ? Back(Pants) : Pants;
        c.Draw(p => S.U(S.Capsule(p, hip, knee, 4f), S.Capsule(p, knee, foot + new V(0, -2), 3.4f)), pants);
        var shoes = back ? Back(Shoes) : Shoes;
        c.Draw(p => S.I(S.U(S.Ellipse(p, foot + new V(2, 0), 5f, 3f), S.Box(p, foot.X - 3.5f, foot.Y - 4, foot.X + 3, foot.Y + 2)), p.Y - 79.5f), shoes);
    }

    protected V[] TorsoPoly(Rig r, float hemY = 0, float flare = 0)
    {
        var hem = hemY > 0 ? hemY : r.H.Y + 2;
        if (r.Frontal)
            return [r.S + new V(-7, -1), r.S + new V(7.5f, -1), r.S + new V(8, 5), new V(r.H.X + 7 + flare, hem), new V(r.H.X - 6.5f - flare, hem), r.S + new V(-7.5f, 5)];
        // Turned three-quarters to the right: narrower, with the chest pushing forward.
        return [r.S + new V(-3.5f, -1), r.S + new V(7, -1), r.S + new V(8.5f, 5), new V(r.H.X + 7 + flare, hem), new V(r.H.X - 3.5f - flare, hem)];
    }

    protected abstract void DrawTorso(Canvas c, Rig r, Pose p);
    protected virtual void DrawBehind(Canvas c, Rig r, Pose p) { }
    protected virtual void DrawHeld(Canvas c, Rig r, Pose p) { }
    protected virtual void DrawWeapon(Canvas c, Rig r, Pose p) { }
    protected virtual void DrawFront(Canvas c, Rig r, Pose p) { }
    protected virtual void DrawHeadExtras(Canvas c, Rig r, Pose p) { }

    /// <summary>Faces the player (see Rig.Frontal). The face features slide toward the middle of the head by FaceShift.</summary>
    public virtual bool Frontal => true;
    protected float FaceShift => Frontal ? 2f : 0;
    /// <summary>How much smaller the hair is drawn when facing the player (the head is smaller).</summary>
    protected virtual float HairScale => 0.9f;
    /// <summary>0..1: facing the player, how much rounder (wider and shorter) the skull is. Tim's bald head read as an egg.</summary>
    protected virtual float RoundHead => 0;

    /// <summary>A round hairstyle (or Tim's headphone band) rasterizes with a short strip sitting on a much wider row at the
    /// very top, which reads as a little tab. Drop that strip and outline the row under it, so the crown is one smooth curve.
    /// Only looks at what was drawn since <paramref name="before"/>, and only above <paramref name="crownY"/>.</summary>
    private void SmoothCrown(Canvas c, Col?[] before, float crownY, bool lone = false)
    {
        int top = -1;
        for (int y = 0; y < c.H && top < 0; y++)
            for (int x = 0; x < c.W; x++) if (c.Px[y * c.W + x] != before[y * c.W + x]) { top = y; break; }
        if (top < 0 || top + 1 >= c.H || c.ModelOf(new V(0, top + 0.5f)).Y > crownY) return;
        int Count(int y) { int n = 0; for (int x = 0; x < c.W; x++) if (c.Px[y * c.W + x] != before[y * c.W + x]) n++; return n; }
        int n1 = Count(top), n2 = Count(top + 1);
        if (n1 < (lone ? 1 : 2) || n1 > 7 || n2 < n1 + 2) return;   // (a lone hair pixel is a tuft or spike tip: leave it)
        var line = Canvas.Outline;
        for (int x = 0; x < c.W; x++) if (c.Px[top * c.W + x] is { } q && c.Px[top * c.W + x] != before[top * c.W + x]) { line = q; break; }
        for (int x = 0; x < c.W; x++)
        {
            int i = top * c.W + x;
            if (c.Px[i] == before[i]) continue;
            c.Px[i] = before[i];
            int below = (top + 1) * c.W + x;
            if (c.Px[below] != before[below]) c.Px[below] = line;
        }
    }

    protected void DrawHead(Canvas c, Rig r, Pose p)
    {
        var h = r.Head; var fs = FaceShift;
        // neck
        if (r.Frontal)   // under the middle of the chin, stopping at the collar instead of running down the chest
            c.Draw(q => S.Capsule(q, r.S + new V(1.5f, -1.5f), h + new V(1, 7), 2.8f), Skin.Lerp(Skin.Shade, 0.6f));
        else
            c.Draw(q => S.Capsule(q, r.S + new V(0, 1), h + new V(-1, 8), 3.2f), Skin.Shade);
        // back hair mass (behind the head)
        if (Hair == HairStyle.Shaggy)
            c.Draw(q => S.I(S.Circle(q, h + new V(-2.5f, 0), 11.2f) + Wobble(q, h, 0.9f), S.U(q.X - (h.X + 1), q.Y - (h.Y - 2))), HairC);
        // head
        // (facing the player, the right side of the head is pulled in so the face isn't stretched past the eyes)
        // (facing the player the skull is smaller and doesn't reach as far back: the face is turned toward us)
        // (RoundHead widens it and lowers the top, for a head with no hair to round it out: Tim)
        float rh = RoundHead;
        Sdf skull = fs > 0 ? q => S.Ellipse(q, h + new V(0, 0.4f + rh * 0.9f), Rig.HeadR - 1.4f + rh * 0.6f, Rig.HeadR - 0.8f - rh * 0.9f) : q => S.Circle(q, h, Rig.HeadR);
        float jawW = fs > 0 ? 5.5f : 6.5f;
        c.Draw(q => S.U(skull(q), S.Ellipse(q, h + new V(4 - fs, 5), jawW, 4.5f)), Skin, hi: Skin.Light);
        // nose: one dark pixel between and just below the eyes, with a soft shadow above it (smaller than an eye)
        c.Plot(h.X + 5.6f - fs, h.Y + 2.6f, Skin.Shade); c.Plot(h.X + 5.6f - fs, h.Y + 3.6f, Canvas.Outline);
        // beard: lower face plus chin, sideburn up to the ear
        if (Stubble)
        {
            // inside the face, no outline: the jaw and chin, or the whole beard area
            Sdf face = q => S.U(skull(q) + 0.4f, S.Ellipse(q, h + new V(4 - fs, 5), jawW - 0.5f, 4f));
            Sdf jaw = StubbleFull
                ? q => S.I(face(q), -(q.Y - (h.Y + 4.5f - MathF.Max(0, h.X - q.X) * 0.25f)))   // a jawline rising gently toward the ear
                : q => S.I(S.I(face(q), -(q.Y - (h.Y + 4.5f))), -(q.X - (h.X - 0.5f - fs)));
            c.Flat(jaw, Skin.Lerp(BeardC, StubbleAmt));
        }
        else
            c.Draw(q => S.I(S.U(skull(q) - 0.6f, S.Ellipse(q, h + new V(4.5f - fs, 7.5f), 6.5f - fs * 0.5f, BeardLen)) + Wobble(q, h, 0.6f),
                S.U(-(q.Y - (h.Y + 5f)) + MathF.Max(0, q.X - (h.X + 2 - fs)) * 0.15f, S.I(-(q.Y - (h.Y + 0.5f)), q.X - (h.X - 0.5f - fs)))), BeardC, hi: BeardC.Light, line: BeardC.Mul(0.6f));
        // mouth
        var mouth = p.Face == Face.Hurt || p.Face == Face.Dead ? Eye : BeardC.Shade.Mul(0.75f);
        c.Plot(h.X + 5 - fs, h.Y + 6, mouth); c.Plot(h.X + 6 - fs, h.Y + 6, mouth); c.Plot(h.X + 7 - fs, h.Y + 6, mouth);
        if (p.Face == Face.Happy) { c.Plot(h.X + 4 - fs, h.Y + 5, mouth); c.Plot(h.X + 8 - fs, h.Y + 5, mouth); }
        if (p.Face == Face.Hurt) { c.Plot(h.X + 6 - fs, h.Y + 7, mouth); }
        // top hair (facing the player, hair styles drawn for the wider side-on head would hang past the face like a cap
        // brim, so anything they put past the side of the narrower face is undone)
        var beforeHair = fs > 0 ? (Col?[])c.Px.Clone() : null;
        // every hairstyle was drawn for the bigger side-on head, so facing the player it's drawn scaled down about the
        // middle of the head (the canvas transform: scale K about the head instead of the feet)
        var (oldK, oldOff) = (c.K, c.Offset);
        if (fs > 0) { var hc = h + new V(0, 2); c.K = oldK * HairScale; c.Offset = oldOff + (hc - c.Origin) * oldK * (1 - HairScale); }
        DrawHair(c, r, p);
        (c.K, c.Offset) = (oldK, oldOff);
        if (beforeHair != null)
            for (int y = 0; y < c.H; y++)
                for (int x = 0; x < c.W; x++)
                {
                    var m = c.ModelOf(new V(x + 0.5f, y + 0.5f));
                    // (a straight line just past the side of the face: it only reaches the hair hanging beside the face,
                    // never the crown, so it can't leave a step on top of the head)
                    if (m.Y > h.Y + 8) continue;
                    int i = y * c.W + x;
                    float env = m.X - (h.X + 8f);   // (the side of the face: no strip of hair left between it and the cut)
                    if (env > 0) c.Px[i] = beforeHair[i];
                    // a fresh outline along the cut, where the hair now meets the background
                    else if (env > -1 && c.Px[i] != beforeHair[i] && (beforeHair[i] is null || beforeHair[i] == Canvas.Outline)) c.Px[i] = Canvas.Outline;
                }
        if (beforeHair != null) SmoothCrown(c, beforeHair, h.Y - 9);
        // ear, in front of short hair (shaggy hair covers it): it sits inside the head, so it gets a soft skin-tone line instead of the black outline
        if (Hair != HairStyle.Shaggy)
        {
            var ear = h + new V(-7f + fs * 0.2f, 2.6f);
            c.Draw(q => S.Ellipse(q, ear, 1.9f, 2.7f), Skin, line: Skin.Shade.Mul(0.7f), noShade: true);
            c.Plot(ear.X - 0.2f, ear.Y - 0.6f, Skin.Shade); c.Plot(ear.X - 0.2f, ear.Y + 0.4f, Skin.Shade);   // the hollow of the ear
        }
        // eyes
        DrawEyes(c, h, p.Face);
        var beforeExtras = fs > 0 ? (Col?[])c.Px.Clone() : null;
        DrawHeadExtras(c, r, p);
        if (beforeExtras != null) SmoothCrown(c, beforeExtras, h.Y - 9, lone: true);
    }

    private static float Wobble(V q, V center, float amp)
    {
        var a = MathF.Atan2(q.Y - center.Y, q.X - center.X);
        var s = MathF.Sin(a * 6f); return -s * s * amp;   // squared, not abs: smooth dips with no sharp point between them (one sat on top of every head)
    }

    protected virtual void DrawHair(Canvas c, Rig r, Pose p)
    {
        var h = r.Head;
        switch (Hair)
        {
            case HairStyle.Short:
                c.Draw(q => S.I(S.Circle(q, h + new V(-0.8f, -1.2f), 10.8f) + Wobble(q, h, 1.1f),
                    S.U(q.Y - (h.Y - 4.5f) - MathF.Max(0, -(q.X - h.X)) * 0.6f, S.I(q.X - (h.X - 3.5f), q.Y - (h.Y + 3)))), HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
                // fringe tuft
                c.Draw(q => S.Taper(q, h + new V(2, -7), h + new V(7.5f, -3.5f), 2.6f, 0.6f), HairC, outline: false, hi: HairC.Light);
                break;
            case HairStyle.Shaggy:
                // top mass with a jagged fringe over the forehead
                c.Draw(q =>
                {
                    float jag = MathF.Abs(((q.X - h.X) * 0.55f % 2f + 2f) % 2f - 1f) * 2.2f;
                    return S.I(S.Circle(q, h + new V(-1, -1.5f), 12f) + Wobble(q, h, 1.3f), q.Y - (h.Y - 4.2f + jag + MathF.Max(0, -(q.X - (h.X + 1))) * 0.9f));
                }, HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
                break;
            case HairStyle.Messy:
                // tousled top, choppy bangs falling onto the forehead, a couple of tufts sticking up
                c.Draw(q =>
                {
                    float jag = MathF.Abs(((q.X - h.X) * 0.7f % 2f + 2f) % 2f - 1f) * 1.3f;   // (was 2.4: too wild)
                    var top = S.I(S.Circle(q, h + new V(-0.4f, -1.6f), 10.7f) + Wobble(q, h, 1.2f),
                        S.U(q.Y - (h.Y - 4.5f + jag * MathF.Max(0, MathF.Min(1, (q.X - h.X) / 3f))) - MathF.Max(0, -(q.X - h.X)) * 0.6f,
                            S.I(q.X - (h.X - 5f), q.Y - (h.Y + 0.5f))));
                    return S.U(top, S.U(S.Taper(q, h + new V(-1, -10), h + new V(-2.5f, -12.3f), 1.6f, 0.4f),
                        S.Taper(q, h + new V(3, -10.5f), h + new V(4, -12f), 1.4f, 0.4f)));
                }, HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
                break;
            case HairStyle.Fringe:
                // short and neat, with a straight fringe cut across the forehead
                c.Draw(q => S.I(S.Circle(q, h + new V(-0.6f, -1.2f), 10.9f) + Wobble(q, h, 0.5f),
                    S.U(q.Y - (h.Y - 3.6f) - MathF.Max(0, -(q.X - (h.X + 1))) * 0.7f, S.I(q.X - (h.X - 4f), q.Y - (h.Y + 1.5f)))),
                    HairC, hi: HairC.Light, line: HairC.Mul(0.55f));
                for (float x = h.X + 1; x < h.X + 8; x += 2) c.Plot(x, h.Y - 4.5f, HairC.Shade);   // strands in the fringe
                break;
            case HairStyle.Crew:
                // a close crew cut hugging the skull, hairline set back from the brow
                c.Draw(q => S.I(S.I(S.Circle(q, h + new V(-0.3f, -0.6f), 10.7f),
                    S.U(q.Y - (h.Y - 5.5f) - MathF.Max(0, -(q.X - (h.X + 2))) * 0.8f, S.I(q.X - (h.X - 4f), q.Y - (h.Y + 1)))), q.X - (h.X + 8.5f)),
                    HairC, hi: HairC.Light, line: HairC.Mul(0.55f));

                break;
            case HairStyle.Bald:
            case HairStyle.Buzzed:
                if (Hair == HairStyle.Buzzed)   // a shaved shadow on the sides and back
                    c.Flat(q => S.I(S.I(S.Circle(q, h, Rig.HeadR - 0.5f), S.I(q.X - (h.X + 0.5f), -(q.Y - (h.Y - 6f)))),
                        S.I(q.Y - (h.Y + 1.5f), -S.Ellipse(q, h + new V(-7f, 2.6f), 2.8f, 3.6f))), Skin.Lerp(BeardC, 0.1f));
                c.Plot(h.X - 3, h.Y - 7, Skin.Light.Light); c.Plot(h.X - 2, h.Y - 7, Skin.Light.Light); c.Plot(h.X - 4, h.Y - 6, Skin.Light.Light);
                c.Plot(h.X - 4, h.Y - 5, Skin.Light);
                break;
        }
    }

    protected void DrawEyes(Canvas c, V h, Face f)
    {
        float[] xs = [h.X + 2.5f - FaceShift, h.X + 6.5f - FaceShift];
        float y = h.Y + 1;
        foreach (var x in xs)
        {
            switch (f)
            {
                case Face.Blink:
                    c.Plot(x - 0.5f, y + 1, Eye); c.Plot(x + 0.5f, y + 1, Eye);
                    break;
                case Face.Hurt:
                    c.Plot(x - 0.5f, y - 1, Eye); c.Plot(x + 0.5f, y, Eye); c.Plot(x + 1.5f, y, Eye); c.Plot(x - 0.5f, y + 1, Eye);
                    break;
                case Face.Dead:
                    c.Plot(x - 1, y - 1, Eye); c.Plot(x + 1, y - 1, Eye); c.Plot(x, y, Eye); c.Plot(x - 1, y + 1, Eye); c.Plot(x + 1, y + 1, Eye);
                    break;
                case Face.Happy:
                    c.Plot(x - 0.5f, y + 1, Eye); c.Plot(x + 0.5f, y, Eye); c.Plot(x + 1.5f, y + 1, Eye);
                    break;
                case Face.Down:
                    // looking down: the eyes sit low under a heavy lid line
                    c.Plot(x - 0.5f, y, Skin.Shade); c.Plot(x + 0.5f, y, Skin.Shade);
                    c.Plot(x - 0.5f, y + 1, Eye); c.Plot(x + 0.5f, y + 1, Eye);
                    break;
                default:
                    // 2 x 3 eyes with a little white glint on the side he's facing
                    for (int ey = -1; ey <= 1; ey++) { c.Plot(x - 0.5f, y + ey, Eye); c.Plot(x + 0.5f, y + ey, Eye); }
                    c.Plot(x + 0.5f, y - 1, Col.Hex("ffffff"));
                    if (f == Face.Fierce) { c.Plot(x - 1, y - 2, BeardC.Shade); c.Plot(x, y - 2, BeardC.Shade); c.Plot(x + 1, y - 1.5f, BeardC.Shade); }
                    break;
            }
        }
    }

    /// <summary>Rectangular glasses over the eyes, with an arm back to the ear: a solid brow bar on top, the rest of the
    /// rim softer so the face shows through, and a faint glassy tint over the skin inside each lens. Facing the player
    /// they're drawn by <see cref="DrawFrontGlasses"/> instead.</summary>
    protected void DrawGlasses(Canvas c, V h, Col frame)
    {
        if (FaceShift > 0) { DrawFrontGlasses(c, h + new V(-FaceShift, 0), frame); return; }
        var rim = frame.Lerp(Skin, 0.4f);
        c.Draw(q => S.Capsule(q, h + new V(-6.2f, 0.6f), h + new V(0.5f, -0.6f), 0.5f), frame, outline: false, noShade: true);
        // the far lens is a little narrower, turned away
        foreach (var (x0, x1) in new[] { (1.2f, 4f), (5.2f, 7.9f) })
        {
            Sdf inner = q => S.Box(q, h.X + x0, h.Y - 0.2f, h.X + x1, h.Y + 2.4f);
            c.Recolor(inner, px => px == Eye || px == Col.Hex("ffffff") ? px : px.Lerp(Col.Hex("dff0ff"), 0.3f));
            c.Flat(q => S.I(S.Box(q, h.X + x0 - 1, h.Y - 1.2f, h.X + x1 + 1, h.Y + 3.4f), -inner(q)), rim);
            c.Flat(q => S.Box(q, h.X + x0 - 1, h.Y - 1.2f, h.X + x1 + 1, h.Y - 0.2f), frame);
        }
    }

    /// <summary>Glasses on a face turned toward the player: wide, short wire-frame lenses (all the brothers' glasses are
    /// wider than they are tall) spanning the face from the near lens to the far side of the face, a bridge, and an arm
    /// back to the ear. No tint and nothing drawn inside the lenses, so the eyes always show whole. <paramref name="h"/> is already shifted
    /// with the face, so the eyes are at h.X + 2.5 and h.X + 6.5, rows h.Y to h.Y + 2.</summary>
    private void DrawFrontGlasses(Canvas c, V h, Col frame)
    {
        // Drawn on exact pixels (fractional shapes rounded differently from the eyes and landed on them): the inside of
        // each lens is exactly the eye's three rows, with a one-pixel frame line above and below.
        var wire = frame.Lerp(Skin, 0.15f);
        int PixX(float mx) { for (int x = 0; x < c.W; x++) if (c.ModelOf(new V(x + 1, 0)).X > mx) return x; return c.W - 1; }
        int PixY(float my) { for (int y = 0; y < c.H; y++) if (c.ModelOf(new V(0, y + 1)).Y > my) return y; return c.H - 1; }
        int top = PixY(h.Y) - 1, bottom = PixY(h.Y + 2) + 1;   // the eyes are plotted on rows h.Y .. h.Y + 2
        void Box(int x0, int x1)
        {
            for (int x = x0; x <= x1; x++) { c.Set(x, top, wire); c.Set(x, bottom, wire); }
            for (int y = top; y <= bottom; y++) { c.Set(x0, y, wire); c.Set(x1, y, wire); }
        }
        int nearL = PixX(h.X + 0.4f), nearR = PixX(h.X + 4.6f), farL = PixX(h.X + 5.4f), farR = PixX(h.X + 10f);
        Box(nearL, nearR);                                                   // near lens
        Box(farL, farR);                                                     // far lens, out to the side of the face
        for (int x = nearR; x <= farL; x++) c.Set(x, top + 1, wire);         // bridge
        int ear = PixX(h.X - 4.4f);
        for (int x = ear; x < nearL; x++) c.Set(x, x < (ear + nearL) / 2 ? top + 2 : top + 1, wire);   // near arm, back to the ear
    }
    private static void TintAll(Canvas c, Col tint, float t)
    {
        for (int i = 0; i < c.Px.Length; i++)
            if (c.Px[i] is { } px && px != Canvas.Outline) c.Px[i] = px.Lerp(tint, t);
    }

    // ---- prop helpers ----

    /// <summary>A swing trail: a thin arc of light pixels around a center.</summary>
    protected static void Swoosh(Canvas c, V center, float radius, float from, float to, Col col)
    {
        for (float a = from; a <= to; a += 2f)
        {
            var pt = center + V.Dir(a) * radius;
            c.Plot(pt, col);
            if ((a - from) / (to - from) > 0.3f) c.Plot(pt + V.Dir(a) * -1, col.Mul(0.85f));
        }
    }

    protected static void Note(Canvas c, V at, Col col)
    {
        // a single eighth note: head, stem, flag
        c.Draw(q => S.Ellipse(q, at, 1.8f, 1.4f), col);
        c.Draw(q => S.Capsule(q, at + new V(1.4f, 0), at + new V(1.4f, -5.5f), 0.45f), col, noShade: true);
        c.Draw(q => S.Capsule(q, at + new V(1.4f, -5.5f), at + new V(3.2f, -3.5f), 0.45f), col, noShade: true);
    }

    protected static void Sparkle(Canvas c, V at, int size, Col col)
    {
        c.Plot(at, Col.Hex("ffffff"));
        for (int i = 1; i <= size; i++)
        {
            c.Plot(at + new V(i, 0), col); c.Plot(at + new V(-i, 0), col);
            c.Plot(at + new V(0, i), col); c.Plot(at + new V(0, -i), col);
        }
    }
}
