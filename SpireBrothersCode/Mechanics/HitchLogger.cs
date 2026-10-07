using Godot;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// Lag-spike finder: whenever one frame takes longer than <see cref="ThresholdMs"/>, writes "[Hitch] N ms" to godot.log.
/// The log has no timestamps, so the lines just above each hitch show what the game was doing when it happened.
/// Loading screens and alt-tabbing show up too; repeated hitches during play are the interesting ones.
/// </summary>
public static class HitchLogger
{
    private const double ThresholdMs = 150;

    private static ulong _lastUsec;
    private static int _count;

    public static void Start()
    {
        if (Engine.GetMainLoop() is not SceneTree tree) return;
        tree.ProcessFrame += OnFrame;
    }

    private static void OnFrame()
    {
        ulong now = Time.GetTicksUsec();
        if (_lastUsec != 0)
        {
            double ms = (now - _lastUsec) / 1000.0;
            if (ms >= ThresholdMs)
                MainFile.Logger.Info($"[Hitch] #{++_count}: {ms:0} ms frame (fps {Engine.GetFramesPerSecond():0}, " +
                                     $"static mem {OS.GetStaticMemoryUsage() / 1048576} MB, managed {GC.GetTotalMemory(false) / 1048576} MB, " +
                                     $"GCs {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)})");
        }
        _lastUsec = now;
    }
}
