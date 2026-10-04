using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using SpireBrothers.SpireBrothersCode.Character;
using SpireBrothers.SpireBrothersCode.Potions;
using SpireBrothers.SpireBrothersCode.Relics;

namespace SpireBrothers.SpireBrothersCode;

//You're recommended but not required to keep all your code in this package and all your assets in the SpireBrothers folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "SpireBrothers"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);
     
        Harmony harmony = new(ModId);

        harmony.PatchAll(assembly);

        ShareMonkeyIslandItems();
    }

    // The Monkey Island relics and potions live in Daniel's pools (their [Pool] attribute); every other
    // brother gets them too. Add each new brother's pools here as he is built.
    private static void ShareMonkeyIslandItems()
    {
        ModHelper.AddModelToPool<DavidRelicPool, MonkeyPhrasebook>();
        ModHelper.AddModelToPool<DavidRelicPool, StoneMonkeyHead>();
        ModHelper.AddModelToPool<DavidRelicPool, MonkeyWrench>();
        ModHelper.AddModelToPool<DavidPotionPool, OokOokEek>();
        ModHelper.AddModelToPool<DavidPotionPool, Grog>();
        ModHelper.AddModelToPool<DavidPotionPool, MonkeyBusiness>();
        ModHelper.AddModelToPool<JoshuaRelicPool, MonkeyPhrasebook>();
        ModHelper.AddModelToPool<JoshuaRelicPool, StoneMonkeyHead>();
        ModHelper.AddModelToPool<JoshuaRelicPool, MonkeyWrench>();
        ModHelper.AddModelToPool<JoshuaPotionPool, OokOokEek>();
        ModHelper.AddModelToPool<JoshuaPotionPool, Grog>();
        ModHelper.AddModelToPool<JoshuaPotionPool, MonkeyBusiness>();
    }
}
