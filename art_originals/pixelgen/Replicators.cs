// The Replicators (Stargate SG-1), the second Act 2 boss: the Replicator Queen and her bugs. Rendered by mobgen
// (replicator_queen replicator_bug), which also writes the Queen's map icon (images/map/replicators_boss_icon.png + _outline.png).
// Spider-bugs built out of little grey blocks, every block with a light top-left edge and a dark bottom-right one, so the
// seams show. No eyes. Attack: lunges with its pincers open. Cast (the Queen's Build): rears up while loose blocks fly in.
// Dead: falls apart, the blocks tumbling to the floor in a heap. Drawn facing right; the game mirrors them.

public static class ReplicatorParts
{
    public static readonly Col Metal = Col.Hex("858b94"), Hi = Col.Hex("b4bac4"), Lo = Col.Hex("50565f"), Leg = Col.Hex("5a6068");

    /// <summary>A rect tiled with cell x cell blocks. Corner cells are left off when rounded, for a bug-ish shape.</summary>
    public static void Blocks(Canvas c, int x0, int y0, int x1, int y1, int cell = 3, bool rounded = false)
    {
        int cols = (x1 - x0 + 1) / cell, rows = (y1 - y0 + 1) / cell;
        for (int gy = 0; gy < rows; gy++)
            for (int gx = 0; gx < cols; gx++)
            {
                if (rounded && (gx == 0 || gx == cols - 1) && (gy == 0 || gy == rows - 1)) continue;
                Block(c, x0 + gx * cell, y0 + gy * cell, cell, (gx * 7 + gy * 3) % 5 == 0 ? 0.9f : 1f);
            }
    }

    public static void Block(Canvas c, int x, int y, int cell, float shade = 1f)
    {
        for (int v = 0; v < cell; v++)
            for (int u = 0; u < cell; u++)
                c.Set(x + u, y + v, u + v < cell - 1 ? Hi.Mul(shade) : u + v > cell - 1 ? Lo : Metal.Mul(shade));   // bevelled: lit top-left, shaded bottom-right
    }

    /// <summary>
    /// The body coming apart: every cell x cell square of it drops toward the floor (t = 0 standing, 1 a heap), each
    /// one drifting a little sideways. Draw the body without its outline first; this outlines the result.
    /// </summary>
    public static Canvas Crumble(Canvas body, float t, int cell = 3)
    {
        var c = new Canvas(body.W, body.H);
        for (int gy = 0; gy + cell <= body.H; gy += cell)
            for (int gx = 0; gx + cell <= body.W; gx += cell)
            {
                bool any = false;
                for (int v = 0; v < cell && !any; v++) for (int u = 0; u < cell && !any; u++) any = body.Get(gx + u, gy + v) != null;
                if (!any) continue;
                uint h = (uint)(gx * 73856093 ^ gy * 19349663); h ^= h >> 13; h *= 0x5bd1e995; h ^= h >> 15;
                float middle = 1 - Math.Abs(gx - body.W / 2f) / body.W * 2;   // the heap is taller in the middle
                float floor = body.H - cell - (h % 3) * cell * middle;
                int ny = (int)(gy + (floor - gy) * t);
                int nx = gx + (int)(((h >> 5) % 9 - 4f) * t * 1.5f);
                for (int v = 0; v < cell; v++) for (int u = 0; u < cell; u++)
                    if (body.Get(gx + u, gy + v) is { } px) c.Set(nx + u, ny + v, px);
            }
        Px.Outline(c);
        return c;
    }

    public static Canvas Finish(Canvas c, float red)
    {
        Px.Outline(c);
        Px.Tint(c, Col.Hex("ff4040"), red);
        return c;
    }
}

// 48 x 30. A small Replicator bug: a block abdomen, a smaller block head, pincers, three legs a side.
public class ReplicatorBugArt : IFrameArt
{
    public string Name => "replicator_bug";
    const int W = 48, H = 30;

    record struct P(int Dx = 0, int Rise = 0, int Step = 0, float Red = 0, bool Open = false);

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(), new(Step: 1), new(), new(Step: 2) }.Select(Draw).ToList(),
        ["attack"] = new P[] { new(Dx: -2), new(Dx: 5, Open: true), new(Dx: 6, Open: true), new(Dx: 2), new() }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Rise: 2), new(Rise: 4, Step: 1), new(Rise: 2), new() }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Dx: -2, Red: 0.4f), new(Dx: -1, Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new[] { 0.2f, 0.55f, 1f }.Select(t => ReplicatorParts.Crumble(Body(new()), t)).ToList(),
    };

    static Canvas Draw(P p) => ReplicatorParts.Finish(Body(p), p.Red);

    static Canvas Body(P p)
    {
        var c = new Canvas(W, H);
        int cx = 24 + p.Dx, top = 13 - p.Rise;
        // legs: short and splayed wide, knees just above the back, feet out to the sides
        for (int side = 0; side < 2; side++)
            for (int i = 0; i < 3; i++)
            {
                var col = side == 0 ? ReplicatorParts.Leg.Mul(0.75f) : ReplicatorParts.Leg;
                float hipX = cx - 6 + i * 5 + side * 2;
                bool lifted = (p.Step == 1 && i % 2 == 0) || (p.Step == 2 && i % 2 == 1);
                float spread = (i - 1f) * 9 + (side == 0 ? -3 : 3);
                var hip = new V(hipX, top + 5);
                var knee = new V(hipX + spread * 0.9f, top - 1 - (lifted ? 2 : 0));
                var foot = new V(hipX + spread * 1.6f, H - 2 - (lifted ? 2 : 0));
                Px.Line(c, hip, knee, col, 2);
                Px.Line(c, knee, foot, col, 2);
            }
        ReplicatorParts.Blocks(c, cx - 16, top - 1, cx + 1, top + 10, rounded: true);
        ReplicatorParts.Blocks(c, cx + 2, top + 2, cx + 10, top + 7);
        // pincers
        int o = p.Open ? 2 : 0;
        Px.Rect(c, cx + 12, top + 2 - o, cx + 14, top + 3 - o, ReplicatorParts.Lo);
        Px.Rect(c, cx + 12, top + 6 + o, cx + 14, top + 7 + o, ReplicatorParts.Lo);
        c.Set(cx + 15, top + 3 - o, ReplicatorParts.Hi);
        c.Set(cx + 15, top + 6 + o, ReplicatorParts.Hi);
        return c;
    }
}

// 96 x 64. The Replicator Queen: the same build at twice the size, a long segmented abdomen, a thorax, a wedge head with
// big pincers, and thicker legs. Cast: rears up while loose blocks fly in toward her front.
public class ReplicatorQueenArt : IFrameArt
{
    public string Name => "replicator_queen";
    const int W = 96, H = 64;

    record struct P(int Dx = 0, int Rise = 0, int Step = 0, float Red = 0, bool Open = false, float Fly = 0);
    // Fly: 0 = no flying blocks; otherwise how far in they've come, 0.01..1.

    public Dictionary<string, List<Canvas>> Frames() => new()
    {
        ["idle"] = new P[] { new(), new(Step: 1), new(), new(Step: 2) }.Select(Draw).ToList(),
        ["attack"] = new P[] { new(Dx: -3), new(Dx: 6, Open: true), new(Dx: 9, Open: true), new(Dx: 4), new() }.Select(Draw).ToList(),
        ["cast"] = new P[] { new(Rise: 3, Fly: 0.01f), new(Rise: 6, Fly: 0.35f), new(Rise: 7, Step: 1, Fly: 0.7f), new(Rise: 5, Fly: 1), new(Rise: 2) }.Select(Draw).ToList(),
        ["hit"] = new P[] { new(Dx: -3, Red: 0.4f), new(Dx: -1, Red: 0.2f), new() }.Select(Draw).ToList(),
        ["dead"] = new[] { 0.15f, 0.45f, 0.75f, 1f }.Select(t => ReplicatorParts.Crumble(Body(new()), t)).ToList(),
    };

    static Canvas Draw(P p) => ReplicatorParts.Finish(Body(p), p.Red);

    static Canvas Body(P p)
    {
        var c = new Canvas(W, H);
        int cx = 56 + p.Dx, top = 30 - p.Rise;
        // legs: splayed wide, knees a little above the back, feet far out to the sides
        for (int side = 0; side < 2; side++)
            for (int i = 0; i < 3; i++)
            {
                var col = side == 0 ? ReplicatorParts.Leg.Mul(0.75f) : ReplicatorParts.Leg;
                float hipX = cx - 6 + i * 7 + side * 3;
                bool lifted = (p.Step == 1 && i % 2 == 0) || (p.Step == 2 && i % 2 == 1);
                float spread = (i - 1f) * 13 + (side == 0 ? -4 : 4);
                var hip = new V(hipX, top + 8);
                var knee = new V(hipX + spread * 0.9f, top - 5 - (lifted ? 3 : 0));
                var foot = new V(hipX + spread * 1.5f, H - 3 - (lifted ? 3 : 0));
                Px.Line(c, hip, knee, col, 3);
                Px.Line(c, knee, foot, col, 3);
            }
        // abdomen: two segments, the big round one at the back
        ReplicatorParts.Blocks(c, cx - 51, top - 7, cx - 24, top + 20, 4, rounded: true);
        ReplicatorParts.Blocks(c, cx - 23, top - 2, cx - 8, top + 17, 4, rounded: true);
        // thorax, head, pincers
        ReplicatorParts.Blocks(c, cx - 7, top + 1, cx + 8, top + 16, 4);
        ReplicatorParts.Blocks(c, cx + 9, top + 3, cx + 20, top + 14, 3);
        int o = p.Open ? 3 : 0;
        Px.Rect(c, cx + 21, top + 4 - o, cx + 26, top + 5 - o, ReplicatorParts.Lo);
        Px.Rect(c, cx + 21, top + 11 + o, cx + 26, top + 13 + o, ReplicatorParts.Lo);
        Px.Rect(c, cx + 27, top + 4 - o, cx + 28, top + 6 - o, ReplicatorParts.Hi);
        Px.Rect(c, cx + 27, top + 11 + o, cx + 28, top + 12 + o, ReplicatorParts.Hi);
        // Build: loose blocks flying in from above and behind toward the floor in front of her
        if (p.Fly > 0)
        {
            var dest = new V(Math.Min(cx + 34, W - 4), H - 6);
            V[] from = [new(W - 4, 2), new(W - 2, 26), new(cx + 8, 0), new(W - 10, 12), new(cx + 22, 4)];
            for (int i = 0; i < from.Length; i++)
            {
                float t = Math.Clamp(p.Fly + i * 0.08f, 0, 1);
                var at = from[i] + (dest - from[i]) * t;
                ReplicatorParts.Block(c, (int)at.X - 1, (int)at.Y - 1, 3);
            }
        }
        return c;
    }
}
