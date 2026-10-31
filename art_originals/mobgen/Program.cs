// Renders the regular-fight foes in pixelgen's Mobs.cs and the Stargate bosses (Stargate.cs), nothing else: out/<name>/frames/<anim>_<n>.png plus a
// preview sheet per foe. Copy out/<name>/frames into SpireBrothers/images/monsters/<name>/frames.
// Run: dotnet run -c Release [name ...]   (no names = all of them)
var all = new IFrameArt[] { new CreeperArt(), new SkeletonArt(), new SpiderArt(), new EndermanArt(),
    new MobRig(new LookoutArt()), new MobRig(new PowderMonkeyArt()),
    new MobRig(new PhilistineScoutArt()), new JackalArt(), new RoombaArt(), new LaptopArt(),
    new LaundryArt(), new DishesArt(), new LawnArt(),
    new MobRig(new PoxwalkerArt()),
    new MobRig(new ApophisArt()), new MobRig(new JaffaArt()), new SarcophagusArt(), new MobRig(new BaalArt()),
    new MobRig(new AnubisArt()), new MobRig(new KullArt()) };
var only = args.ToHashSet();
var bg = Col.Hex("3a2e2a");
foreach (var foe in all.Where(f => only.Count == 0 || only.Contains(f.Name)))
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
            frames[i].SavePng(Path.Combine("out", foe.Name, "frames", $"{name}_{i}.png"));
            for (int y = 0; y < fh; y++) for (int x = 0; x < fw; x++) sheet.Set(i * (fw + 2) + x, row * (fh + 2) + y, frames[i].Get(x, y) ?? bg);
        }
        row++;
    }
    sheet.SavePng(Path.Combine("out", $"preview_{foe.Name}.png"), 4);
    Console.WriteLine($"{foe.Name}: " + string.Join(", ", anims.Select(a => $"{a.Key} x{a.Value.Count}")));
    // the bosses' map icons: copy out/<name>/map/* into SpireBrothers/images/map/
    if (foe.Name is "apophis" or "baal" or "anubis")
    {
        var (icon, outline) = BossIcon.MapIcon(anims["idle"][0]);
        icon.SavePng(Path.Combine("out", foe.Name, "map", $"{foe.Name}_boss_icon.png"));
        outline.SavePng(Path.Combine("out", foe.Name, "map", $"{foe.Name}_boss_icon_outline.png"));
    }
}

/// <summary>Rig-drawn foes (people) as frame sets, like pixelgen's RigFrames.</summary>
class MobRig(AnimatedBrother art) : IFrameArt
{
    public string Name => art.Name;
    public Dictionary<string, List<Canvas>> Frames() =>
        art.Anims().ToDictionary(a => a.Key, a => a.Value.poses.Select(art.Render).ToList());
}
