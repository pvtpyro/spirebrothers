var outDir = args.Length > 0 ? args[0] : "out";
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
