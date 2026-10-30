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

        // Saved fields register with BaseLib when created, which has to happen before the game finishes loading mods.
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Mechanics.Ages).TypeHandle);
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Mechanics.RunStats).TypeHandle);
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Mechanics.Grind).TypeHandle);
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Mechanics.StimmLab).TypeHandle);
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Mechanics.BrandiSaves).TypeHandle);

        // Character select order: the four brothers in the order they've always had, the friends, then Brandi last (her pick).
        if (!BaseLib.Utils.CustomCharacterUtils.TryOrderCustomCharacters([typeof(Daniel), typeof(David), typeof(Joshua), typeof(Tim), typeof(Huffman), typeof(John), typeof(Brandi)]))
            Logger.Error("Couldn't set the character select order.");

        ShareMonkeyIslandItems();

        // Writes "[Hitch] N ms" to godot.log on slow frames, for tracking down lag spikes.
        Mechanics.HitchLogger.Start();
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
        ModHelper.AddModelToPool<TimRelicPool, MonkeyPhrasebook>();
        ModHelper.AddModelToPool<TimRelicPool, StoneMonkeyHead>();
        ModHelper.AddModelToPool<TimRelicPool, MonkeyWrench>();
        ModHelper.AddModelToPool<TimPotionPool, OokOokEek>();
        ModHelper.AddModelToPool<TimPotionPool, Grog>();
        ModHelper.AddModelToPool<TimPotionPool, MonkeyBusiness>();

        // Shared brother relics, also [Pool]ed to Daniel. Party Headset skips Tim, who has no Share cards.
        ModHelper.AddModelToPool<DavidRelicPool, BlownCartridge>();
        ModHelper.AddModelToPool<DavidRelicPool, PartyHeadset>();
        ModHelper.AddModelToPool<DavidRelicPool, SharedSteamLibrary>();
        ModHelper.AddModelToPool<DavidRelicPool, BrothersInArms>();
        ModHelper.AddModelToPool<JoshuaRelicPool, BlownCartridge>();
        ModHelper.AddModelToPool<JoshuaRelicPool, PartyHeadset>();
        ModHelper.AddModelToPool<JoshuaRelicPool, SharedSteamLibrary>();
        ModHelper.AddModelToPool<JoshuaRelicPool, BrothersInArms>();
        ModHelper.AddModelToPool<TimRelicPool, BlownCartridge>();
        ModHelper.AddModelToPool<TimRelicPool, SharedSteamLibrary>();
        ModHelper.AddModelToPool<TimRelicPool, BrothersInArms>();
        // The friends get Blown Cartridge (not Monkey Island, the family relics, or Party Headset: they have almost no Share cards).
        ModHelper.AddModelToPool<JohnRelicPool, BlownCartridge>();
        ModHelper.AddModelToPool<HuffmanRelicPool, BlownCartridge>();
        ModHelper.AddModelToPool<DavidRelicPool, DadsTackleBox>();
        ModHelper.AddModelToPool<JoshuaRelicPool, DadsTackleBox>();
        ModHelper.AddModelToPool<TimRelicPool, DadsTackleBox>();
        // Brandi: Blown Cartridge like the friends, plus two of the family relics (she has Share cards for the headset). Her memorial relics for Pugsley and Sandi (pets she and her late
        // husband had) are [Pool]ed to her and shared with Daniel.
        ModHelper.AddModelToPool<BrandiRelicPool, BlownCartridge>();
        ModHelper.AddModelToPool<BrandiRelicPool, SharedSteamLibrary>();
        ModHelper.AddModelToPool<BrandiRelicPool, PartyHeadset>();
        ModHelper.AddModelToPool<DanielRelicPool, PugsleysCollar>();
        ModHelper.AddModelToPool<DanielRelicPool, SandisSunbeam>();
    }
}
