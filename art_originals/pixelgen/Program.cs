var outDir = args.Length > 0 ? args[0] : "out";
if (args.Contains("brandi")) { BrandiArt.Preview(outDir); return; }
AnimatedBrother[] brothers = [new DanielArt(), new DavidArt(), new JoshuaArt(), new TimArt()];
var bg = Col.Hex("5a5f6a");
foreach (var b in brothers)
{
    var anims = b.Anims();
    int maxF = anims.Values.Max(a => a.poses.Count);
    var sheet = new Canvas(64 * maxF + (maxF - 1) * 2, anims.Count * 82 - 2);
    int row = 0;
    foreach (var (name, (poses, fps, loop)) in anims)
    {
        for (int i = 0; i < poses.Count; i++)
        {
            var frame = b.Render(poses[i]);
            frame.SavePng(Path.Combine(outDir, b.Name, "frames", $"{name}_{i}.png"));
            for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++)
                sheet.Set(i * 66 + x, row * 82 + y, frame.Get(x, y) ?? bg);
        }
        row++;
    }
    sheet.SavePng(Path.Combine(outDir, $"preview_{b.Name}.png"), 3);
    Console.WriteLine($"{b.Name}: " + string.Join(", ", anims.Select(a => $"{a.Key} x{a.Value.poses.Count}")));
}
// The elites and their helpers (Captain Blockbeard, and the Bible elites: Goliath, the Lions' Den, the Walls of
// Jericho, Alexander the Coppersmith): frames only, no select screen, hands, or campfire. People are drawn on the Rig;
// lions and the Wall draw their own frames (Beasts.cs).
var foes = new List<IFrameArt>();
foreach (AnimatedBrother foe in new AnimatedBrother[] { new PirateArt(), new GoliathArt(), new ShieldBearerArt(),
    new JerichoGuardArt(), new JerichoArcherArt(), new AlexanderArt(), new CopperAutomatonArt() })
    foes.Add(new RigFrames(foe));
foes.AddRange([new LionArt(mane: true), new LionArt(mane: false), new WallArt()]);
foreach (var foe in foes)
{
    var anims = foe.Frames();
    int fw = anims.Values.First()[0].W, fh = anims.Values.First()[0].H;
    int maxF = anims.Values.Max(a => a.Count);
    var sheet = new Canvas(fw * maxF + (maxF - 1) * 2, anims.Count * (fh + 2) - 2);
    int row = 0;
    foreach (var (name, frames) in anims)
    {
        for (int i = 0; i < frames.Count; i++)
        {
            frames[i].SavePng(Path.Combine(outDir, foe.Name, "frames", $"{name}_{i}.png"));
            for (int y = 0; y < fh; y++) for (int x = 0; x < fw; x++) sheet.Set(i * (fw + 2) + x, row * (fh + 2) + y, frames[i].Get(x, y) ?? bg);
        }
        row++;
    }
    sheet.SavePng(Path.Combine(outDir, $"preview_{foe.Name}.png"), 3);
    Console.WriteLine($"{foe.Name}: " + string.Join(", ", anims.Select(a => $"{a.Key} x{a.Value.Count}")));
}
Backgrounds.RenderAll(brothers, outDir);
Hands.RenderAll(brothers, outDir);
// big side-by-side of everyone's first idle frame, at game scale
var lineup = new Canvas(64 * 4 + 6, 80);
for (int i = 0; i < brothers.Length; i++)
{
    var f = brothers[i].Render(brothers[i].Anims()["idle"].poses[0]);
    for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) lineup.Set(i * 66 + x, y, f.Get(x, y) ?? bg);
}
lineup.SavePng(Path.Combine(outDir, "lineup.png"), 5);
// zoomed heads for checking faces
var heads = new Canvas(34 * 4, 30);
for (int i = 0; i < brothers.Length; i++)
{
    var f = brothers[i].Render(brothers[i].Anims()["idle"].poses[0]);
    for (int y = 0; y < 30; y++) for (int x = 0; x < 34; x++) heads.Set(i * 34 + x, y, f.Get(x + 14, y + 2) ?? bg);
}
heads.SavePng(Path.Combine(outDir, "heads.png"), 8);
// everyone sitting at the campfire (first rest frame), at game scale
var camp = new Canvas(64 * 4 + 6, 80);
for (int i = 0; i < brothers.Length; i++)
{
    var f = brothers[i].Render(brothers[i].Anims()["rest"].poses[0]);
    for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) camp.Set(i * 66 + x, y, f.Get(x, y) ?? bg);
}
camp.SavePng(Path.Combine(outDir, "campfire.png"), 5);
// every rest frame for everyone, zoomed, for checking the campfire loops
var rest = new Canvas(66 * 8, 82 * brothers.Length);
for (int b = 0; b < brothers.Length; b++)
{
    var poses = brothers[b].Anims()["rest"].poses;
    for (int i = 0; i < poses.Count; i++)
    {
        var f = brothers[b].Render(poses[i]);
        for (int y = 0; y < 80; y++) for (int x = 0; x < 64; x++) rest.Set(i * 66 + x, b * 82 + y, f.Get(x, y) ?? bg);
    }
}
rest.SavePng(Path.Combine(outDir, "campfire_frames.png"), 3);

/// <summary>A Rig-drawn character's frames, in the same shape as the hand-drawn ones.</summary>
class RigFrames(AnimatedBrother art) : IFrameArt
{
    public string Name => art.Name;
    public Dictionary<string, List<Canvas>> Frames() =>
        art.Anims().ToDictionary(a => a.Key, a => a.Value.poses.Select(art.Render).ToList());
}